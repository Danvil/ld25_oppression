using UnityEngine;
using System.Collections;

public class Rebel : MonoBehaviour {
	
	const float MAX_SPEED = 0.1f;
	const float GOAL_REACHED_TOLERANCE = 0.01f;
	const float RADIUS = 0.1f;
	const float AVOID_RADIUS = 0.5f;
	const float AVOID_STRENGTH = 0.5f;
	
	Vector3 goal;
	float goalWaitTime;
	float goalFollowTime;
	
	// Use this for initialization
	void Start () {
		setNewGoal();
	}
	
	// Update is called once per frame
	void Update () {
		goalFollowTime -= MyTime.deltaTime;
		if(goalFollowTime < 0.0f) {
			setNewGoal();
		}
		else if(isGoalReached()) {
			goalWaitTime -= MyTime.deltaTime;
			if(goalWaitTime < 0.0f) {
				setNewGoal();
			}
		}
		else {
			Vector3 moveFollow = computeGoalFollow();
			Vector3 moveAvoid = computeAvoidOther();
			Vector3 moveLevel = computeAvoidLevel();
			Debug.DrawRay(this.transform.position, moveLevel, Color.blue);
			Vector3 move = moveFollow + moveAvoid + moveLevel;
			// some randomness
			move += 0.05f * MoreMath.RandomInsideUnitCircleXZ;
			// limit max velocity
			float mag = move.magnitude;
			if(mag > MAX_SPEED) {
				move *= MAX_SPEED / mag;
			}
			// compute new position
			transform.position += MyTime.deltaTime * move;
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		}
	}
	
	void setNewGoal() {
		goal = new Vector3(
			Random.Range(Globals.City.SizeMin.x,Globals.City.SizeMax.x),
			0.0f,
			Random.Range(Globals.City.SizeMin.y,Globals.City.SizeMax.y));
		goalWaitTime = Random.Range(1.0f, 3.0f);
		goalFollowTime = 5.0f;
	}

	bool isGoalReached() {
		return (transform.position - goal).magnitude < GOAL_REACHED_TOLERANCE;
	}
	
	Vector3 computeGoalFollow() {
		return MAX_SPEED * (goal - transform.position).normalized;
	}

	float avoidFalloff(float d, float d_min) {
		float z = Mathf.Max(d/d_min, 0.4f);
		return 1.0f / (z*z);
	}

	Vector3 computeAvoidOther() {
		Vector3 force = Vector3.zero;
		float d_min = 2.0f * RADIUS;
		foreach(GameObject x in Globals.People.GetInRange(gameObject, AVOID_RADIUS)) {
			Vector3 delta = x.transform.position - transform.position;
			force -= avoidFalloff(delta.magnitude, d_min) * delta.normalized;
		}
		return AVOID_STRENGTH * force;
	}
	
	Vector3 computeAvoidLevel() {
		return new Vector3(0,0,0);
	}
	
}
