using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using LitJson;
using System;
using System.Text;
using System.Threading;

public class airobj 
{
    // C# 3.0 auto-implemented properties
	public float lon {get; set;}
	public float lat {get; set;}
	public float height {get; set;}
	public float yaw {get;set;}
	public float pitch {get;set;}
	public float roll {get;set;}
	public float[] loc ;
	public GameObject go;
	public string name;
	public TerrainManager terrainmanager;

	private bool isInited = false;

	private GameObject pitchimage;
	private GameObject yawobj;
	private GameObject rollobj;
	private Text lontext;
	private Text lattext;
	private Text veltext;
	private Text htext;
	private CameraManager cm;

	public void update(JsonData jd)
	{
		updateData (jd);

		if (!isInited){
			isInited = true;
			int[] center = maplib.lnglatToXY (lon, lat, publicvar.basezoom);
			publicvar.basei = center [0];
			publicvar.basej = center [1];
		}

		loc = maplib.getUnityPosfromLatlng (lon, lat, publicvar.basezoom);

		go.transform.position = new Vector3(loc[0], height,loc[2]);
		go.transform.Rotate(new Vector3 (0, 0, 0));
		if (this.go.GetComponentInChildren<Camera>() == cm.currentCamera) {	
			updateGUIComponents ();
				}

		drawLine ();
//		Debug.DrawLine (this.go.transform.position, new Vector3 (this.go.transform.position.x, 0, this.go.transform.position.z), Color.red);
	}

	public airobj(string name ,GameObject originairplane ,JsonData jd)
	{
		getGUIComponents ();
		cm = GameObject.Find ("Connecter").GetComponent<CameraManager> ();
		// create a plane on the stage
    	go = (GameObject)MonoBehaviour.Instantiate(originairplane);
		go.SetActive (true);
		go.name = name;

		// inactivate the camera
		Camera camera = go.GetComponentInChildren<Camera> ();
		camera.gameObject.SetActive(false);

		// register to the camera manager
		CameraManager cmngr = GameObject.Find ("Connecter").GetComponent<CameraManager> ();
		cmngr.cameras [name] = camera;

		update(jd);
		this.go.AddComponent("TerrainManager");
		TerrainManager terrainmanager = this.go.GetComponent<TerrainManager>();
		terrainmanager.plane = go;
		terrainmanager.StartUpdate ();
	}

	public void updateData(JsonData jd){
		lon = (float) ( (double) jd["lon"]);
		lat = (float) ( (double) jd["lat"]);
		height = (float) ( (double) jd["height"]);
		yaw = (float) ( (double) jd["yaw"]);
		pitch = (float) ( (double) jd["pitch"]);
		roll = (float) ( (double) jd["roll"]);
		//		Debug.Log ("yaw " + this.yaw);
		//		Debug.Log ("roll " + this.roll);
		//		Debug.Log ("pitch " + this.pitch);
	}

	public void getGUIComponents(){
		this.pitchimage = GameObject.Find("pitchimage");
		this.yawobj = GameObject.Find ("yaw");
		this.rollobj = GameObject.Find ("roll");
		this.lontext = GameObject.Find ("Lon").GetComponent<Text>();
		this.lattext = GameObject.Find ("Lat").GetComponent<Text>();
		this.veltext = GameObject.Find ("Velocity").GetComponent<Text>();
		this.htext = GameObject.Find ("Heightobj").GetComponent<Text>();
	}

	public void updateGUIComponents(){
		this.pitchimage.transform.localPosition = new Vector3 (0, this.pitch*2, 0);
		this.rollobj.transform.rotation = Quaternion.Euler(0,0,this.roll);
		this.yawobj.transform.rotation  = Quaternion.Euler(0,0,this.yaw);
		this.lontext.text = this.lon.ToString ();
		this.lattext.text = this.lat.ToString ();
//		this.velobj.guiText.text = this.go.transform.;
		this.htext.text = this.go.transform.position.y.ToString ();	
	}

	public void drawLine(){
		LineRenderer line = this.go.GetComponentInChildren<LineRenderer> ();
		Vector3 ground = new Vector3 (this.go.transform.position.x, 0, this.go.transform.position.z);
		line.SetPosition (0, this.go.transform.position - new Vector3(0,20,0));
		line.SetPosition (1, ground);
	}

	public void destroy(){
		GameObject.Destroy(go);
	}
}
