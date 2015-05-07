using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.Globalization;

public class EventsHandler: MonoBehaviour
{
	// THIS IS THE CORE OF THE ENTIRE GAME

	private Dictionary<string, airobj> airs;
	private AirManager airmanager;
	private publicvar publicv;
	private HeightmapLoader heightmaploader;
	public GameObject originairplane;

	void Start ()
	{
		// 1. register the events	

		connect.connected += this.OnConnected;
		connect.report += this.OnReport;
		connect.receiveGamedata += this.OnReceiveGamedata;

		// 2. Initialize Global varibles
		try {
			publicv = GameObject.Find ("publicvar").GetComponent<publicvar> ();
		} catch (Exception ex) {
			print ("Gameobject named publicvar not found! \n  " + ex.Message);
		}

		if (publicv.originairplane1 == null) {
			publicv.originairplane1 = this.originairplane;	
		}

		int i = maplib.lnglatToXY (publicvar.longitude, publicvar.latitude, publicvar.basezoom) [0];
		int j = maplib.lnglatToXY (publicvar.longitude, publicvar.latitude, publicvar.basezoom) [1];
		publicvar.basei = i;
		publicvar.basej = j;
		Debug.Log ("basei ,basej: " + i + " " + j);

		// 3. Initialize Air manager
//		airmanager = gameObject.AddComponent<AirManager> ();
		airmanager = GameObject.Find ("AirManager").GetComponent<AirManager> ();
		airmanager.originair = this.originairplane;
		airmanager.airs = publicv.airs;

		// 4. Initialize HeightmapLoader
//		heightsloader = gameObject.AddComponent<HeightmapLoader> ();
		heightmaploader = GameObject.Find ("HeightmapLoader").GetComponent<HeightmapLoader> ();

		StartCoroutine (Startloadheightmap ());

	}

	IEnumerator Startloadheightmap ()
	{
		yield return new WaitForSeconds (6);
		heightmaploader.Startload ();
		yield break;
	}

	void OnEnable ()
	{
		// register the events	
		connect.connected += this.OnConnected;
		connect.report += this.OnReport;
		connect.receiveGamedata += this.OnReceiveGamedata;
	}

	#region EventsHandler

	void OnConnected (JsonData data)
	{

	}

	void OnReport (JsonData data)
	{
		
	}

	void OnReceiveGamedata (JsonData jd)
	{
	
		airmanager.UpdateOrCreate (jd);
		

	}

	#endregion

	void OnDisable ()
	{
		//unregister the events
		connect.connected -= this.OnConnected;
		connect.report -= this.OnReport;
		connect.receiveGamedata -= this.OnReceiveGamedata;
	}

	void OnDestroy ()
	{
		//unregister the events
		connect.connected -= this.OnConnected;
		connect.report -= this.OnReport;	
		connect.receiveGamedata -= this.OnReceiveGamedata;
	}

}