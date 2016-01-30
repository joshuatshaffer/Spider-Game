using UnityEngine;
using System.Collections;

namespace PlayerMovement {
	[System.Serializable]
	public class Head {
		public float yawSpeed = 5;
		public float pitchSpeed = 5;
		public float maxHeadPitch = 89.0f;
		public float minHeadPitch = -89.0f;
		public Transform head, neck;

		private Vector3 lastHeadAxis, lastNeckAxis;

		private Ground ground;

		public void Init (Ground g) {
			ground = g;

			lastHeadAxis = head.forward;
			lastNeckAxis = neck.forward;
		}

		public void Update () {
			//Head and neck MUST be done separately because singularities are a pain.

			Quaternion groundRotation = Quaternion.AngleAxis(
				ground.angularVelocity.magnitude * Mathf.Rad2Deg * Time.fixedDeltaTime, 
				ground.angularVelocity);

			lastNeckAxis = groundRotation * lastNeckAxis;
			lastHeadAxis = groundRotation * lastHeadAxis;

			lastNeckAxis = neck.parent.InverseTransformVector(lastNeckAxis);
			lastHeadAxis = neck.InverseTransformVector(lastHeadAxis);

			float  yaw  = Quaternion.LookRotation(lastNeckAxis).eulerAngles.y;
			float pitch = Quaternion.LookRotation(lastHeadAxis).eulerAngles.x;

			yaw  += Input.GetAxis ("Mouse X") * yawSpeed;
			pitch -= Input.GetAxis ("Mouse Y") * pitchSpeed;

			while (pitch >  180) pitch -= 360;
			while (pitch < -180) pitch += 360;
			pitch = Mathf.Clamp(pitch, minHeadPitch, maxHeadPitch);

			neck.localRotation = Quaternion.Euler(0,yaw,0);
			head.localRotation = Quaternion.Euler(pitch,0,0);

			lastNeckAxis = neck.forward;
			lastHeadAxis = head.forward;
		}

		public Vector3 forward {
			get {
				return Vector3.ProjectOnPlane (neck.forward, ground.normal).normalized;
			}
		}
		public Vector3 right {
			get {
				return Vector3.ProjectOnPlane (neck.right, ground.normal).normalized;
			}
		}
		public Vector3 neckForward {
			get {
				return neck.forward;
			}
		}
		public Vector3 axis {
			get {
				return head.forward;
			}
		}
		public Quaternion rotation {
			get {
				return head.rotation;
			}
		}
	}
}