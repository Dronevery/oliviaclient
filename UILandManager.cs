using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;


public class UILandManager : MonoBehaviour
{
	public static UILandManager SP;

	public GameObject scene;
	// the scene object is a plane(maybe mesh?) with a drone and a camera as its childs

	public Dictionary<string, GameObject> scenes = new Dictionary<string, GameObject> ();
	public Dictionary<string,Quaternion> datasRotation = new Dictionary<string, Quaternion> ();
	public Dictionary<string,Location> datasPosition = new Dictionary<string, Location> ();

	// persistent structure to retrieve data from
	//
	//	private GameObject drone;
	//	private GameObject plane;
	//	private GameObject camera;
	//	// temp names, just for convenience

	void Awake ()
	{// initialize
		UILandManager.SP = this;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void CreateLandScene (string name)
	{
		var s = Instantiate (scene, new Vector3 (10000, -2000, 0), new Quaternion ()) as GameObject;
		scenes.Add (name, s);
		datasPosition.Add (name, new Location (name, 117, 41, 10));
		datasRotation.Add (name, Quaternion.Euler (0, 0, 0));
		var sui = s.GetComponent <UILand> ();
		sui.dataSourceRotation = datasRotation [name];
		sui.dataSourcePosition = datasPosition [name];
	}

	public  void DestroyLandScene (string name)
	{
		scenes [name].GetComponent <UILand> ().Finalize ();
	}

	//	public void StartLanding ()
	//	{
	//		// Initiate a plane and a drone
	//		Debug.Log ("starting to land....");
	//		plane = Instantiate (scene, publicvar.landPosition, Quaternion.identity) as GameObject;
	//		setSceneMembers (plane, out this.drone, out this.camera);
	//	}
	//
	//
	//	public void UpdateScene ()
	//	{
	//		// update the postition of the drone
	//		Vector3 shift = getShift ();
	//		UpdatePostionby (shift);
	//	}
	//
	//	public void FinishLanding ()
	//	{
	//		// Destroy all the objects
	//		Debug.Log ("land mission finished");
	//		this.isUpdating = false;
	//		camera.SetActive (false);
	//		Destroy (plane);
	//		this.scene = null;
	//	}
	//
	//	private void UpdatePostionby (Vector3 shift)
	//	{
	//		drone.transform.position = plane.transform.position + shift;
	//	}
	//
	//	public Vector3 getShift ()
	//	{
	//		return new Vector3 (3, 3, 3);
	//	}
	//
	//	public Vector3 getShift (Location loc)
	//	{
	//		float lon = loc.longitude;
	//		float lat = loc.latitude;
	//		float height = loc.height;
	//
	//		return new Vector3 (3, 3, 3);
	//
	//		//TODO
	//	}
	//
	//
	//	private void setSceneMembers (GameObject plane, out GameObject drone, out GameObject camera)
	//	{
	//		plane.SetActive (true);
	//		plane.SetChildrenActiveRecursively (true);
	//		drone = plane.transform.FindChild ("drone").gameObject;
	//		camera = plane.transform.FindChild ("Camera").gameObject;
	//		camera.SetActive (true);
	//		this.isUpdating = true;
	//	}
}

