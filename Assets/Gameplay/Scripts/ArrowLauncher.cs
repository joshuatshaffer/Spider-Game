﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerMovement.SpiderLocomotion))]
public class ArrowLauncher : MonoBehaviour {
	
	public GameObject arrowPrefab;
	public Transform muzzle;
	public float muzzleSpeed = 10;
	public float imprecision = 1;
	private PlayerMovement.SpiderLocomotion body;

	void Start () {
		body = GetComponent<PlayerMovement.SpiderLocomotion> ();
	}

	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			GameObject arrow = (GameObject)Instantiate (arrowPrefab, muzzle.position, muzzle.rotation);
			Vector3 velocity = Random.insideUnitCircle * imprecision;
			velocity = muzzle.rotation * new Vector3(velocity.x, velocity.y, muzzleSpeed) + body.worldVelocity;
			arrow.GetComponent<Rigidbody> ().velocity = velocity;
		}
	}
}
