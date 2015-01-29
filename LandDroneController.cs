using System;
using UnityEngine;


public class LandDroneController:MonoBehaviour
{
	public float speed = 0.5f;
	public float minH = 0f;
	public  float maxH = 6f;

	public float minX = -4f;
	public float maxX = 5f;

	public float minZ = -5f;
	public float maxZ = 5f;

	public bool isLanded = false;


	void Update ()
	{
		bool oldisLanded = isLanded;
		Vector3 delPos = new Vector3 ();
		if (Input.GetKey (KeyCode.W)) {
			delPos.y += Time.deltaTime * speed;
		}
		if (Input.GetKey (KeyCode.S)) {
			delPos.y -= Time.deltaTime * speed;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			delPos.z += Time.deltaTime * speed;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			delPos.z -= Time.deltaTime * speed;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			delPos.x -= Time.deltaTime * speed;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			delPos.x += Time.deltaTime * speed;
		}
		gameObject.transform.Translate (delPos);

		float localY = gameObject.transform.localPosition.y;
		float localX = gameObject.transform.localPosition.x;
		float localZ = gameObject.transform.localPosition.z;
		localY = Mathf.Clamp (localY, minH, maxH);
		localX = Mathf.Clamp (localX, minX, maxX);
		localZ = Mathf.Clamp (localZ, minZ, maxZ);
		gameObject.transform.localPosition = new Vector3 (localX, localY, localZ);
		if (localY <= minH + 0.001f) {
			isLanded = true;
		}
		if (isLanded == true && oldisLanded == false) {
			showLanded ();
		}


	}

	public LandDroneController ()
	{
	}

	private void showLanded ()
	{
		Debug.Log ("landed successfully");
	}
}


