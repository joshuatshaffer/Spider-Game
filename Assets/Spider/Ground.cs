using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ground {

	private Feet feet;
	private Rigidbody body;

	public Vector3 centroid { get; private set; }
	public Vector3 normal { get; private set; }
	public Vector3 velocity { get; private set; }
	public Vector3 angularVelocity { get; private set; }
	public bool isGrounded = false, hasTraction = false;
	private int numberOfHits;

	public void Init (Rigidbody b, Feet f) {
		body = b;
		feet = f;
	}

	public void Update () {
		Vector3 lastVelocity = velocity;
		Vector3 lastAngularVelocity = angularVelocity;

		numberOfHits = 0;
		velocity = angularVelocity = centroid = normal = Vector3.zero;
		hasTraction = false;

		feet.Casts ();

		if (numberOfHits > 0) {
			velocity /= numberOfHits;
			angularVelocity /= numberOfHits;
			centroid /= numberOfHits;
			normal = normal.normalized;
			isGrounded = true;

			body.AddTorque (angularVelocity - lastAngularVelocity, ForceMode.VelocityChange);
			body.AddForce (velocity - lastVelocity, ForceMode.VelocityChange);
		} else {
			velocity = angularVelocity = Vector3.zero;
			isGrounded = false;
		}
	}

	public void ProcessHit (RaycastHit hit) {
		++numberOfHits;
		centroid += hit.point;
		normal += hit.normal;

		if (!IsSlippery(hit.collider.material)) {
			hasTraction = true;
			if (hit.rigidbody != null) {
				velocity += hit.rigidbody.GetPointVelocity (body.position);
				angularVelocity += hit.rigidbody.angularVelocity;
			}
		}
	}

	private bool IsSlippery (PhysicMaterial mat) {
		return mat.dynamicFriction < 0.01 &&
			mat.staticFriction < 0.01 &&
			mat.frictionCombine == PhysicMaterialCombine.Minimum;
	}

}
