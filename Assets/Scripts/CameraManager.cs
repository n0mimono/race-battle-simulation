using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	public Car   target;
	public float offset;
	public float height; 

	private IEnumerator lateRoutine;

	private void Start () {
		lateRoutine = chase ();
	}

	private void LateUpdate() {
		if (lateRoutine != null) {
			lateRoutine.MoveNext ();
		}
	}

	private IEnumerator chase() {
		Transform trans = transform;

		while (true) {
			Vector3 pos = target.Pos - target.Dir * offset;
			pos.y = height;

			trans.position = pos;
			trans.LookAt (target.Pos);

			yield return null;
		}
	}

}
