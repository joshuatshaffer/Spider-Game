using UnityEngine;
using System.Collections;

public class Shuttle : MonoBehaviour {

	public bool returnOnLeave = true;
	public float timeOfTravel = 3;
	public float departureDelay = 0.5f;


	public Vector3 startPosition;
	public Vector3 endPosition;
	public Quaternion startRotation;
	public Quaternion endRotation;

	private float t;

	private Rigidbody body;

	public enum ShuttleState {
		waitingAtStart, movingToEnd, waitingAtEnd, movingToStart
	}
	public ShuttleState state = ShuttleState.waitingAtStart;

	void Start () {
		body = GetComponent<Rigidbody> ();
	}
	
	void FixedUpdate () {
		if (state == ShuttleState.waitingAtStart) {
			body.MovePosition(startPosition);
			body.MoveRotation(startRotation);
		} else if (state == ShuttleState.waitingAtEnd) {
			body.MovePosition(endPosition);
			body.MoveRotation(endRotation);
		} else if (state == ShuttleState.movingToEnd) {
			if (t > 1) {
				state = ShuttleState.waitingAtEnd;
			} else {
				t += Time.fixedDeltaTime / timeOfTravel;
				float tp = (1 - Mathf.Cos (Mathf.Clamp01(t) * 3.141592653589793f)) / 2;
				body.MovePosition(Vector3.Lerp(startPosition, endPosition, tp));
				body.MoveRotation(Quaternion.Lerp(startRotation, endRotation, tp));
			}
		} else if (state == ShuttleState.movingToStart) {
			if (t > 1) {
				state = ShuttleState.waitingAtStart;
			} else {
				t += Time.fixedDeltaTime / timeOfTravel;
				float tp = (1 - Mathf.Cos (Mathf.Clamp01(t) * 3.141592653589793f)) / 2;
				body.MovePosition(Vector3.Lerp(endPosition, startPosition, tp));
				body.MoveRotation(Quaternion.Lerp(endRotation, startRotation, tp));
			}
		}
	}

	public void GoToStart () {
		state = ShuttleState.movingToStart;
		t = -departureDelay;
	}

	public void GoToEnd () {
		state = ShuttleState.movingToEnd;
		t = -departureDelay;
	}

	void OnPlatformEnter () {
		if (state == ShuttleState.waitingAtStart) {
			GoToEnd();
		} else if (state == ShuttleState.waitingAtEnd) {
			GoToStart();
		} 
	}

	void OnPlatformLeave () {
		if (returnOnLeave && state == ShuttleState.waitingAtEnd) {
			GoToStart();
		}
	}

	[ContextMenu ("Set Start")]
	void SetStart () {
		startPosition = transform.position;
		startRotation = transform.rotation;
	}

	[ContextMenu ("Set End")]
	void SetEnd () {
		endPosition = transform.position;
		endRotation = transform.rotation;
	}

	[ContextMenu ("Jump to start")]
	void JumpToStart () {
		transform.position = startPosition;
		transform.rotation = startRotation;
	}

	[ContextMenu ("Jump to end")]
	void JumpToEnd () {
		transform.position = endPosition;
		transform.rotation = endRotation;
	}
}
