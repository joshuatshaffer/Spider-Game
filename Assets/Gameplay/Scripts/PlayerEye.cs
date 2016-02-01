using UnityEngine;
using System.Collections;

public class PlayerEye : MonoBehaviour {
	
	void LateUpdate () {
		if (Player.current != null) {
			transform.position = Player.current.eyePosition.position;
			transform.rotation = Player.current.eyePosition.rotation;
		}
	}
}
