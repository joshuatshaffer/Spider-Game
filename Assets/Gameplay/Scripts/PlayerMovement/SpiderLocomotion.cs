using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO
 * remove jitter when on rigidbody (probably due to position update timeing)
 */
namespace PlayerMovement {
	public class SpiderLocomotion : MonoBehaviour {

		public Movement movement;
		public Head head;
		public Feet feet;
		public Psudobody body;

		private Ground ground;

		void Awake () {
			ground = new Ground ();

			feet.Init (transform, ground);
			ground.Init (feet, transform);
			head.Init (ground);
			movement.Init (body, ground, head, transform);
			body.Init(transform);

			OnUnpause ();
		}

		void Update () {
			head.Update ();
			movement.Update ();
		}

		void FixedUpdate () {
			ground.Update ();
			movement.FixedUpdate ();
			body.FixedUpdate();
		}

		void OnUnpause () {
			LevelController.current.SetCurserLock (true);
			Input.ResetInputAxes();
		}
	}
}