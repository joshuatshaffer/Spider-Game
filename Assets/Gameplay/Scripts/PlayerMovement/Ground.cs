using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlayerMovement {
	public class Ground {

		private Feet feet;
		private Transform transform;

		public Vector3 centroid { get; private set; }
		public Vector3 normal { get; private set; }
		public Vector3 velocity { get; private set; }
		public Vector3 angularVelocity { get; private set; }
		public bool isGrounded = false, hasTraction = false;
		private int numberOfHits;

		public void Init (Feet f, Transform t) {
			feet = f;
			transform = t;
		}

		public void Update () {
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

				//TODO body.ChangeGroundbody();
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
					velocity += hit.rigidbody.GetPointVelocity (transform.position);
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
}