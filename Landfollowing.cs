using System;
using UnityEngine;

public class Landfollowing:MonoBehaviour
{
	public GameObject go;

	public Landfollowing ()
	{

	}

	void Update ()
	{
		float y = gameObject.transform.localPosition.y;
		float x = go.transform.localPosition.x;
		float z = go.transform.localPosition.z;
		gameObject.transform.localPosition = new Vector3 (x, y, z);
	}
}



