using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Driver : MonoBehaviour {
	public Map map;
	public Car car;

	public bool isAutoDriver;

	private const float MinHandleRot = 2f;
	public List<Circle> Obstacles { get { return map.obstacles; } }

	private List<DriverAssistant> assistants;

	private void Start() {
		Initialize ();
	}

	public void Initialize() {
		car.Initialize ();

		if (isAutoDriver) {
			startAutoDrive ();
		}
 	}

	private void startAutoDrive() {
		assistants = new List<DriverAssistant> ();
		assistants.Add (new ObstacleAvoider ());
		assistants.Add (new StraightFowarder ());
		assistants.Add (new BasicRacer ());
		assistants.ForEach (a => a.Initialize (this));

		StartCoroutine (drive ());
	}

	private IEnumerator drive() {
		while (true) {
			Vector3 curDir = car.Dir;
			Vector3 tgtDir = curTargetDir ();
			float   tgtAng = toAngFrom (curDir, tgtDir);
			float   addAng = tgtAng - car.Ang;

			if (Mathf.Abs (addAng) > MinHandleRot) {
				car.ForceHandle (addAng > 0f, Time.deltaTime);
			}

			car.ForceGasPedal (Time.deltaTime);
			yield return null;
		}
	}

	private float toAngFrom(Vector3 curDir, Vector3 tgtDir) {
		Quaternion aq = Quaternion.FromToRotation (curDir, tgtDir);
		return toSignedAng(aq.eulerAngles.y);
	}

	private static float toSignedAng(float ang) {
		return ang > 180f ? ang - 360f : ang;
	}

	private Vector3 curTargetDir() {
		return DriverAssistant.TotalDirection(assistants);
	}

	public struct DirectionInfo {
		public Vector3 dir;
		public float   weight;
		public DirectionInfo(Vector3 dir, float weight) {
			this.dir    = dir;
			this.weight = weight;
		}
	}
	public class DriverAssistant {
		public Driver driver;
		public virtual void Initialize (Driver driver) {
			this.driver = driver;
		}
		public virtual DirectionInfo CurTargetDir() {
			return DefaultTargetDir ();
		}
		public DirectionInfo DefaultTargetDir() {
			return new DirectionInfo (Vector3.forward, 0f);
		}

		public static Vector3 TotalDirection(List<DriverAssistant> assists) {
			Vector3 sumDir    = Vector3.zero;
			float   sumWeight = 0f;
			foreach (DriverAssistant assist in assists) {
				DirectionInfo info = assist.CurTargetDir ();

				info.weight -= Mathf.Max(0f, sumWeight + info.weight - 1f);
				sumDir      += info.dir * info.weight;
				sumWeight   += info.weight;
			}

			return sumDir.normalized;
		}
	}
	public class ObstacleAvoider : DriverAssistant {
		public float outOfRangeThreshold = 50f;
		public float coefPriorityWeight  = 40f;
		public float dotOffset           = 0.1f;

		public override void Initialize (Driver driver) {
			base.Initialize (driver);
		}
		public override DirectionInfo CurTargetDir() {
			return calcTargetDir (); // todo: i want to use cache scheme.
		}

		public DirectionInfo calcTargetDir() {
			Car          car       = driver.car;
			List<Circle> obstacles = driver.Obstacles;

			DirectionInfo info = obstacles
				.Where (c => {
					c.CachedDist = Vector3.Distance (car.Pos, c.Center) - c.Radius;
					return c.CachedDist < outOfRangeThreshold;
				})
				.Aggregate (new DirectionInfo (Vector3.zero, 0f), (d, c) => {
					Vector3 relPos = car.Pos - c.Center;
					Vector3 tgtDir = relPos.normalized;
					float   dot    = Mathf.Max(0f, dotOffset - Vector3.Dot (tgtDir, car.Dir));
					float   weight = dot * coefPriorityWeight / c.CachedDist;

					return new DirectionInfo(d.dir + tgtDir, d.weight + weight);
				});
			info.dir = info.dir.normalized;
			return info.weight > 0.01f ? info : DefaultTargetDir();
		}

	}
	public class StraightFowarder : DriverAssistant {
		public override void Initialize (Driver driver) {
			base.Initialize (driver);
		}
		public override DirectionInfo CurTargetDir() {
			return new DirectionInfo (driver.car.Dir, 0.1f);
		}
	}
	public class BasicRacer : DriverAssistant {
		public override void Initialize (Driver driver) {
			base.Initialize (driver);
		}
		public override DirectionInfo CurTargetDir() {
			return new DirectionInfo (driver.car.Dir, 1f);
		}
	}

}
