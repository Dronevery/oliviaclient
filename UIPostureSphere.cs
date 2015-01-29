using UnityEngine;
using System.Collections;

public class UIPostureSphere : MonoBehaviour
{
	public DataPostureSphere data;
	private float pitch=0;
	private float yaw=0;
	private float roll=0;
	public string airname;
	
		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
			updateRotation (airname);
		}
	
	private void updateRotation(string name){
		data.getData (name, out pitch, out yaw, out roll);
		Debug.Log (roll);
		Rotate (pitch, yaw, roll);
	}

	public void Rotate(float pitch, float yaw, float roll){
		gameObject.transform.rotation = Quaternion.Euler(pitch, yaw, roll);
	}

}

