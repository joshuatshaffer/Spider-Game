using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO
 * remove jitter when on rigidbody (probably due to position update timeing)
 */
namespace PlayerMovement {
	[RequireComponent (typeof (Rigidbody))]
	public class SpiderLocomotion : MonoBehaviour {

		public Movement movement;
		public Head head;
		public Feet feet;

		private Ground ground;
		private Rigidbody body;

		void Awake () {
			ground = new Ground ();
			body = GetComponent<Rigidbody> ();

			feet.Init (body, ground);
			ground.Init (body, feet);
			head.Init (ground);
			movement.Init (body, ground, head, transform);

			SetCurserLock ();
		}

		void Update () {
			head.Update ();
			movement.Update ();
		}

		void FixedUpdate () {
			ground.Update ();
			movement.FixedUpdate ();
		}

		void SetCurserLock (bool isLocked=true) {
			Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !isLocked;
		}
	}
}