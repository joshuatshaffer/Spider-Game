using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public static Player current;
	public Transform eyePosition;
	public GameObject deathFadePrefab;

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
		float duration = Instantiate (deathFadePrefab).GetComponent<FadeScreen>().duration;
		Checkpoint.activeCheckpoint.Invoke ("SpawnPlayer", duration);
	}
}
