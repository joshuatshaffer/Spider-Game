using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public float penetrationVariation = 0.2f;
	private Vector3 lastPosition;
	private Rigidbody body;

	void Start () {
		Destroy (gameObject, 20);

		lastPosition = transform.position;
		body = GetComponent<Rigidbody> ();

		//Nun'in like random variation to make things look real for cheap.
		Vector3 extraPenetration = Vector3.forward * Random.Range(0f, penetrationVariation);
		Quaternion rotationVariation = Quaternion.Euler(Random.Range(-180f,108f),0,0);
		foreach (Transform child in transform) {
			child.localPosition += extraPenetration;
			child.localRotation *= rotationVariation;
		}
		if (body.velocity != Vector3.zero)
			transform.rotation = Quaternion.LookRotation (body.velocity);
	}

	void FixedUpdate () {
		RaycastHit hitInfo;
		if (Physics.Linecast (lastPosition, transform.position, out hitInfo)) {
			//Stick to the hit object
			transform.position = hitInfo.point;
			gameObject.AddComponent<PinTo> ().Setup(hitInfo.transform);

			//Simulate a perfictly inelastic collistion
			if (hitInfo.rigidbody != null) {
				float ma = body.mass;
				Vector3 ua = body.velocity;
				float mb = hitInfo.rigidbody.mass;
				Vector3 ub = hitInfo.rigidbody.velocity;
				Vector3 deltaV = (ma*ua + mb*ub) / (ma + mb) - ub;

				hitInfo.rigidbody.AddForceAtPosition(deltaV, hitInfo.point, ForceMode.VelocityChange);
			}

			//this and the rigidbody are done now
			Destroy (this);
			Destroy (body);
		} else {
			if (body.velocity != Vector3.zero)
				transform.rotation = Quaternion.LookRotation (body.velocity);
			lastPosition = transform.position;
		}
	}
}
