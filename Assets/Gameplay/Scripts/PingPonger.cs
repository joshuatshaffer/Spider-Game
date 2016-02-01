using UnityEngine;
using System.Collections;

public class PingPonger : MonoBehaviour {
	private Rigidbody body;

	public Transform start;
	public Transform stop;

	void Start () {
		body = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		float t = Mathf.Sin(Time.fixedTime*0.1f*6) * 0.5f + 0.5f;
		body.MovePosition(Vector3.Lerp(start.position, stop.position, t));
		body.MoveRotation(Quaternion.Lerp(start.rotation, stop.rotation, t));
	}
}
