/*
The MIT License (MIT)

Copyright (c) 2013, David Suarez

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MavLinkNet
{
	/// <summary>
	/// Sync events 
	/// implementation of the producer/consumer pattern from MS
	/// </summary>
	public class SyncEvents
	{
		public SyncEvents ()
		{

			_newItemEvent = new AutoResetEvent (false);
			_exitThreadEvent = new ManualResetEvent (false);
			_eventArray = new WaitHandle[2];
			_eventArray [0] = _newItemEvent;
			_eventArray [1] = _exitThreadEvent;
		}

		public EventWaitHandle ExitThreadEvent {
			get { return _exitThreadEvent; }
		}

		public EventWaitHandle NewItemEvent {
			get { return _newItemEvent; }
		}

		public WaitHandle[] EventArray {
			get { return _eventArray; }
		}

		private EventWaitHandle _newItemEvent;
		private EventWaitHandle _exitThreadEvent;
		private WaitHandle[] _eventArray;
	}

	/// <summary>
	/// Mav link UDP transporter
	/// Async implementation with high performance
	/// </summary>
	public class MavLinkUdpTransport: MavLinkGenericTransport
	{
		public int UdpListeningPort = 14550;
		// Any available port
		public int UdpTargetPort = 64448;
		public IPAddress TargetIpAddress = new IPAddress (new byte[] { 100, 65, 9, 7 });
		public int HeartBeatUpdateRateMs = 1000;

		private Queue<byte[]> mReceiveQueue = new Queue<byte[]> ();
		private Queue<UasMessage> mSendQueue = new Queue<UasMessage> ();
		private AutoResetEvent mReceiveSignal = new AutoResetEvent (true);
		// not in use

		private AutoResetEvent mSendSignal = new AutoResetEvent (true);
		private MavLinkAsyncWalker mMavLink = new MavLinkAsyncWalker ();
		private UdpClient mUdpClient;
		private bool mIsActive = true;
		private bool mConnected = false;


		SyncEvents recvSyncEvents = new SyncEvents ();
		SyncEvents sendSyncEvents = new SyncEvents ();


		public override void Initialize ()
		{
			InitializeMavLink ();
			InitializeUdpListener (UdpListeningPort);
//			InitializeUdpSender (TargetIpAddress, UdpTargetPort);

//			Debug.Log ("udp transporter ready!");
		}

		public override void Dispose ()
		{
			mIsActive = false;
			mUdpClient.Close ();
			mReceiveSignal.Set ();
			mSendSignal.Set ();
		}

		private void InitializeMavLink ()
		{
			mMavLink.PacketReceived += HandlePacketReceived;
		}

		private void InitializeUdpListener (int port)
		{
			// Create UDP listening socket on port
			IPEndPoint ep = new IPEndPoint (IPAddress.Any, port);
			mUdpClient = new UdpClient (ep);


			mUdpClient.BeginReceive (
				new AsyncCallback (ReceiveCallback), ep);

			ThreadPool.QueueUserWorkItem (
				new WaitCallback (ProcessReceiveQueue), null);
		}

		private void InitializeUdpSender (IPAddress targetIp, int targetPort)
		{
			ThreadPool.QueueUserWorkItem (
				new WaitCallback (ProcessSendQueue), new IPEndPoint (targetIp, targetPort));
			this.BeginHeartBeatLoop ();

		}


		// __ Receive _________________________________________________________


		private void ReceiveCallback (IAsyncResult ar)
		{
			try {
				IPEndPoint ep = new IPEndPoint (0, 0);//= ar.AsyncState as IPEndPoint;

				byte[] buffer = mUdpClient.EndReceive (ar, ref ep);
//				Console.print (string.Format ("received ip: {0}  port:   {1}", ep.Address.ToString (), ep.Port.ToString ()));
				//set the target IP and Port

				if (!mConnected) {
					mConnected = true;
					this.TargetIpAddress = new IPAddress (ep.Address.GetAddressBytes ());
					this.UdpTargetPort = ep.Port;
					this.InitializeUdpSender (this.TargetIpAddress, this.UdpTargetPort);
				}
					

//				Console.print (string.Format ("target ip: {0}  port:   {1}", ep.Address.ToString (), ep.Port.ToString ()));

				if (!recvSyncEvents.ExitThreadEvent.WaitOne (0, false)) {
					lock (((ICollection)mReceiveQueue).SyncRoot) {						
						mReceiveQueue.Enqueue (buffer);
						recvSyncEvents.NewItemEvent.Set ();
					}				
				}

				if (!mIsActive) {
					Console.print ("CONNECTION LOST 1");
					// process all data already received
					recvSyncEvents.NewItemEvent.Set ();
					return;
				}
				mUdpClient.BeginReceive (new AsyncCallback (ReceiveCallback), ar);

			} catch (SocketException) {
				mIsActive = false;
			}
		}

		private void ProcessReceiveQueue (object state)
		{
			
			while (true) {
				byte[] buffer;

				if (WaitHandle.WaitAny (recvSyncEvents.EventArray) != 1) {
					lock (((ICollection)mReceiveQueue).SyncRoot) {
						buffer = mReceiveQueue.Dequeue ();
//						Console.print ("dequeue..");
						mMavLink.ProcessReceivedBytes (buffer, 0, buffer.Length);
					}
				}
			}

			HandleReceptionEnded (this);
		}


		// __ Send ____________________________________________________________


		private void ProcessSendQueue (object state)
		{
			while (true) {
				UasMessage msg = new UasMessage ();

				if (WaitHandle.WaitAny (sendSyncEvents.EventArray) != 1) {
					lock (((ICollection)mSendQueue).SyncRoot) {
						msg = mSendQueue.Dequeue ();
						SendMavlinkMessage (state as IPEndPoint, msg);
					}
				}

				if (!mIsActive)
					break;

			}
		}

		private void SendMavlinkMessage (IPEndPoint ep, UasMessage msg)
		{
			byte[] buffer = mMavLink.SerializeMessage (msg, MavlinkSystemId, MavlinkComponentId, true);

			mUdpClient.Send (buffer, buffer.Length, ep);
		}


		// __ Heartbeat _______________________________________________________


		public void BeginHeartBeatLoop ()
		{
			ThreadPool.QueueUserWorkItem (new WaitCallback (HeartBeatLoop), null);
		}

		private void HeartBeatLoop (object state)
		{
			while (true) {
				foreach (UasMessage m in UavState.GetHeartBeatObjects()) {
					SendMessage (m);
				}

				Thread.Sleep (HeartBeatUpdateRateMs);
			}
		}


		// __ API _____________________________________________________________


		public override void SendMessage (UasMessage msg)
		{
			if (!this.mConnected) {
				Console.print ("connection lost. please connect to the vehicle first");
				return;
			}

			Console.print ("send msg id: " + msg.MessageId.ToString ());
			if (!sendSyncEvents.ExitThreadEvent.WaitOne (0, false)) {
				lock (((ICollection)mSendQueue).SyncRoot) {
					mSendQueue.Enqueue (msg);
					sendSyncEvents.NewItemEvent.Set ();
				}

			}
		}
	}


}
