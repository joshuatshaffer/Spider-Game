using UnityEngine;
using System.Collections;

namespace PlayerMovement {
	[System.Serializable]
	public class Psudobody {
		
		public bool useGravity = true;
		public float mass = 1;
		public Vector3 velocity, angularVelocity;
		
		public Rigidbody gbody;
		public Vector3 position;
		public Quaternion rotation;

		private Transform transform;
		private Ground ground;
		private Head head;
		public void Init (Head h, Transform t, Ground g) {
			head = h;
			transform = t;
			ground = g;

			position = transform.position;
			rotation = transform.rotation;
		}

		public void FixedUpdate() {
			if (useGravity) AddForce(Physics.gravity, ForceMode.Acceleration);
			position += velocity * Time.deltaTime;
			rotation = Quaternion.AngleAxis( angularVelocity.magnitude * Mathf.Rad2Deg * Time.deltaTime, angularVelocity) * rotation;

			transform.position = DerelevisePosition(position);
			transform.rotation = DereleviseRotation(rotation);
		}

		public void ChangeGroundbody (Rigidbody b) {
			if (gbody == b) return;

			Debug.Log(b);

			if (gbody != null) {
				position = DerelevisePosition(position);
				rotation = DereleviseRotation(rotation);

				velocity = DereleviseDirection(velocity);
				angularVelocity = DereleviseDirection(angularVelocity);

				head.lastHeadAxis = DereleviseDirection(head.lastHeadAxis);
				head.lastNeckAxis = DereleviseDirection(head.lastNeckAxis);
			}
			gbody = b;
			if (b != null) {
				position = RelevisePosition(transform.position);
				rotation = ReleviseRotation(rotation);

				velocity = ReleviseDirection(velocity);
				angularVelocity = ReleviseDirection(angularVelocity);
			}
		}

		public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force) {
			switch (mode) {
			case ForceMode.Acceleration:
				velocity += force * Time.deltaTime;
				break;
			case ForceMode.Force:
				velocity += force / mass * Time.deltaTime;
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
				angularVelocity += torque * Time.deltaTime;
				break;
			case ForceMode.Force:
				angularVelocity += torque / mass * Time.deltaTime;
				break;
			case ForceMode.VelocityChange:
				angularVelocity += torque;
				break;
			default: 
				Debug.LogError("Psudobody does not support this"); 
				break;
			}
		}

		public Vector3 ReleviseDirection (Vector3 dir) {
			if (gbody != null) {
				return gbody.transform.InverseTransformDirection(dir);
			} else {
				return dir;
			}
		}
		public Vector3 RelevisePosition (Vector3 pos) {
			if (gbody != null) {
				return gbody.transform.InverseTransformPoint(pos);
			} else {
				return pos;
			}
		}
		public Quaternion ReleviseRotation (Quaternion rot) {
			if (gbody != null) {
				return Quaternion.Inverse (gbody.rotation) * rot;
			} else {
				return rot;
			}
		}

		public Vector3 DereleviseDirection (Vector3 dir) {
			if (gbody != null) {
				return gbody.transform.TransformDirection(dir);
			} else {
				return dir;
			}
		}
		public Vector3 DerelevisePosition (Vector3 pos) {
			if (gbody != null) {
				return gbody.transform.TransformPoint(pos);
			} else {
				return pos;
			}
		}
		public Quaternion DereleviseRotation (Quaternion rot) {
			if (gbody != null) {
				return gbody.rotation * rot;
			} else {
				return rot;
			}
		}


		public Vector3 forward {
			get {
				return rotation * Vector3.forward;
			}
		}
		public Vector3 up {
			get {
				return rotation * Vector3.up;
			}
		}
	}
}
