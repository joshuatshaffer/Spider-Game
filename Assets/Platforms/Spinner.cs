using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {
	
	public float speed = 40; //Degrees Per Second
	public Vector3 axis = Vector3.up;

	private Quaternion originalRotation;
	private Rigidbody body;

	void Awake () {
		body = GetComponent<Rigidbody> ();
		originalRotation = body.rotation;
	}

	void FixedUpdate () {
		body.MoveRotation (originalRotation * Quaternion.AngleAxis(Time.fixedTime*speed, axis));
	}
}
