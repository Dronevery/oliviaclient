using UnityEngine;
using System.Collections;

public class FPSFlight : MonoBehaviour
{
	
	public float moveSpeed = 3.0f;
	public float rotationSpeedX = 1;
	public float rotationSpeedY = 1;
	public float maxSpeed = 10.0f;
	public Transform shipModel;
	public float actualSpeed = 0.0f;
	//因为要用场景中的camera来做位置限制
	public Camera camera;
	
	private Vector3 moveDirection = Vector3.zero;
	private float baseRotationX = 0.0f;
	private float baseRotationY = 0.0f;
	private float oldSpeed = 0.0f;
	
	private float recToX = 0f;

	void Start ()
	{
		
	}

	void FixedUpdate ()
	{
		//控制机身旋转;
		baseRotationX = Input.GetAxis ("Horizontal") * rotationSpeedX;
		baseRotationY = -Input.GetAxis ("Vertical") * rotationSpeedY;
		float angleZ = baseRotationX * 30;
		float angleX = baseRotationY * 15;
		float force = (Input.GetKey (KeyCode.Space)) ? 1 : 0;
		shipModel.localEulerAngles = new Vector3 (angleX, 0, angleZ);
		
		float toX = -Input.GetAxis ("Horizontal") * moveSpeed;
		float toZ = -Input.GetAxis ("Vertical") * moveSpeed;

		//限制;
		Vector3 screenPos = camera.WorldToScreenPoint (shipModel.position);
		if (Input.GetAxis ("Horizontal") < 0) {
			//向左移动;
			if (screenPos.x <= 50) {
				toX = 0;
			}
		}
		
		if (Input.GetAxis ("Horizontal") > 0) {
			//向右移动;
			if (screenPos.x >= Screen.width - 50) {
				toX = 0;
			}
		}
		
		if (Input.GetAxis ("Vertical") < 0) {
			//向下移动;
			if (screenPos.y <= 0) {
				toZ = 0;
			}
		}
		
		if (Input.GetAxis ("Vertical") > 0) {
			//向上移动;
			if (screenPos.y >= Screen.height - 110) {
				toZ = 0;
			}
		}
		
		//移动机身;
		moveDirection.Set (toX, 0, toZ);
		this.transform.position += moveDirection * Time.deltaTime;
//		this.rigidbody.AddForce (transform.forward * force * 100);
	}
	
}