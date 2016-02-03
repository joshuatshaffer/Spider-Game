using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlayerMovement {
	public class Ground {

		public Vector3 centroid;
		public Vector3 normal;
		public bool isGrounded = false, hasTraction = false;
		private int numberOfHits;
		private FrequencyCounter fc;

		[System.NonSerialized] private Psudobody body;
		[System.NonSerialized] private Feet feet;

		public void Init (Psudobody b, Feet f) {
			body = b;
			feet = f;
		}

		public void Update () {
			numberOfHits = 0;
			centroid = normal = Vector3.zero;
			hasTraction = false;
			fc = new FrequencyCounter();

			feet.Casts ();

			if (numberOfHits > 0) {
				centroid /= numberOfHits;
				normal = normal.normalized;
				isGrounded = true;
			} else {
				isGrounded = false;
			}

			if (hasTraction) {
				body.ChangeGroundbody(fc.Mode());
			} else {
				body.ChangeGroundbody(null);
			}

			centroid = body.RelevisePosition(centroid);
			normal = body.ReleviseDirection(normal);
		}

		public void ProcessHit (RaycastHit hit) {
			++numberOfHits;
			centroid += hit.point;
			normal += hit.normal;
			if (!IsSlippery(hit.collider.material)) {
				hasTraction = true;
				fc.Occured(hit.rigidbody);
			}
		}

		private bool IsSlippery (PhysicMaterial mat) {
			return mat.dynamicFriction < 0.01 &&
				mat.staticFriction < 0.01 &&
				mat.frictionCombine == PhysicMaterialCombine.Minimum;
		}
	}
}