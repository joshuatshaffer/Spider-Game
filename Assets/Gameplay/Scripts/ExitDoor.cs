using UnityEngine;
using System.Collections;

// Player wins the level upon reaching a trigger on this object
public class ExitDoor : MonoBehaviour {

	void OnPlayerTouch () {
		LevelController.current.WinLevel();
	}
}
