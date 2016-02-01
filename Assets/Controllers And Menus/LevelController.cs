﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelController : MonoBehaviour {

	public static LevelController current;

	public int pausePixelation = 8;
	public GameObject pauseMenu;
	public RawImage lastImage;
	private RenderTexture lastTexture;

	public bool paused { get; private set; }

	private List<GameObject> pausedObjects;

	void Start () {
		if (current == null) {
			current = this;
		} else {
			Debug.LogError("Duplicate LevelController.");
			Destroy(gameObject);
		}
		paused = false;
		pauseMenu.SetActive(false);
	}

	void Update () {
		if (!paused && Input.GetButtonDown("Cancel")) {
			Pause();
		}
	}

	public void Pause() {
		if (paused) return;
		paused = true;
		Time.timeScale = 0;

		Destroy(lastTexture);
		lastTexture = new RenderTexture(Screen.width/pausePixelation, Screen.height/pausePixelation, 24);
		lastTexture.filterMode = FilterMode.Point;
		Camera.main.targetTexture = lastTexture;
		Camera.main.Render();
		Camera.main.targetTexture = null;
		lastImage.texture = lastTexture;

		pausedObjects = new List<GameObject>();
		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject go in gos) {
			if (go && go.transform.parent == null && go != gameObject) {
				pausedObjects.Add(go);
				go.BroadcastMessage("OnPause", SendMessageOptions.DontRequireReceiver);
				go.SetActive(false);
			}
		}
		pauseMenu.SetActive(true);
	}

	public void Unpause() {
		if (!paused) return;
		paused = false;
		Time.timeScale = 1;
		foreach (GameObject go in pausedObjects) {
			go.SetActive(true);
			go.BroadcastMessage("OnUnpause", SendMessageOptions.DontRequireReceiver);
		}
		pauseMenu.SetActive(false);
	}
}