using UnityEngine;
using System.Collections;

public class Circle : MonoBehaviour {

	private float   radius;
	private Vector3 center;

	public float   Radius { get { return radius; } }
	public Vector3 Center { get { return center; } }
	public float   CachedDist { get; set; }

	private void Awake() {
		radius     = transform.localScale.x / 2f;
		center     = transform.position;
		CachedDist = 0f;
	}

}
