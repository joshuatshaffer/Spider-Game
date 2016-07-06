using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlayerMovement {
	public class SpiderLocomotion : MonoBehaviour {

		public Movement movement;
		public Head head;
		public Feet feet;
		public Psudobody body;

		[System.NonSerialized] private Ground ground;
		[System.NonSerialized] private StepingSounds stepingSounds;

		private Vector3 lastPos;

		void Awake () {
			ground = new Ground ();

			feet.Init (transform, ground);
			ground.Init (body, feet);
			head.Init (ground, body);
			movement.Init (body, ground, head, transform);
			body.Init(head, transform);

			stepingSounds = gameObject.GetComponent<StepingSounds> ();
			lastPos = transform.position;

			OnUnpause ();
		}

		void Update () {
			movement.Update ();
			body.UpdatePositioning ();
			head.Update ();
			if (ground.hasTraction && !movement.isJumping)
				stepingSounds.Ack_Moved((body.velocity * Time.deltaTime).magnitude);
		}

		void FixedUpdate(){
			body.UpdatePositioning ();

			ground.Update ();
			movement.FixedUpdate ();

			body.FixedUpdate ();
		}

		void OnUnpause () {
			GameController.SetCurserLock (true);
			Input.ResetInputAxes();
		}

		public Vector3 worldVelocity { get { return body.worldVelocity; } }
		public Vector3 worldAngularVelocity { get { return body.worldAngularVelocity; } }
	}
}