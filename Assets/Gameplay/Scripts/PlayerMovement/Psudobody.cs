﻿using UnityEngine;
using System.Collections;

namespace PlayerMovement {
	[System.Serializable]
	public class Psudobody {
		
		public bool useGravity = true;
		public float mass = 1, radius = 0.25f;

		public Vector3 velocity, angularVelocity;
		public Vector3 position;
		public Quaternion rotation;
		
		[System.NonSerialized] private Rigidbody groundBody;
		[System.NonSerialized] private Transform transform;
		[System.NonSerialized] private Head head;

		public void Init (Head h, Transform t) {
			head = h;
			transform = t;

			position = transform.position;
			rotation = transform.rotation;
		}

		public void FixedUpdate () {
			if (useGravity) AddForce(Physics.gravity, ForceMode.Acceleration);
			Vector3 lastposition = position;
			position += velocity * Time.fixedDeltaTime;
			CheckTunnenling (lastposition);
			rotation = Quaternion.AngleAxis(angularVelocity.magnitude * Mathf.Rad2Deg * Time.fixedDeltaTime, angularVelocity) * rotation;
			UpdatePositioning ();
			TouchTriggers ();
		}

		void TouchTriggers () {
			Collider[] hits = Physics.OverlapSphere(DerelevisePosition(position), radius, 1 << 9);
			foreach (Collider c in hits) {
				c.SendMessageUpwards ("OnPlayerTouch", SendMessageOptions.DontRequireReceiver);
			}
		}

		void CheckTunnenling (Vector3 lastposition) {
			RaycastHit hit;
			lastposition = DerelevisePosition (lastposition);
			Vector3 deltaPosition = DerelevisePosition (position) - lastposition;
			if (Physics.SphereCast(lastposition, radius, deltaPosition, out hit, deltaPosition.magnitude,~0,QueryTriggerInteraction.Ignore)) {
				position = RelevisePosition (hit.point + hit.normal * radius);
				velocity = Vector3.ProjectOnPlane (velocity, ReleviseDirection (hit.normal));
			}
		}

		public void UpdatePositioning () {
			transform.position = DerelevisePosition(position);
			transform.rotation = DereleviseRotation(rotation);
		}

		public void ChangeGroundbody (Rigidbody b) {
			if (groundBody == b) return;

			Debug.Log(b);

			if (groundBody != null) {
				groundBody.SendMessage ("OnPlatformLeave", SendMessageOptions.DontRequireReceiver);

				position = DerelevisePosition(position);
				rotation = DereleviseRotation(rotation);

				velocity = DereleviseDirection(velocity) + groundBody.GetPointVelocity(position);
				angularVelocity = DereleviseDirection(angularVelocity) + groundBody.angularVelocity;

				head.lastHeadAxis = DereleviseDirection(head.lastHeadAxis);
				head.lastNeckAxis = DereleviseDirection(head.lastNeckAxis);
			}
			groundBody = b;
			if (b != null) {
				groundBody.SendMessage ("OnPlatformEnter", SendMessageOptions.DontRequireReceiver);

				velocity = ReleviseDirection(velocity - groundBody.GetPointVelocity(position));
				angularVelocity = ReleviseDirection(angularVelocity - groundBody.angularVelocity);

				position = RelevisePosition(transform.position);
				rotation = ReleviseRotation(rotation);

				head.lastHeadAxis = ReleviseDirection(head.lastHeadAxis);
				head.lastNeckAxis = ReleviseDirection(head.lastNeckAxis);
			}
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

		public Vector3 ReleviseDirection (Vector3 worldDirection) {
			if (groundBody != null)
				return groundBody.transform.InverseTransformDirection(worldDirection);
			return worldDirection;
		}
		public Vector3 RelevisePosition (Vector3 worldPosition) {
			if (groundBody != null)
				return groundBody.transform.InverseTransformPoint(worldPosition);
			return worldPosition;
		}
		public Quaternion ReleviseRotation (Quaternion worldRotation) {
			if (groundBody != null)
				return Quaternion.Inverse (groundBody.rotation) * worldRotation;
			return worldRotation;
		}

		public Vector3 DereleviseDirection (Vector3 localDirection) {
			if (groundBody != null)
				return groundBody.transform.TransformDirection(localDirection);
			return localDirection;
		}
		public Vector3 DerelevisePosition (Vector3 localPosition) {
			if (groundBody != null)
				return groundBody.transform.TransformPoint(localPosition);
			return localPosition;
		}
		public Quaternion DereleviseRotation (Quaternion localRotaton) {
			if (groundBody != null)
				return groundBody.rotation * localRotaton;
			return localRotaton;
		}
		public Vector3 worldVelocity {
			get {
				if (groundBody != null) {
					return DereleviseDirection (velocity) + groundBody.GetPointVelocity (position);
				} else {
					return velocity;
				}
			}
		}
		public Vector3 worldAngularVelocity {
			get {
				if (groundBody != null) {
					return DereleviseDirection(angularVelocity) + groundBody.angularVelocity;
				} else {
					return angularVelocity;
				}
			}
		}
		public Vector3 forward {get{return rotation * Vector3.forward;}}
		public Vector3 up {get{return rotation * Vector3.up;}}
	}
}
