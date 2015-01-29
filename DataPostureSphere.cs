using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public interface IDataPostureSphere
{
	void UpdateData (string name, Vector3 eulerangle);

	Vector3 getData (string name);

	Vector3 getData ();

	int Count{ get; }
}

public class DataPostureSphere: IDataPostureSphere
{
	//-----------------Variables---------------------
	public Dictionary<string, Quaternion> dataRotates;
	// store the current rotation data(eulerangle) of all posture spheres
	// mapping by name
	public int Count{ get { return dataRotates.Count; } }



	//------------------Methods-----------------------
	void Update ()
	{
//		Debug.Log (dataRotates["test"].roll);
//		Debug.Log (dataRotates.ContainsKey ("test"));
	}


	public DataPostureSphere ()
	{
		this.dataRotates = new Dictionary<string, Quaternion> ();
	}

	public bool Exists (string name)
	{
		// if the plane named "name" exists in the data
		return dataRotates.ContainsKey (name);
	}

	public void AddPlane (string name)
	{
		if (dataRotates.ContainsKey (name)) {
			Debug.Log ("the plane named " + name + " already exists");
			return;
		} else {
			dataRotates.Add (name, new Quaternion ());
		}
	}

	public bool isEmpty ()
	{
		return this.Count == 0 ? true : false;
	}

	public void UpdateData (string name, float pitch, float yaw, float roll)
	{
		if (!dataRotates.ContainsKey (name) || dataRotates [name] == null) {
			Debug.Log ("the plane named " + name + " does not exist");
			this.AddPlane (name);
		}
		var dataRotate = dataRotates [name];
		dataRotate.eulerAngles = new Vector3 (pitch, yaw, roll);
	}

	public void UpdateData (string name, Vector3 eulerAngle)
	{
		if (!dataRotates.ContainsKey (name) || dataRotates [name] == null) {
			Debug.Log ("the plane named " + name + " does not exist");
			this.AddPlane (name);
		}
		var dataRotate = dataRotates [name];
		dataRotate.eulerAngles = eulerAngle;
	}

	public void getData (string name, out float pitch, out float yaw, out float roll)
	{
		if (!dataRotates.ContainsKey (name)) {
			Debug.Log ("getData(string ,out float, out float, out float) : dataRotates does not contain key " + name);
			pitch = 0;
			yaw = 0;
			roll = 0;
			return;		
		}

		Vector3 eulerangle = dataRotates [name].eulerAngles;
		pitch = eulerangle.x;
		yaw = eulerangle.y;
		roll = eulerangle.z;
	}

	public Vector3 getData (string name)
	{
		if (!dataRotates.ContainsKey (name)) {
			Debug.Log ("getData(string ,out float, out float, out float) : dataRotates does not contain key " + name);
			return Vector3.zero;
		}
		
		return dataRotates [name].eulerAngles;
	}

	public Vector3 getData ()
	{
		return new Vector3 (0, 0, 0);
	}

	
}

