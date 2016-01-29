using UnityEngine;
using System.Collections;

public class PinTo : MonoBehaviour {

	public Transform pseudoParent;
	private Vector3 relivivePosition;
	private Quaternion relativeRotation;

	public void Setup (Transform newParent) {
		pseudoParent = newParent;
		relivivePosition = Quaternion.Inverse (pseudoParent.rotation) * (transform.position - pseudoParent.position);
		relativeRotation = Quaternion.Inverse (pseudoParent.rotation) * transform.rotation;
	}
	
	void LateUpdate () {
		transform.position = pseudoParent.position + pseudoParent.rotation * relivivePosition;
		transform.rotation = pseudoParent.rotation * relativeRotation;
	}
}
