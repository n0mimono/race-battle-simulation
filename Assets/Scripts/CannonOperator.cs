using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CannonOperator : MonoBehaviour {
	public Car    myCar;
	public Cannon cannon;
	public float  rotForce;

	private List<Car> targetList;
	private Car       curTarget;
	private float     curRotForce;
	private float     curAddAngToTgt;

	private Transform cannonTrans;

	private const float PredictiveSeconds = 0.5f;
	private const float FireRange         = 10f;

	private void Start() {
		Initialize ();
	}

	public void Initialize() {
		cannon.Initialize ();

		cannonTrans   = cannon.transform;
		targetList    = GameObject.FindObjectsOfType<Car> ().Where (c => c != myCar).ToList ();

		StartCoroutine (rotateCannon ());

		StartCoroutine (procRotate ());
		StartCoroutine (procFire ());
		StartCoroutine (procTargetting ());
	}

	private IEnumerator rotateCannon() {
		while (true) {
			curRotForce = Mathf.Clamp (curRotForce, -rotForce, rotForce);
			cannon.Rotate (curRotForce, Time.deltaTime);

			yield return null;
		}
	}

	private IEnumerator procRotate() {
		while (curTarget == null) {
			yield return null;
		}

		while (true) {
			curAddAngToTgt = calcAngTo (curTarget);
			curRotForce    = curAddAngToTgt;

			yield return null;
		}
	}

	private float calcAngTo(Car car) {
		Vector3 tgtPos = car.Pos;
		Vector3 tgtDir = car.Dir;
		float   tgtSpd = car.engine.Speed;
		Vector3 prdPos = tgtPos + tgtDir * tgtSpd * PredictiveSeconds;

		Vector3 curPos = cannonTrans.position;
		Vector3 curDir = cannonTrans.forward;
		Vector3 relDir = (tgtPos - curPos).normalized;

		return Driver.Util.ToAngFrom (curDir, relDir);
	}

	private IEnumerator procFire() {

		while (!(cannon.IsFireable && Mathf.Abs (curAddAngToTgt) < FireRange)) {
			yield return null;
		}

		yield return null;
		cannon.Fire ();

		yield return null;
		StartCoroutine (procFire ());
	}

	private IEnumerator procTargetting() {
		yield return null;

		while (true) {
			curTarget = targetList.WhichMin (t => Mathf.Abs( calcAngTo (t)));

			yield return new WaitForSeconds (1f);
		}
	}

}
