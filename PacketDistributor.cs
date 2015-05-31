using System;
using UnityEngine;
using MavLinkNet;
using System.Collections.Generic;
using LitJson;

namespace AssemblyCSharp
{
	public class PacketDistributor: MonoBehaviour
	{
		public MavLinkUdpTransport mMAVLink;
		private EventsHandler eHandler;
		private publicvar publicv;
		private Dictionary<string,airobj> airs;
		private AirManager airmanager;
		private long countPkg = 0;

		void Update ()
		{

		}

		void Start ()
		{
			/* get some global components of the whole app */
			this.eHandler = GameObject.Find ("Connecter").GetComponent <EventsHandler> ();
			this.publicv = GameObject.Find ("publicvar").GetComponent <publicvar> ();
			this.airs = this.publicv.airs;
			this.airmanager = GameObject.Find ("AirManager").GetComponent<AirManager> ();

			
			mMAVLink = new MavLinkUdpTransport ();
			mMAVLink.Initialize ();
			mMAVLink.OnPacketReceived += DistributePacket;
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
			Debug.Log ("packet received! " + pkg.Message);
			Debug.Log ("numbers of packets received: " + countPkg.ToString ());

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
		/// send "Arm" command to the UAV
		/// </summary>
		/// <param name="sys_id_s">(target) system_id ,in String format.</param>
		public void _DoArm (string sys_id_s)
		{
			
		}

		/// <summary>
		/// send "DisArm" command to the UAV
		/// </summary>
		/// <param name="sys_id_s">(target) system_id ,in String format.</param>
		public void _DoDisArm (string sys_id_s)
		{
			
		}


		//----- impl -----------

		/// <summary>
		/// Update connection situation. make sure the UAV is online.
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void  ReceivedHeartbeat (MavLinkPacket pkg)
		{
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
		/// update the spacial coordinates of the air base on GPS data(latitude,longitude,altitude)
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void ReceivedGPSGlobalPosition (MavLinkPacket pkg)
		{
			string sys_id_s = pkg.SystemId.ToString ();
			UasGlobalPositionInt msg = (UasGlobalPositionInt)pkg.Message;
			float lat = (float)msg.Lat / 10000000; // Unit: degree
			float lon = (float)msg.Lon / 10000000; // Unit: degree
			float altitude = (float)msg.Alt / 1000000; //Unit: meter

			JsonData airStatusJson = new JsonData ();
			airStatusJson ["lon"] = lon;
			airStatusJson ["lat"] = lat;
			airStatusJson ["height"] = altitude;
			JsonData data = new JsonData ();
			data [sys_id_s] = airStatusJson;
			this.airmanager.UpdateOrCreate (data);			
		}

		/// <summary>
		/// Update the attitude of the model based on euler angle
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void ReceivedAttitude (MavLinkPacket pkg)
		{
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
			this.airs [sys_id_s].go.gameObject.transform.rotation = rotation;
		}
	}
}

