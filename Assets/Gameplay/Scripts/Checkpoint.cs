using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	void OnPlayerTouch () {
		Activate ();
	}

	public void Activate () {
		if (LevelController.current.ActivateCheckpoint (this)) {
			Debug.Log ("Checkpoint!!!");
		}
	}

	public void SpawnPlayer () {
		Instantiate(LevelController.current.playerPrefab, transform.position, transform.rotation);
	}
}
