using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public static Checkpoint active;
	public GameObject playerPrefab;

	void OnTriggerEnter(Collider other) {
		if (other.transform.root.gameObject.layer == 8) {
			active = this;
		}
	}

	public void SpawnPlayer () {
		Instantiate(playerPrefab,transform.position, transform.rotation);
	}
}
