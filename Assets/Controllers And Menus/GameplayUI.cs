using UnityEngine;
using System.Collections;

public class GameplayUI : MonoBehaviour {

	public static GameplayUI current;

	void Awake () {
		if (current == null) {
			current = this;
		} else {
			Debug.LogError("Duplicate GameplayUI.");
			Destroy(gameObject);
		}
	}
}
