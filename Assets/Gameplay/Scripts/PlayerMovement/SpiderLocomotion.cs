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
			ground.Init (body, feet, transform);
			head.Init (ground, body);
			movement.Init (body, ground, head, transform);
			body.Init(transform, ground);

			OnUnpause ();
		}

		void Update () {
			movement.Update ();


			ground.Update ();
			movement.FixedUpdate ();
			body.FixedUpdate();

			head.Update ();
		}


		void OnUnpause () {
			LevelController.current.SetCurserLock (true);
			Input.ResetInputAxes();
		}
	}
}