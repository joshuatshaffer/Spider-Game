using UnityEngine;
using System.Collections;

// Player wins the level upon reaching a trigger on this object
public class ExitDoor : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.transform.root.gameObject.layer == 8) {
			LevelController.current.WinLevel();
		}
	}
}
