using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float force;

	private Transform trans;

	public void Initialize(Transform parent) {
		trans = transform;

		trans.SetParent (parent);
		trans.localPosition    = Vector3.zero;
		trans.localEulerAngles = Vector3.zero;
	}

	public void Fire() {
		trans.SetParent (null);

		StartCoroutine (procFire ());
	}

	private IEnumerator procFire() {
		Rigidbody rigidBody = GetComponent<Rigidbody> ();
		rigidBody.AddForce (trans.forward * force);

		yield return null;
	}

}
