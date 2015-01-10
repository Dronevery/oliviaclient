using UnityEngine;
using System.Collections;

public class PostureSphere : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

	public void Rotate(float roll, float yaw, float pitch){
		gameObject.transform.rotation.eulerAngles = new Vector3 (pitch, yaw, roll);
	}

}

