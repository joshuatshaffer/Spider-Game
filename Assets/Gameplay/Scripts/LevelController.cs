using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//This must be above all scripts that refer to LevelController.current in the execution order.
public class LevelController : MonoBehaviour {

	public static LevelController current;
	public Checkpoint activeCheckpoint;
	public GameObject gameplayCanvace;
	public GameObject playerPrefab;

	public enum LevelState {
		normal, paused, won, failed
	}
	public LevelState state {get; private set;}

	public int pausePixelation = 8;
	public GameObject pauseMenu;
	public RawImage lastImage;

	public float killHeight = -100;

	public GameObject deathFade;
	public GameObject respawnFade;
	public GameObject winFade;

	private RenderTexture lastTexture;
	private List<GameObject> pausedObjects;

	void Awake () {
		if (current == null) {
			current = this;
		} else {
			Debug.LogError("Duplicate LevelController.");
			Destroy(gameObject);
		}
		state = LevelState.normal;
		pauseMenu.SetActive(false);
		activeCheckpoint.SpawnPlayer ();
	}
	
	void Update () {
		if (Input.GetButtonDown("Cancel")) {
			Pause();
		}
	}
	
	public bool ActivateCheckpoint (Checkpoint cp) {
		if (activeCheckpoint != cp) {
			activeCheckpoint = cp;
			return true;
		} else {
			return false;
		}
	}

	public void PlayerDied () {
		InstantiateFade (deathFade);
	}

	public void SpawnPlayer () {
		activeCheckpoint.SpawnPlayer ();
		InstantiateFade (respawnFade);
	}
	
	public void WinLevel () {
		if (state != LevelState.normal) return;
		state = LevelState.won;
		InstantiateFade (winFade);
		LevelSelect.WonLevel(SceneManager.GetActiveScene().name);
	}

	void InstantiateFade (GameObject fade) {
		(Instantiate(fade) as GameObject).transform.SetParent(gameplayCanvace.transform, false);
	}

	public void QuitLevel () {
		SceneManager.LoadScene("Main Menu");
		SetCurserLock (false);
	}

	public void Pause() {
		if (state == LevelState.paused || state == LevelState.won) return;
		state = LevelState.paused;
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
		if (state != LevelState.paused) return;
		state = LevelState.normal;
		Time.timeScale = 1;

		pauseMenu.SetActive(false);
		foreach (GameObject go in pausedObjects) {
			go.SetActive(true);
			go.BroadcastMessage("OnUnpause", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void SetCurserLock (bool isLocked=true) {
		Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !isLocked;
	}
}
