using UnityEngine;
using System.Collections;

namespace PlayerMovement {
	[System.Serializable]
	public class Movement {
		//how fast it moves
		public float movmentSpeed = 70;

		//how quickly it stops
		public float velocityStoping = 30;
		public float rotationStoping = 40;

		//how aggresivly it sticks to the wall
		public float linearCorrection = 100;
		public float angularCorrection = 600;

		public float standingHeight = 0.5f;

		public float jumpSpeed = 9.3f;
		public float minJumpTime = 0.4f;
		private float lastJumpTime = 0;
		public bool isJumping {get{return lastJumpTime + minJumpTime > Time.time;}}

		[System.NonSerialized] private Psudobody body;
		[System.NonSerialized] private Ground ground;
		[System.NonSerialized] private Head head;

		public void Init (Psudobody b, Ground g, Head h, Transform t) {
			body = b;
			ground = g;
			head = h;
		}

		public void Update () {
			if (!isJumping && ground.isGrounded && Input.GetButtonDown ("Jump")) {
				if (ground.hasTraction) {
					body.AddForce (head.axis * jumpSpeed, ForceMode.VelocityChange);
				} else {
					body.AddForce (Vector3.Project(head.axis * jumpSpeed, ground.normal), ForceMode.VelocityChange);
				}
				lastJumpTime = Time.time;
			}
		}

		public void FixedUpdate () {
			if (ground.isGrounded) {
				body.useGravity = false;
				GroundedAngular ();
				if (!isJumping && ground.hasTraction) {
					TractionLinear ();
				} else {
					SlipLinear ();
				}
			} else {
				body.useGravity = true;
				FallingAngular ();
			}
		}

		void TractionLinear () {
			body.AddForce (
				// Mantain distance from ground.
				Vector3.Project (
					ground.centroid + ground.normal * standingHeight - body.position,
					ground.normal
				) * linearCorrection
				// Walk about.
				+ Vector3.ClampMagnitude(
					head.forward * Input.GetAxis ("Vertical") + head.right * Input.GetAxis ("Horizontal"), 
					1
				) * movmentSpeed 
				//Control velocity not acceleration.
				- body.velocity * velocityStoping,

				ForceMode.Acceleration
			);
		}

		void SlipLinear () {
			Vector3 wantedAcceleration = 
				Vector3.Project (
					// Mantain distance from ground.
					(ground.centroid + ground.normal * standingHeight - body.position) * linearCorrection
					//Control velocity not acceleration.
					- body.velocity * velocityStoping,
					ground.normal
				);

			bool isPushingAway = Vector3.Dot (wantedAcceleration, ground.normal) > 0;
			bool isWithGravity = Vector3.Dot (wantedAcceleration, Physics.gravity) > 0;

			if (isPushingAway) {
				wantedAcceleration += Vector3.ProjectOnPlane (Physics.gravity, ground.normal);
			} else {
				if (isWithGravity) {
					Vector3 gravOnGNorm = Vector3.Project (Physics.gravity, ground.normal);

					if (wantedAcceleration.sqrMagnitude > gravOnGNorm.sqrMagnitude) {
						wantedAcceleration = gravOnGNorm;
					}

					wantedAcceleration += Vector3.ProjectOnPlane (Physics.gravity, ground.normal);
				} else {
					wantedAcceleration = Physics.gravity;
				}
			}

			body.AddForce (wantedAcceleration, ForceMode.Acceleration);
		}

		void GroundedAngular () {
			body.AddTorque (
				Vector3.Cross (body.up, ground.normal) * angularCorrection 
				- body.angularVelocity * rotationStoping,
				ForceMode.Acceleration
			);
		}

		void FallingAngular () {
			body.AddTorque (
				(
					Vector3.Cross (body.up, head.rotation * new Vector3(0.0f, 0.707106781186548f, -0.707106781186548f)) +
					Vector3.Cross(body.forward, head.neckForward)
				) * angularCorrection
				- body.angularVelocity * rotationStoping,
				ForceMode.Acceleration
			);
		}
	}
}