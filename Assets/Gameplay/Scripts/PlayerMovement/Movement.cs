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
		private bool isJumping {get{return lastJumpTime + minJumpTime > Time.time;}}

		private Psudobody body;
		private Ground ground;
		private Head head;

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
				GroundedAngular ();
				if (!isJumping && ground.hasTraction) {
					TractionLinear ();
				} else {
					SlipLinear ();
				}
			} else {
				FallingAngular ();
			}
		}

		void TractionLinear () {
			body.useGravity = false;

			Vector3 correctionVelocity = ground.centroid - (body.position - ground.normal * standingHeight);
			correctionVelocity = Vector3.Project (correctionVelocity, ground.normal) * linearCorrection;

			Vector3 wantedVelocity = head.forward * Input.GetAxis ("Vertical");
			wantedVelocity += head.right * Input.GetAxis ("Horizontal");
			wantedVelocity = Vector3.ClampMagnitude(wantedVelocity, 1) * movmentSpeed;

			wantedVelocity += correctionVelocity;
			body.AddForce (wantedVelocity - body.velocity * velocityStoping, ForceMode.Acceleration);
		}

		void SlipLinear () {
			body.useGravity = false;
			bool gravDone = false;

			Vector3 wantedVelocity = ground.centroid - (body.position - ground.normal * standingHeight);
			wantedVelocity = Vector3.Project (wantedVelocity, ground.normal) * linearCorrection;
			wantedVelocity -= body.velocity * velocityStoping;
			wantedVelocity = Vector3.Project (wantedVelocity, ground.normal);

			if (Vector3.Dot (wantedVelocity, ground.normal) > 0) {
				body.AddForce (wantedVelocity, ForceMode.Acceleration);
			} else {
				if (Vector3.Dot (wantedVelocity, Physics.gravity) > 0) {
					Vector3 foo = Vector3.Project (Physics.gravity, wantedVelocity);
					if (wantedVelocity.sqrMagnitude < foo.sqrMagnitude) {
						body.AddForce (wantedVelocity);
					} else {
						body.AddForce (foo);
					}
				} else {
					gravDone = true;
					body.AddForce (Physics.gravity, ForceMode.Acceleration);
				}
			}
			if (!gravDone) {
				body.AddForce (Vector3.ProjectOnPlane(Physics.gravity, ground.normal), ForceMode.Acceleration);
			}
		}

		void GroundedAngular () {
			Vector3 wantedAngularVelocity = Vector3.Cross (body.up, ground.normal);
			wantedAngularVelocity *= angularCorrection;
			body.AddTorque (wantedAngularVelocity - body.angularVelocity * rotationStoping, ForceMode.Acceleration);
		}

		void FallingAngular () {
			body.useGravity = true;
			Vector3 wantedAngularVelocity = 
				Vector3.Cross (body.up, head.rotation * new Vector3(0.0f, 0.707106781186548f, -0.707106781186548f)) +
				Vector3.Cross(body.forward, head.neckForward);
			wantedAngularVelocity *= angularCorrection;
			body.AddTorque (wantedAngularVelocity - body.angularVelocity * rotationStoping, ForceMode.Acceleration);
		}
	}
}