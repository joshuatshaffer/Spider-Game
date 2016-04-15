using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//This must be above all scripts that refer to LevelController.current in the execution order.
public class LevelController : MonoBehaviour {

	public static LevelController current;

	public GameObject playerPrefab;
	public GameObject winFadePrefab;

	public float killHeight = -100;

	void Awake () {
		if (current == null) {
			current = this;
		} else {
			Debug.LogError("Duplicate LevelController.");
			Destroy(gameObject);
		}
	}

	public void WinLevel () {
		GameController.current.WinLevel();
		float duration = Instantiate (winFadePrefab).GetComponent<FadeScreen>().duration;
		GameController.current.Invoke ("ExitLevel", duration);
	}
}
