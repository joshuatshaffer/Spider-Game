using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeScreen : MonoBehaviour {

	public float duration = 1.0f;
	public Color start, end;

	private float startTime = 0;
	new private GUITexture guiTexture;

	void Awake () {
		startTime = Time.time;
		guiTexture = GetComponent<GUITexture>();
		guiTexture.color = start;
	}

	void Update () {
		float progress = (Time.time - startTime) / duration;
		if (progress >= 1) {
			guiTexture.color = end;
			Destroy (gameObject);
		} else {
			guiTexture.color = Color.Lerp (start, end, progress);
		}
	}
}
