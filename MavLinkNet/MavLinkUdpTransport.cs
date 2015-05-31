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
using System.Collections.Generic;
using UnityEngine;

namespace MavLinkNet
{
	public class MavLinkUdpTransport: MavLinkGenericTransport
	{
		public int UdpListeningPort = 7777;
		// Any available port
		public int UdpTargetPort = 7778;
		public IPAddress TargetIpAddress = new IPAddress (new byte[] { 127, 0, 0, 1 });
		public int HeartBeatUpdateRateMs = 1000;

		private Queue<byte[]> mReceiveQueue = new Queue<byte[]> ();
		private Queue<UasMessage> mSendQueue = new Queue<UasMessage> ();
		private AutoResetEvent mReceiveSignal = new AutoResetEvent (true);
		private AutoResetEvent mSendSignal = new AutoResetEvent (true);
		private MavLinkAsyncWalker mMavLink = new MavLinkAsyncWalker ();
		private UdpClient mUdpClient;
		private bool mIsActive = true;


		public override void Initialize ()
		{
			InitializeMavLink ();
			InitializeUdpListener (UdpListeningPort);
			InitializeUdpSender (TargetIpAddress, UdpTargetPort);
			Debug.Log ("udp transporter ready!");
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
		}


		// __ Receive _________________________________________________________


		private void ReceiveCallback (IAsyncResult ar)
		{
			try {
				IPEndPoint ep = ar.AsyncState as IPEndPoint;
				mReceiveQueue.Enqueue (mUdpClient.EndReceive (ar, ref ep));

				if (!mIsActive) {
					mReceiveSignal.Set ();
					return;
				}
				mReceiveSignal.Set ();
				mUdpClient.BeginReceive (new AsyncCallback (ReceiveCallback), ar);

				// Signal processReceive thread

			} catch (SocketException) {
				mIsActive = false;
			}
		}

		private void ProcessReceiveQueue (object state)
		{
			while (true) {
				byte[] buffer;


				try {
					buffer = mReceiveQueue.Dequeue ();
				} catch (Exception e) {
					Console.print ("cannot dequeue " + e.Message);
					mReceiveSignal.WaitOne ();
					if (!mIsActive) {
						Console.print ("mIsActive false!!! connection lost");
						break;
					}
					
					continue;
				}
				Console.print ("buffer length: " + buffer.Length.ToString ());
				mMavLink.ProcessReceivedBytes (buffer, 0, buffer.Length);


				/*
				if (mReceiveQueue.TryDequeue (out buffer)) {
					mMavLink.ProcessReceivedBytes (buffer, 0, buffer.Length);
				} else {
					// Empty queue, sleep until signalled
					mReceiveSignal.WaitOne ();

					if (!mIsActive)
						break;
				}
				*/
			}

			HandleReceptionEnded (this);
		}


		// __ Send ____________________________________________________________


		private void ProcessSendQueue (object state)
		{
			while (true) {
				UasMessage msg = new UasMessage ();

				try {
					msg = mSendQueue.Dequeue ();
				} catch (Exception e) {
					// Queue is empty, sleep until signalled
					Console.print ("send queue empty: " + e.Message);
					mSendSignal.WaitOne ();

					if (!mIsActive)
						break;
					continue;
				} finally {
					SendMavlinkMessage (state as IPEndPoint, msg);
				}
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
			mSendQueue.Enqueue (msg);

			// Signal send thread
			mSendSignal.Set ();
		}
	}
}
