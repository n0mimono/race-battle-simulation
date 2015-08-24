using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CalcCacheManager {

	private static CalcCacheManager instance;
	public static CalcCacheManager Instance { 
		get {  
			if (instance == null) {
				instance = new CalcCacheManager ();
			}
			return instance;
		}
	}

	private struct CachedObject {
		public System.Object obj;
		public float  expireTime;
	}
	Dictionary<System.Object, CachedObject> cacheDict = new Dictionary<System.Object, CachedObject>();

	public T GetValue<T>(System.Object key, float validDuration, System.Func<T> func) {
		if (cacheDict.ContainsKey (key) && Time.time < cacheDict [key].expireTime) {
			return (T)cacheDict [key].obj;
		}

		CachedObject cache = new CachedObject ();
		cache.obj = func ();
		cache.expireTime = Time.time + validDuration;

		cacheDict [key] = cache;
		return (T)cache.obj;
	}

}

