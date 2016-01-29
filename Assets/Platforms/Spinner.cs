using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {
	
	public float speed = 40;
	public Vector3 axis = Vector3.up;

	private Rigidbody body;

	void Start () {
		body = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		Debug.Log (body.angularVelocity);
		body.MoveRotation (Quaternion.AngleAxis(Time.fixedTime*speed, axis));
	}
}
