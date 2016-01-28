using UnityEngine;
using System.Collections;

public class PinTo : MonoBehaviour {

	[SerializeField] public Transform other;
	private Vector3 relivivePosition;
	private Quaternion relativeRotation;

	public void Setup (Transform newPerant) {
		other = newPerant;
		relivivePosition = Quaternion.Inverse (other.rotation) * (transform.position - other.position);
		relativeRotation = Quaternion.Inverse (other.rotation) * transform.rotation;
	}
	
	void LateUpdate () {
		transform.position = other.position + other.rotation * relivivePosition;
		transform.rotation = other.rotation * relativeRotation;
	}
}
