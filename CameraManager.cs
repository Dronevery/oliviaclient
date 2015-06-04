using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
	public Camera initCamera;
	public Dictionary<string, Camera> cameras;

	public Camera currentCamera;

	private GameObject dashboard;
	private GameObject datas;

	// Use this for initialization
	void Start ()
	{
		cameras = new Dictionary<string, Camera> ();	
			
		currentCamera = initCamera;
			
		cameras ["airplane0"] = GameObject.Find ("airplane0").GetComponentInChildren<Camera> ();

		getGUIs ();
	}
	// Update is called once per frame
	void Update ()
	{
			
	}

	public void Activate (Camera ca)
	{
		if (!ca.gameObject.activeSelf) {
			if (ca == initCamera) {
				foreach (KeyValuePair<string, Camera> entry  in cameras) {
					Camera cam = (Camera)entry.Value;
					cam.gameObject.SetActive (false);
				}
				hideGUIs ();
			} else {
				initCamera.gameObject.SetActive (false);
				foreach (KeyValuePair<string, Camera> entry  in cameras) {
					Camera cam = (Camera)entry.Value;
					cam.gameObject.SetActive (false);
				}
				showGUIs ();
			}
			ca.gameObject.SetActive (true);
			currentCamera = ca;
		}
	}

	public void switchCamera (GameObject air)
	{
		try {
			string name = air.name;
			Camera ca = cameras [name];
			Activate (ca);
		} catch (System.Exception ex) {
			Debug.Log (ex);
			return;	
		}
	}

	public void switchCamera (string name)
	{
		if (name == "main") {
			Activate (initCamera);
			return;
		}
		try {
			Camera ca = cameras [name];
			Activate (ca);
		} catch (System.Exception ex) {
			Debug.Log (ex);
		}
	}

	public void getGUIs ()
	{
		this.dashboard = GameObject.Find ("zhuangtai");
		this.datas = GameObject.Find ("Status");
	}

	public void hideGUIs ()
	{
		this.dashboard.SetActive (false);
		this.datas.SetActive (false);
	}

	public void showGUIs ()
	{
		this.dashboard.SetActive (true);
		this.datas.SetActive (true);
	}


}

