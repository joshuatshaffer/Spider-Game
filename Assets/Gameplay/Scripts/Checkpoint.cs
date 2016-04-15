using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public static Checkpoint activeCheckpoint;
	public bool isStart = false;

	void Awake () {
		if (isStart) {
			if (activeCheckpoint == null) {
				Activate ();
				SpawnPlayer ();
			} else {
				Debug.LogError ("Multipule start checkpoints");
			}
		}
	}

	void OnPlayerTouch () {
		Activate ();
	}

	public void Activate () {
		if (this != activeCheckpoint) {
			if (activeCheckpoint != null) 
				activeCheckpoint.Deactivate ();
			activeCheckpoint = this;
			Debug.Log ("Checkpoint!!!");
		}
	}

	public void Deactivate () {
		if (this == activeCheckpoint) {
			activeCheckpoint = null;
			Debug.Log ("Checkpoint deactivated.");
		}
	}

	public void SpawnPlayer () {
		Instantiate(LevelController.current.playerPrefab, transform.position, transform.rotation);
	}
}
