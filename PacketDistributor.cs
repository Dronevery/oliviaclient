using System;
using UnityEngine;
using MavLinkNet;
using System.Collections.Generic;
using System.Collections;
using LitJson;
using System.Threading;

namespace AssemblyCSharp
{
	
	public class PacketDistributor: MonoBehaviour
	{
		public event PacketReceivedDelegate threadTrans;

		public MavLinkUdpTransport mMAVLink = new MavLinkUdpTransport ();
		private EventsHandler eHandler;
		private publicvar publicv;
		private Dictionary<string,airobj> airs;
		private AirManager airmanager;
		private long countPkg = 0;

		private SyncEvents pktReceiveEvent = new SyncEvents ();
		private Queue<MavLinkPacket> pktQueue = new Queue<MavLinkPacket> ();

		void Update ()
		{
			lock (((ICollection)pktQueue).SyncRoot) {
				while (pktQueue.Count > 0) {
					this.DistributePacket (null, pktQueue.Dequeue ());
				}
			}
		}

		void Start ()
		{
			/* get some global components of the whole app */
			this.eHandler = GameObject.Find ("Connecter").GetComponent <EventsHandler> ();
			this.publicv = GameObject.Find ("publicvar").GetComponent <publicvar> ();
			this.airs = this.publicv.airs;
			this.airmanager = GameObject.Find ("AirManager").GetComponent<AirManager> ();

			this.mMAVLink.UdpTargetPort = publicvar.MAVLINK_TARGET_PORT;
			this.mMAVLink.UdpListeningPort = publicvar.MAVLINK_LISTENNING_PORT;
			this.mMAVLink.Initialize ();
			this.mMAVLink.OnPacketReceived += this.Oooooooon;
		}

		void OnDestroy ()
		{
			mMAVLink.Dispose ();
		}

		void Oooooooon (object sender, MavLinkPacket pkg)
		{
			if (!pktReceiveEvent.ExitThreadEvent.WaitOne (0, false)) {
				lock (((ICollection)pktQueue).SyncRoot) {
					pktQueue.Enqueue (pkg);
					pktReceiveEvent.NewItemEvent.Set ();
				}
//				Console.print (String.Format ("{0} pkgs waited to be handled", pktQueue.Count));
			}
			
		}

		/// <summary>
		/// Distributes the packet according to its message id.
		/// </summary>
		/// <returns>The packet.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="pkg">Package.</param>
		public void DistributePacket (object sender, MavLinkPacket pkg)
		{
			countPkg++;
//			Console.print ("packet received! " + pkg.MessageId);

			switch (pkg.MessageId) {
			case 0:		// heartbeat
				this.ReceivedHeartbeat (pkg);
				break;

			case 1:		// sys status
				this.ReceivedSysstatus (pkg);
				break;

			case 33:	// *** global position int ***
				this.ReceivedGPSGlobalPosition (pkg);
				break;

			case 30:	// altitude (pitch yaw roll)
				this.ReceivedAttitude (pkg);
				break;

			case 31:	// altitude quaternion
				this.ReceivedAttitudeQuaternion (pkg);
				break;

			case 32: 	// localPosition
				this.ReceivedLocalPositionNed (pkg);
				break;

			case 39:	// *** mission item ***

				break;

			case 66:	// request data stream

				break;

			case 76:	// uas command long

				break;

			default:
				break;
			}
		}

		/// <summary>
		/// send "TAKEOFF" command to the UAV
		/// </summary>
		/// <param name="sys_id_s">(target) system_id ,in String format.</param>
		public void DO_TAKEOFF (string sys_id_s)
		{
			Console.print ("TAKEOFF TRIGGED");
			UasCommandLong msg = (UasCommandLong)UasSummary.CreateFromId (76);
			msg.Command = MavCmd.NavTakeoff;

			mMAVLink.SendMessage (msg);

		}

		/// <summary>
		/// send "RETURN TO LAUNCH POINT" command to the UAV
		/// </summary>
		/// <param name="sys_id_s">(target) system_id ,in String format.</param>
		public void DO_RETURN_TO_LAUNCH (string sys_id_s)
		{
			Console.print ("RETURN_TO_LAUNCH TRIGGED");
			UasCommandLong msg = (UasCommandLong)UasSummary.CreateFromId (76);
			msg.Command = MavCmd.NavReturnToLaunch;

			mMAVLink.SendMessage (msg);
		}

		/// <summary>
		/// send 'land' command to the vehicle
		/// </summary>
		/// <param name="sys_id_s">Sys identifier s.</param>
		public void DO_LAND (string sys_id_s)
		{
			Console.print ("LAND TRIGGED");
			UasCommandLong msg = (UasCommandLong)UasSummary.CreateFromId (76);
			msg.Command = MavCmd.NavLand;

			mMAVLink.SendMessage (msg);
		}


		//----- impl -----------

		/// <summary>
		/// Update connection situation. make sure the UAV is online.
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void  ReceivedHeartbeat (MavLinkPacket pkg)
		{
			Console.print ("heartbeat received");
			return;
//			UasHeartbeat msg = (UasHeartbeat)pkg.Message;
			string sys_id_s = pkg.SystemId.ToString ();

			// if UAV not registered, register it
			if (!this.airs.Keys.ContainsItem (sys_id_s)) {
				JsonData airStatusJson = new JsonData ();
				airStatusJson ["lon"] = 0;
				airStatusJson ["lat"] = 0;
				airStatusJson ["height"] = 0;
				airStatusJson ["pitch"] = 0;
				airStatusJson ["yaw"] = 0;
				airStatusJson ["roll"] = 0;

				JsonData data = new JsonData ();
				data [sys_id_s] = airStatusJson;
				airmanager.UpdateOrCreate (data);
			}

			// update connection status
			// TODO			
		}

		/// <summary>
		/// Update the status of UAV stored in GCS
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void ReceivedSysstatus (MavLinkPacket pkg)
		{
			// TODO
		}

		/// <summary>
		/// update the spacial coordinates of the air base on GPS data(latitude, longitude, altitude)
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void ReceivedGPSGlobalPosition (MavLinkPacket pkg)
		{			
			
			string sys_id_s = pkg.SystemId.ToString ();
			print ("name" + sys_id_s);
			UasGlobalPositionInt msg = (UasGlobalPositionInt)pkg.Message;
			float lat = (float)msg.Lat / 10000000; // Unit: degree
			float lon = (float)msg.Lon / 10000000; // Unit: degree
			float altitude = 100 + (float)msg.Alt / 1000; //Unit: meter

			JsonData airStatusJson = new JsonData ();
			airStatusJson ["lon"] = lon;
			airStatusJson ["lat"] = lat;
			airStatusJson ["height"] = altitude;
//			return;
			JsonData data = new JsonData ();
			data [sys_id_s] = airStatusJson;
//			Console.print ("lon    " + airStatusJson ["lon"]);
//			return;
			this.airmanager.UpdateOrCreate (data);
//			Console.print ("air status updated!");
		}

		/// <summary>
		/// Update the attitude of the model based on euler angle
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void ReceivedAttitude (MavLinkPacket pkg)
		{
			return;
			string sys_id_s = pkg.SystemId.ToString ();
			UasAttitude msg = (UasAttitude)pkg.Message;
			float pitch = msg.Pitch * 180 / Mathf.PI;
			float roll = msg.Roll * 180 / Mathf.PI;
			float yaw = msg.Yaw * 180 / Mathf.PI;

			JsonData airStatusJson = new JsonData ();
			airStatusJson ["pitch"] = pitch;
			airStatusJson ["yaw"] = yaw;
			airStatusJson ["roll"] = roll;
			JsonData data = new JsonData ();
			data [sys_id_s] = airStatusJson;
			this.airmanager.UpdateOrCreate (data);

		}

		/// <summary>
		/// Update the attitude of the model base on quaternion
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void ReceivedAttitudeQuaternion (MavLinkPacket pkg)
		{
			string sys_id_s = pkg.SystemId.ToString ();
			UasAttitudeQuaternion msg = (UasAttitudeQuaternion)pkg.Message;
			Quaternion rotation = new Quaternion (msg.Q2, msg.Q3, msg.Q4, msg.Q1);//Q1:w 
//			Console.print (String.Format ("{0},{1},{2},{3}", msg.Q2, msg.Q3, msg.Q4, msg.Q1));
			if (this.airs.Keys.ContainsItem (sys_id_s.ToString ())) {
				this.airs [sys_id_s.ToString ()].go.gameObject.transform.rotation = rotation;
			}
		}

		/// <summary>
		/// Receiveds the local position ned.
		/// TODO
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void ReceivedLocalPositionNed (MavLinkPacket pkg)
		{
			return;
			UasLocalPositionNed msg = (UasLocalPositionNed)pkg.Message;
			Console.print ("local postion data received! POSITION:");
			Console.print (String.Format ("{0},{1},{2}", msg.X, msg.Y, msg.Z));
			Console.print ("VELOCITY:");
			Console.print (String.Format ("{0},{1},{2}\n", msg.Vx, msg.Vy, msg.Vz));


		}
	}
}

