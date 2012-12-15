using UnityEngine;

static class MyTime
{

	static bool pause = false;

	static public bool Pause {
		get {
			return pause;
		}
		set {
			pause = value;
			Time.timeScale = pause ? 0.0f : 1.0f; // for whatever it's good ...
		}
	}

	static public float deltaTime {
		get {
			return Pause ? 0.0f : Time.deltaTime;
		}
	}

	static public float time {
		get {
			return Time.time;
		}
	}

}
