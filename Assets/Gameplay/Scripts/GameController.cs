using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//This must be above all scripts that refer to LevelController.current in the execution order.
public class GameController : MonoBehaviour {

	public static GameController current;
	
	public GameObject pauseMenu;
	public RawImage lastImage;
	public int pausePixelation = 8;

	private RenderTexture lastTexture;
	private List<GameObject> pausedObjects;
	private bool isPaused = false;

	void Awake () {
		if (current == null) {
			current = this;
		} else {
			Debug.LogError("Duplicate GameController.");
			Destroy(gameObject);
		}
		pauseMenu.SetActive(false);
	}

	void Update () {
		if (Input.GetButtonDown("Cancel")) {
			Pause();
		}
	}

	public void WinLevel () {
		LevelSelect.WonLevel(SceneManager.GetActiveScene().name);
	}

	public void ExitLevel () {
		Unpause ();
		SetCurserLock (false);
		SceneManager.LoadScene("Main Menu");
	}

	public void Pause() {
		if (isPaused) return;
		isPaused = true;
		Time.timeScale = 0;

		Destroy(lastTexture);
		lastTexture = new RenderTexture(Screen.width/pausePixelation, Screen.height/pausePixelation, 24);
		lastTexture.filterMode = FilterMode.Point;
		Camera.main.targetTexture = lastTexture;
		Camera.main.Render();
		Camera.main.targetTexture = null;
		lastImage.texture = lastTexture;

		pausedObjects = new List<GameObject>();
		GameObject[] gos = FindObjectsOfType<GameObject>();
		foreach (GameObject go in gos) {
			if (go && go.transform.parent == null && go != gameObject) {
				pausedObjects.Add(go);
				go.BroadcastMessage("OnPause", SendMessageOptions.DontRequireReceiver);
				go.SetActive(false);
			}
		}
		pauseMenu.SetActive(true);

		SetCurserLock(false);
	}

	public void Unpause() {
		if (!isPaused) return;
		isPaused = false;
		Time.timeScale = 1;

		pauseMenu.SetActive(false);
		foreach (GameObject go in pausedObjects) {
			go.SetActive(true);
			go.BroadcastMessage("OnUnpause", SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void SetCurserLock (bool isLocked=true) {
		Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !isLocked;
	}
}
