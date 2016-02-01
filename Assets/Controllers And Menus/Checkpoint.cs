using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.transform.root.gameObject.layer == 8) {
			Activate ();
		}
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
