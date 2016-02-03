using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlayerMovement {
	[System.Serializable]
	public class Feet {

		public float length = 0.7f;
		public bool debug = false;

		private Vector3[] directions;

		[System.NonSerialized] private Transform transform;
		[System.NonSerialized] private Ground ground;

		public void Init (Transform t, Ground g) {
			transform = t;
			ground = g;

			List<Vector3> spherePoints = initialize_sphere(4);
			for (int i=0; i<spherePoints.Count; ++i) {
				if (spherePoints[i].y > 0.1f) {
					spherePoints.RemoveAt(i--);
				}
			}
			directions = spherePoints.ToArray();
		}

		public void Casts () {
			foreach (Vector3 d in directions) {
				RaycastHit hit;
				if (Physics.Raycast (transform.position, transform.rotation * d, out hit, length, ~0, QueryTriggerInteraction.Ignore)) {
					if (hit.rigidbody != transform) {
						ground.ProcessHit (hit);
					}
					if (debug) Debug.DrawRay (transform.position, transform.rotation * d, Color.green);
				} else {
					if (debug) Debug.DrawRay (transform.position, transform.rotation * d, Color.red);
				}
			}
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
}