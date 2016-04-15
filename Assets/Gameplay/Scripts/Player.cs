using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public static Player current;
	public Transform eyePosition;

	void Awake () {
		if (current == null) {
			current = this;
		} else {
			Debug.LogError("Duplicate Player.");
			Destroy(gameObject);
		}
	}

	void Update () {
		if (transform.position.y < LevelController.current.killHeight) {
			Kill();
		}
	}

	//initiates the player's death secquence
	public void Kill () {
		BroadcastMessage("Die", SendMessageOptions.DontRequireReceiver);
	}

	void Die () {
		Destroy(gameObject);
		current = null;
		Checkpoint.SpawnPlayer ();
	}
}
