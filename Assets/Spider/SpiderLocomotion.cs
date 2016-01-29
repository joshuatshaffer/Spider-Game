using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO
 * remove jitter when on rigidbody (probably due to position update timeing)
 */
public class SpiderLocomotion : MonoBehaviour {
	
	[SerializeField] public Movement movement;
	[SerializeField] public Head head;
	[SerializeField] public Feet feet;

	private Ground ground;
	private Rigidbody body;

	void Awake () {
		ground = new Ground ();
		body = GetComponent<Rigidbody> ();

		feet.Init (body, ground);
		ground.Init (body, feet);
		head.Init (ground);
		movement.Init (body, ground, head, transform);
	}

	void Update () {
		head.Update ();
		movement.Update ();
	}

	void FixedUpdate () {
		ground.Update ();
		movement.FixedUpdate ();
	}
}