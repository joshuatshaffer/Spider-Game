using UnityEngine;
using System.Collections;

public class ArrowLauncher : MonoBehaviour {

	public GameObject arrowPrefab;
	public Transform muzzle;
	public float muzzleSpeed = 10;
	public float inAcuracy = 1;
	private Rigidbody body;

	void Start () {
		body = GetComponent<Rigidbody> ();
	}

	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			GameObject arrow = (GameObject)Instantiate (arrowPrefab, muzzle.position, muzzle.rotation);
			Vector3 velocity = Random.insideUnitCircle * inAcuracy;
			velocity = muzzle.rotation * new Vector3(velocity.x, velocity.y, muzzleSpeed) + body.velocity;
			arrow.GetComponent<Rigidbody> ().velocity = velocity;
		}
	}
}
