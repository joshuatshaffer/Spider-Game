using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//human walk speed = 1.38 m/s
//human run speed  = 6.71 m/s

/*
 * TODO
 * remove jitter when on rigidbody
 */
public class SpiderMovement : MonoBehaviour {

	//how fast it moves
	public float movmentSpeed = 40;
	public float yawSpeed = 5;
	public float pitchSpeed = 5;

	//how quickly it stops
	public float velocityStoping = 10;
	public float rotationStoping = 10;

	//how aggresivly it sticks to the wall
	public float linearCorrection = 300;
	public float angularCorrection = 30;

	public float footRayDistance = 2.0f;
	public float standingHeight = 1.0f;

	public float jumpSpeed = 10;
	public float minJumpTime = 1;
	private bool isJumping = false;

	public Transform head, neck;
	public float maxHeadPitch = 85.0f;
	public float minHeadPitch = -85.0f;

	private Vector3 lastHeadAxis, lastNeckAxis;
	private Vector3[] feetDirections;
	private Vector3 groundCentroid = Vector3.zero, groundNormal = Vector3.zero;
	private Vector3 groundVelocity = Vector3.zero, groundAngularVelocity = Vector3.zero;
	private bool isGrounded = false, hasTraction = false;

	private Rigidbody body;
	private bool paused = false;

	void Start () {
		body = GetComponent<Rigidbody> ();

		InitFeetDirections();

		lastHeadAxis = head.forward;
		lastNeckAxis = neck.forward;

		lockCurser ();
	}

	void Update () {
		if (paused) {
			if (Input.GetMouseButtonDown(0)) {
				OnResume();
			}
		} else if (Input.GetButton("Cancel")) {
			OnPause();
		} else {
			UpdateHead ();
			if (!isJumping && isGrounded && Input.GetButtonDown ("Jump")) {
				Jump ();
			}
		}
	}

	void UpdateHead () {
		//Head and neck MUST be done separately because singularities are a pain in the ass.

		Quaternion groundRotation = Quaternion.AngleAxis(groundAngularVelocity.magnitude * Mathf.Rad2Deg * Time.fixedDeltaTime, groundAngularVelocity);

		lastNeckAxis = groundRotation * lastNeckAxis;
		lastHeadAxis = groundRotation * lastHeadAxis;
		
		lastNeckAxis = transform.InverseTransformVector(lastNeckAxis);
		lastHeadAxis = neck.InverseTransformVector(lastHeadAxis);

		float  yaw  = Quaternion.LookRotation(lastNeckAxis).eulerAngles.y;
		float pitch = Quaternion.LookRotation(lastHeadAxis).eulerAngles.x;
		
		yaw  += Input.GetAxis ("Mouse X") * yawSpeed;
		pitch -= Input.GetAxis ("Mouse Y") * pitchSpeed;

		while (pitch >  180) pitch -= 360;
		while (pitch < -180) pitch += 360;
		pitch = Mathf.Clamp(pitch, minHeadPitch, maxHeadPitch);

		neck.localRotation = Quaternion.Euler(0,yaw,0);
		head.localRotation = Quaternion.Euler(pitch,0,0);

		lastNeckAxis = neck.forward;
		lastHeadAxis = head.forward;
	}

	void FixedUpdate () {
		GetGroundInfo ();
		if (isGrounded) {
			GroundedAngular ();
			if (!isJumping && hasTraction) {
				TractionLinear ();
			} else {
				SlipLinear ();
			}
		} else {
			FallingAngular ();
		}
	}

	void Jump () {
		isJumping = true;
		body.AddForce (head.forward * jumpSpeed, ForceMode.VelocityChange);
		Invoke("EndJump", minJumpTime);
	}

	void EndJump () {
		isJumping = false;
	}

	void TractionLinear () {
		body.useGravity = false;
		Vector3 correctionVelocity = groundCentroid - (transform.position - groundNormal * standingHeight);
		correctionVelocity = Vector3.Project (correctionVelocity, groundNormal) * linearCorrection;

		Vector3 wantedVelocity = Vector3.ProjectOnPlane (neck.forward, groundNormal).normalized * Input.GetAxis ("Vertical");
		wantedVelocity += Vector3.ProjectOnPlane (neck.right, groundNormal).normalized * Input.GetAxis ("Horizontal");
		wantedVelocity = Vector3.ClampMagnitude(wantedVelocity, 1) * movmentSpeed;

		wantedVelocity += correctionVelocity;
		body.AddForce (wantedVelocity - (body.velocity - groundVelocity) * velocityStoping, ForceMode.Acceleration);
	}

	void SlipLinear () {
		body.useGravity = false;
		bool gravDone = false;

		Vector3 wantedVelocity = groundCentroid - (transform.position - groundNormal * standingHeight);
		wantedVelocity = Vector3.Project (wantedVelocity, groundNormal) * linearCorrection;
		wantedVelocity -= (body.velocity - groundVelocity) * velocityStoping;
		wantedVelocity = Vector3.Project (wantedVelocity, groundNormal);

		if (Vector3.Dot (wantedVelocity, groundNormal) > 0) {
			body.AddForce (wantedVelocity, ForceMode.Acceleration);
		} else {
			if (Vector3.Dot (wantedVelocity, Physics.gravity) > 0) {
				Vector3 foo = Vector3.Project (Physics.gravity, wantedVelocity);
				if (wantedVelocity.sqrMagnitude < foo.sqrMagnitude) {
					body.AddForce (wantedVelocity);
				} else {
					body.AddForce (foo);
				}
			} else {
				gravDone = true;
				body.AddForce (Physics.gravity, ForceMode.Acceleration);
			}
		}
		if (!gravDone) {
			body.AddForce (Vector3.ProjectOnPlane(Physics.gravity, groundNormal), ForceMode.Acceleration);
		}
	}

	void GroundedAngular () {
		Vector3 wantedAngularVelocity = Vector3.Cross (transform.up, groundNormal) * angularCorrection;
		wantedAngularVelocity += Vector3.Cross(transform.forward, neck.forward) * angularCorrection;
		body.AddTorque (wantedAngularVelocity - (body.angularVelocity - groundAngularVelocity) * rotationStoping, ForceMode.Acceleration);
	}

	void FallingAngular () {
		body.useGravity = true;
		Vector3 wantedAngularVelocity = Vector3.Cross (transform.up, head.rotation * new Vector3(0.0f, 0.707106781186548f, -0.707106781186548f)) * angularCorrection;
		wantedAngularVelocity += Vector3.Cross(transform.forward, neck.forward) * angularCorrection;
		body.AddTorque (wantedAngularVelocity - body.angularVelocity * rotationStoping, ForceMode.Acceleration);
	}

	bool GetGroundInfo () {
		int numHits = 0;
		Vector3 lastGroundVelocity = groundVelocity;
		Vector3 lastGroundAngularVelocity = groundAngularVelocity;
		groundVelocity = groundAngularVelocity = groundCentroid = groundNormal = Vector3.zero;
		hasTraction = false;

		foreach (Vector3 d in feetDirections) {
			Vector3 origin = transform.position;
			Vector3 direction = transform.rotation * d;

			RaycastHit hitInfo;
			if (Physics.Raycast(origin, direction, out hitInfo, footRayDistance)) {
				if (hitInfo.rigidbody != body) {
					groundCentroid += hitInfo.point;
					groundNormal += hitInfo.normal;
					++numHits;
					if (!isSlippery(hitInfo.collider.material)) {
						hasTraction = true;
						if (hitInfo.rigidbody != null) {
							groundVelocity += hitInfo.rigidbody.GetPointVelocity (transform.position); // suspected to cause jitter when on rotating platforms
							groundAngularVelocity += hitInfo.rigidbody.angularVelocity;
						}
					}
				}
			}
		}
		if (numHits > 0) {
			groundVelocity /= numHits;
			groundAngularVelocity /= numHits;
			groundCentroid /= numHits;
			groundNormal.Normalize();
			isGrounded = true;

			//TODO stop jitter and sliding when on moving platform
			//these lines help a little but not enough
			Vector3 deltaGV = groundVelocity - lastGroundVelocity;
			Vector3 deltaGAV = groundAngularVelocity - lastGroundAngularVelocity;
			body.AddTorque (deltaGAV, ForceMode.VelocityChange);
			body.AddForce (deltaGV, ForceMode.VelocityChange);

			return true;
		} else {
			groundVelocity = groundAngularVelocity = Vector3.zero;
			isGrounded = false;
			return false;
		}
	}

	bool isSlippery (PhysicMaterial mat) {
		return mat.dynamicFriction < 0.01 &&
			mat.staticFriction < 0.01 &&
			mat.frictionCombine == PhysicMaterialCombine.Minimum;
	}

	void OnPause () {
		paused = true;
		lockCurser (false);
	}

	void OnResume () {
		paused = false;
		lockCurser (true);
	}

	void lockCurser (bool isLock=true) {
		Cursor.visible = !isLock;
		Cursor.lockState = isLock ? CursorLockMode.Locked : CursorLockMode.None;
	}

	void InitFeetDirections () {
		List<Vector3> spherePoints = initialize_sphere(4);
		for (int i=0; i<spherePoints.Count; ++i) {
			if (spherePoints[i].y > 0.1f) {
				spherePoints.RemoveAt(i--);
			}
		}
		feetDirections = spherePoints.ToArray();
	}

	//Not my work. Gotten from http://stackoverflow.com/questions/17705621/algorithm-for-a-geodesic-sphere
	void subdivide(Vector3 v1,  Vector3 v2, Vector3 v3, ref List<Vector3> sphere_points, int depth) {
		if(depth == 0) {
			sphere_points.Add(v1);
			sphere_points.Add(v2);
			sphere_points.Add(v3);
			return;
		}
		Vector3 v12 = (v1 + v2).normalized;
		Vector3 v23 = (v2 + v3).normalized;
		Vector3 v31 = (v3 + v1).normalized;
		subdivide(v1, v12, v31, ref sphere_points, depth - 1);
		subdivide(v2, v23, v12, ref sphere_points, depth - 1);
		subdivide(v3, v31, v23, ref sphere_points, depth - 1);
		subdivide(v12, v23, v31, ref sphere_points, depth - 1);
	}

	List<Vector3> initialize_sphere(int depth) {
		List<Vector3> sphere_points = new List<Vector3>();

		float X = 0.525731112119133606f;
		float Z = 0.850650808352039932f;
		Vector3[] vdata = new Vector3[] {
			new Vector3(-X, 0.0f, Z), new Vector3( X, 0.0f, Z ), new Vector3( -X, 0.0f, -Z ), new Vector3( X, 0.0f, -Z ),
			new Vector3( 0.0f, Z, X ), new Vector3( 0.0f, Z, -X ), new Vector3( 0.0f, -Z, X ), new Vector3( 0.0f, -Z, -X ),
			new Vector3( Z, X, 0.0f ), new Vector3( -Z, X, 0.0f ), new Vector3( Z, -X, 0.0f ), new Vector3( -Z, -X, 0.0f )
		};
		int[][] tindices = new int[][] {
			new int[]{0, 4, 1}, new int[]{ 0, 9, 4 }, new int[]{ 9, 5, 4 }, new int[]{ 4, 5, 8 }, new int[]{ 4, 8, 1 },
			new int[]{ 8, 10, 1 }, new int[]{ 8, 3, 10 }, new int[]{ 5, 3, 8 }, new int[]{ 5, 2, 3 }, new int[]{ 2, 7, 3 },
			new int[]{ 7, 10, 3 }, new int[]{ 7, 6, 10 }, new int[]{ 7, 11, 6 }, new int[]{ 11, 0, 6 }, new int[]{ 0, 1, 6 },
			new int[]{ 6, 1, 10 }, new int[]{ 9, 0, 11 }, new int[]{ 9, 11, 2 }, new int[]{ 9, 2, 5 }, new int[]{ 7, 2, 11 }
		};
		for(int i = 0; i < 20; i++)
			subdivide(vdata[tindices[i][0]], vdata[tindices[i][1]], vdata[tindices[i][2]], ref sphere_points, depth);
		return sphere_points;
	}
}