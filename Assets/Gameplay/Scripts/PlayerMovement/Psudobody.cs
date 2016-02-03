using UnityEngine;
using System.Collections;

namespace PlayerMovement {
	[System.Serializable]
	public class Psudobody {
		
		public bool useGravity = true;
		public float mass = 1;
		public Vector3 velocity, angularVelocity;

		private Transform transform;


		public void Init (Transform t) {
			transform = t;
		}

		public void FixedUpdate() {
			if (useGravity) AddForce(Physics.gravity, ForceMode.Acceleration);
			transform.position += velocity * Time.fixedDeltaTime;
			transform.rotation = Quaternion.AngleAxis( angularVelocity.magnitude * Mathf.Rad2Deg * Time.fixedDeltaTime, angularVelocity) * transform.rotation;
		}

		public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force) {
			switch (mode) {
			case ForceMode.Acceleration:
				velocity += force * Time.fixedDeltaTime;
				break;
			case ForceMode.Force:
				velocity += force / mass * Time.fixedDeltaTime;
				break;
			case ForceMode.VelocityChange:
				velocity += force;
				break;
			default: 
				Debug.LogError("Psudobody does not support this"); 
				break;
			}
		}

		public void AddTorque(Vector3 torque, ForceMode mode) {
			switch (mode) {
			case ForceMode.Acceleration:
				angularVelocity += torque * Time.fixedDeltaTime;
				break;
			case ForceMode.Force:
				angularVelocity += torque / mass * Time.fixedDeltaTime;
				break;
			case ForceMode.VelocityChange:
				angularVelocity += torque;
				break;
			default: 
				Debug.LogError("Psudobody does not support this"); 
				break;
			}
		}

	}
}
