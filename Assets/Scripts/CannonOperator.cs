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

	private Transform cannonTrans;
	private bool      isTargettting;

	private const float PredictiveSeconds = 0.5f;

	private void Start() {
		Initialize ();
	}

	public void Initialize() {
		cannon.Initialize ();

		cannonTrans   = cannon.transform;
		targetList    = GameObject.FindObjectsOfType<Car> ().Where (c => c != myCar).ToList ();
		isTargettting = true;

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
		while (!isTargettting) {

			Vector3 tgtPos = curTarget.Pos;
			Vector3 tgtDir = curTarget.Dir;
			float   tgtSpd = curTarget.engine.Speed;
			Vector3 prdPos = tgtPos + tgtDir * tgtSpd * PredictiveSeconds;

			Vector3 curPos = cannonTrans.position;
			Vector3 curDir = cannonTrans.forward;
			Vector3 relDir = (tgtPos - curPos).normalized;
			yield return null;

			curRotForce = Driver.Util.ToAngFrom (curDir, relDir);
			yield return null;
		}

		while (!isTargettting) {
			yield return null;
		}

		yield return null;
		StartCoroutine (procRotate ());
	}

	private IEnumerator procFire() {

		while (!cannon.IsFireable) {
			yield return null;
		}

		yield return null;
		cannon.Fire ();

		yield return null;
		StartCoroutine (procFire ());
	}

	private IEnumerator procTargetting() {
		while (isTargettting) {
			curTarget     = targetList [(int)(Random.value * targetList.Count)];
			isTargettting = false;

			yield return null;
		}

		while (!isTargettting) {
			yield return null;
		}

		yield return null;
		StartCoroutine (procTargetting ());
	}

	private void startTargetting() {
		isTargettting = true;
	}


}
