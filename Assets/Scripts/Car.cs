using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

	[System.Serializable]
	public class Engine {
		public float accel;
		public float decel;
		public float maxSpeed;
		public float Speed { set; get; }

		public void ProcAccel(float dt) {
			Speed = Mathf.Min(Speed + accel * dt, maxSpeed);
		}
		public void ProcDecel(float dt) {
			Speed = Mathf.Max (0f, Speed - decel * dt);
		}
	}
	public Engine engine;

	[System.Serializable]
	public class Handle {
		public float accel;
		public float rotMax;
		public float toWheel;
		public float Rot { set; get; }

		public void ProcRot(float dirSign, float dt) {
			Rot = Rot + dirSign * accel * dt;
			Rot = Mathf.Sign (Rot) * Mathf.Min (Mathf.Abs(Rot), rotMax);
		}
	}
	public Handle handle;

	private Transform trans;
	private Vector3 forward;

	public  Vector3 Dir { get { return forward; } }
	public  Vector3 Pos { get { return trans.position; } }
	public  float   Ang { get { return handle.Rot; } }

	public void Initialize() {
		trans = transform;

		engine.Speed = 0f;
		handle.Rot   = 0f;
		forward      = trans.forward;

		StartCoroutine (procDecel ());
		StartCoroutine (move ());
	}

	private IEnumerator move() {
		while (true) {
			float wheelRot = handle.Rot * handle.toWheel * Time.deltaTime;;

			Quaternion qy  = Quaternion.AngleAxis (wheelRot, Vector3.up);
			forward        = qy * forward;

			Quaternion drift = Quaternion.AngleAxis (handle.Rot, Vector3.up);
			trans.forward    = drift * forward;
			trans.position  += forward * engine.Speed * Time.deltaTime;

			yield return null;
		}
	}

	private IEnumerator procDecel() {
		while (true) {
			engine.ProcDecel (Time.deltaTime);
			yield return null;
		}
	}

	public void ForceGasPedal(float dt) {
		engine.ProcAccel (Time.deltaTime);
	}

	public void ForceHandle(bool isRight, float dt) {
		float dir = isRight ? 1 : -1;
		handle.ProcRot (dir, dt);
	}

}
