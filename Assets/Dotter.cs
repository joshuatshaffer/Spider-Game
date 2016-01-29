using UnityEngine;
using System.Collections;

public class Dotter : MonoBehaviour {

	void LateUpdate () {
		Vector3 p = transform.position;
		float d = 0.1f;
		Debug.DrawLine (p + new Vector3 (d, 0, 0), p - new Vector3 (d, 0, 0), Color.red, 0.1f, false);
		Debug.DrawLine (p + new Vector3 (0, d, 0), p - new Vector3 (0, d, 0), Color.red, 0.1f, false);
		Debug.DrawLine (p + new Vector3 (0, 0, d), p - new Vector3 (0, 0, d), Color.red, 0.1f, false);
	}
}
