using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using LitJson;
using System;
using System.Text;
using System.Threading;
using MavLinkNet;

public class StateObject
{

	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 256;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder ();
}

public static class MavSettings
{
	public static MavType sys_type = MavType.Generic;
	// the type of the system

}

public class connect: MonoBehaviour
{

	// Use this for initialization
	public Socket clientSocket;
	public static Dictionary<string,airobj> aircluster = new Dictionary<string,airobj> ();
	public GameObject originairplane;
	public JsonData temp_data;
	private auth Auth;

	public IDataManager _datamgr;

	/* events api 
	*/
	// define delegate and events
	// delegated
	public delegate void DataHandler (JsonData data);
	// define events here
	public static event DataHandler connected;
	public static event DataHandler report;
	public static event DataHandler receiveGamedata;


	// trigger the events here
	public static void OnConnected (JsonData data)
	{
		if (connected != null) {
			connected (data);
		}
	}

	public static void OnReport (JsonData data)
	{
		if (report != null) {
			report (data);
		}
	}


	public static void OnReceiveGamedata (JsonData data)
	{
		if (receiveGamedata != null) {
			receiveGamedata (data);
		}
	}

	/* events api 
	end */
	private delegate void some_delegate (JsonData data);

	private static Dictionary<string, some_delegate> m_map = new Dictionary<string, some_delegate> () {
		{ "gamedata", OnReceiveGamedata }
	};
	//new Dictionary<string, some_delegate>();
	//m_map.Add("gamedata",OnReceiveGamedata);
	//	eventmap["gamedata"] = OnReceiveGamedata;

	public void HandleEvent (JsonData origindata)
	{
		String eventname = "";
		JsonData data = null;
		try {
			eventname = (String)origindata ["event"];
		} catch {
			Debug.Log ("key 'event' should be included ");
			Debug.Log (origindata.ToJson ());
		}
		try {
			data = (JsonData)origindata ["data"];
		} catch {
			Debug.Log ("key 'data'should be included");	
		}

		if (m_map.ContainsKey (eventname)) {
			m_map [eventname] (data);
		}
	}



	// methods to connect
	public void send (string sendMessage)
	{
		sendMessage = sendMessage + "$";
		clientSocket.Send (Encoding.ASCII.GetBytes (sendMessage));
	}

	//	private static ManualResetEvent receiveDone =  new ManualResetEvent(false);

	void test ()
	{
		GameObject su27 = (GameObject)MonoBehaviour.Instantiate (originairplane);
		su27.transform.position = new Vector3 (0, 0, 0);
	}


	void Start ()
	{
		//test();
		Auth = new auth (this);
		//game0gui.gamelog("connecting to the server "+publicvar.ip_addr+"...");
		IPAddress ip = IPAddress.Parse (publicvar.ip_addr);
		clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		try {
			clientSocket.Connect (new IPEndPoint (ip, 1026)); //config the ip $ port of the server
			StateObject state = new StateObject ();
			state.workSocket = clientSocket;

			// Begin receiving the data from the remote device.
			clientSocket.BeginReceive (state.buffer, 0, StateObject.BufferSize, 0,
				new AsyncCallback (ReceiveCallback), state);

		} catch {
			Debug.Log ("Falied to connect to the server, connecting....");
		}

	}


	public static string buffer = "";

	private void proc_res (string resp)
	{
		if (resp [resp.Length - 1] == '$') {
			buffer += resp;
			string response = buffer.Substring (0, buffer.Length - 1);
			proc_mes (response);
			buffer = "";
		} else
			buffer += resp;
	}

	private void proc_mes (string resp)
	{
		if (!auth.online) {
			Auth.wait (resp);
		} else {
			try {
				JsonData origindata = JsonMapper.ToObject (resp);
				temp_data = origindata;
			} catch (Exception e) {
				print ("JSON Parse Failed\n" + e.ToString ());
			}
		}

	}

	private void ReceiveCallback (IAsyncResult ar)
	{
		try {
			// Retrieve the state object and the client socket
			// from the asynchronous state object.
			StateObject state = (StateObject)ar.AsyncState;
			Socket client = state.workSocket;
			// Read data from the remote device.
			int bytesRead = client.EndReceive (ar);
			if (bytesRead > 0) {
				// There might be more data, so store the data received so far.
				state.sb.Append (Encoding.ASCII.GetString (state.buffer, 0, bytesRead));
				// Get the rest of the data.
				bytesRead = 0;
				string data = state.sb.ToString ();
				state.sb = new StringBuilder ();

				client.BeginReceive (state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback (ReceiveCallback), state);

				proc_res (data);
			}
		} catch (Exception e) {
			System.Console.WriteLine (e.ToString ());
		}
	}
	// Update is called once per frame
	//
	private int count = 0;

	void Update ()
	{

		if (!auth.online) {
			count++;
		}
		if (count > 30) {
			count = 0;
			Start ();
		}

		if (temp_data != null)
			HandleEvent (temp_data);
	}
}
