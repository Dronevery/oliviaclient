using System;
using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public interface IUILand
{
	Quaternion dataSourceRotation{ get; set; }

	Location dataSourcePosition{ get; set; }

	void Initialize ();
	// set the position of the scene
	//

	void UpdateScene ();
	// update the position&posture of the drone

	void Finalize ();
	// destroy the scene
	// give camera control to main camera
}



public class UILand :MonoBehaviour, IUILand
{
	// Attached to the scene
	public Quaternion dataSourceRotation{ get; set; }

	public Location dataSourcePosition{ get; set; }

	private GameObject _scene;
	private GameObject _drone;
	private GameObject _camera;
	private bool isUpdating = false;

	void Awake ()
	{

	}

	void Update ()
	{
		
	}

	public void Initialize (Vector3 pos, Quaternion datasource)
	{
		print ("a new landing scene is created!");
		gameObject.transform.position = pos;
		setSceneMembers (_scene, out this._drone, out this._camera);
		this.dataSourceRotation = dataSourceRotation;
	}

	public void UpdateScene ()
	{
		Vector3 shift = GetShift ();
		Vector3 euler = GetEuler ();
	}

	public void Finalize ()
	{
		print ("the landing scene will be destroyed!");
		this._camera.SetActive (false);
		Destroy (_scene);
	}


	private void setSceneMembers (GameObject scene, out GameObject drone, out GameObject camera)
	{
		scene = gameObject;
		scene.SetActive (true);
		scene.SetChildrenActiveRecursively (true);
		drone = scene.transform.FindChild ("drone").gameObject;	
		camera = scene.transform.FindChild ("Camera").gameObject;
		camera.SetActive (true);
		this.isUpdating = true;
	}

	private Vector3 GetShift ()
	{
		return new Vector3 (0, 0, 0);
	}

	private Vector3 GetEuler ()
	{
		return dataSourceRotation.eulerAngles;
	}

	public void Initialize ()
	{
	}

		 

}


