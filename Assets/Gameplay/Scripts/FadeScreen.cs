using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeScreen : MonoBehaviour {

	public float fadeSpeed = 1.0f;
	public Color start, end;
	public ActionOnFin actionOnFin;

	public enum ActionOnFin {
		destroySelf,
		endLevel,
		respawnPlayer
	}

	private float t = 0;
	private Image image;

	void Awake () {
		image = GetComponent<Image>();
		image.color = start;
	}

	void Update () {
		t += fadeSpeed * Time.deltaTime;
		if (t > 1) {
			switch (actionOnFin) {
			case ActionOnFin.destroySelf:
				Destroy(gameObject);
				break;
			case ActionOnFin.endLevel:
				Destroy(this);
				LevelController.current.QuitLevel();
				break;
			case ActionOnFin.respawnPlayer:
				Destroy(gameObject);
				//LevelController.current.SpawnPlayer();
				break;
			}
		}
		image.color = Color.Lerp(start, end, t);
	}
}
