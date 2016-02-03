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

		[System.NonSerialized]private Ground ground;

		void Awake () {
			ground = new Ground ();

			feet.Init (transform, ground);
			ground.Init (body, feet);
			head.Init (ground, body);
			movement.Init (body, ground, head, transform);
			body.Init(head, transform);

			OnUnpause ();
		}

		void Update () {
			movement.Update ();
			body.UpdatePositioning ();
			head.Update ();
		}

		void FixedUpdate(){
			body.UpdatePositioning ();

			ground.Update ();
			movement.FixedUpdate ();

			body.FixedUpdate();
		}

		/*void Update () {


			ground.Update ();
			movement.FixedUpdate ();
			body.FixedUpdate();

			head.Update ();
		}*/


		void OnUnpause () {
			LevelController.current.SetCurserLock (true);
			Input.ResetInputAxes();
		}
	}
}