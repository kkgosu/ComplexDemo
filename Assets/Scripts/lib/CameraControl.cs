using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public float positonSpeed = 0.5f;
	public float rotationSpeed = 2.5f;
	public float wheelSpeed = 1.0f;

	void LateUpdate () {
		float x = Input.GetAxis ("Mouse X");
		float y = Input.GetAxis ("Mouse Y");

		if ((Input.GetMouseButton (2)) || (Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)))) {
			transform.Translate(Vector3.right * -x * positonSpeed);
			transform.Translate(transform.up * -y * positonSpeed, Space.World);
		}
		else if (Input.GetMouseButton(1) )
		{
			transform.Rotate(Vector3.up, x * rotationSpeed,Space.World);
			transform.Rotate(Vector3.right, -y * rotationSpeed, Space.Self);
		}
		float currentDistance = Input.GetAxis ("Mouse ScrollWheel") * wheelSpeed; 
		transform.position = transform.position + (transform.rotation * Vector3.forward * currentDistance);
	}
		
}
