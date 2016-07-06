using UnityEngine;
using System.Collections;

public class StepingSounds : MonoBehaviour {
	// Leg enumaration:
	//   Head
	// 0      4
	// 1      5
	// 2      6
	// 3      7
	public float volume = 1;
	public AudioClip stepSound;
	//{xa = -0.61, yfa = -0.46, 0.54 -0.79 0.19
	public FootSounder[] footSounders;

	private float t = 0; 
	private int i = 0;

	public void Ack_Moved (float dist) {
		t += dist;
		while (t >= footSounders[i].time) {
			t -= footSounders[i].time;
			footSounders [i].audio.PlayOneShot (stepSound, volume);
			++i;
			if (i >= footSounders.Length)
				i = 0;
		}
	}
}

[System.Serializable]
public class FootSounder {

	public float time;
	public AudioSource audio;

}