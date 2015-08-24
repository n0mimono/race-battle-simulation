using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {

	public Transform platform;
	public Transform barrel;
	public Bullet    bulletPrefab;

	public float     rotateMax;

	private Transform trans;
	private Vector3   curAngles;
	public  Vector3   CurAngles { get { return curAngles; } }

	private const float FireInterval = 5f;
	private bool  isFireable;
	public  bool  IsFireable { get { return isFireable; } }

	public void Initialize() {
		trans     = transform;
		curAngles = trans.eulerAngles;

		isFireable = true;
	}

	public void Rotate(float force, float dt) {
		float angle = force * dt;
		curAngles.y = Mathf.Clamp(curAngles.y + angle, -rotateMax, rotateMax);
		platform.localEulerAngles = curAngles;
	}

	public void Fire() {
		StartCoroutine (fireBullet ());
	}

	private IEnumerator fireBullet() {
		isFireable = false;

		Bullet bullet = Instantiate (bulletPrefab);
		bullet.Initialize (barrel);
		bullet.Fire ();

		yield return new WaitForSeconds(FireInterval);
		isFireable = true;
	}

}
