using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {

	const float SPEED_NORMAL = 0.10f;
	const float SPEED_FAST = 0.30f;
	const float GOAL_REACHED_TOLERANCE = 0.01f;
	const float RADIUS = 0.04f;
	const float AVOID_RADIUS = 0.35f;
	const float AVOID_STRENGTH_OWN = 0.1f;
	const float AVOID_STRENGTH_OTHER = 0.5f;
	const float AVOID_STRENGTH_ATTACK = 0.02f;
	const float CHECK_TARGET_RADIUS = 0.4f;
	const float TARGET_HIT_RANGE = 0.1f;
	const float DEATH_COOLDOWN = 5.0f;
	const float DEATH_FALLTIME = 1.0f;
	const float ROTATION_MIX_STRENGTH =0.9f;
	
	int hitpoints = 10;
	bool isDead = false;
	float deathTime = 0.0f;
	Vector3 death_axis;
	
	Vector3 goal;
	float goalWaitTime;
	float goalFollowTime;
	Person target;
	float attackCooldown;
	
	bool isStatic;
	
	public int faction;
	
	public int thread_level;
	
	public const int FACTION_REBEL = 0;
	public const int FACTION_POLICE = 1;
	
	// Use this for initialization
	void Start () {
		isStatic = true;
		goalWaitTime = 0.0f;
		goalFollowTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(isDead || hitpoints <= 0) {
			if(!isDead) {
				audio.PlayOneShot(Globals.People.RandomDeathAudio);
				isDead = true;
				float deathangle = 0.0f; Random.Range(0.0f, 360.0f);
				death_axis = new Vector3(Mathf.Sin(deathangle), 0.0f, Mathf.Cos(deathangle));
			}
			deathTime += MyTime.deltaTime;
			float angle = MoreMath.Clamp(deathTime / DEATH_FALLTIME, 0.0f, 1.0f) * 90.0f;
			this.transform.rotation = Quaternion.AngleAxis(angle, death_axis);
			float p = deathTime / DEATH_COOLDOWN;
			this.transform.position = new Vector3(this.transform.position.x, 0.05f * (1.0f - 2.0f*p), this.transform.position.z);
			if(deathTime > DEATH_COOLDOWN) {
				Globals.People.Kill(this);
			}
			return;
		}
		checkTargets();
		goalFollowTime -= MyTime.deltaTime;
		if(goalFollowTime < 0.0f || isGoalReached() || isStatic) {
			isStatic = true;
			goalWaitTime -= MyTime.deltaTime;
			if(goalWaitTime < 0.0f) {
				setNewGoal();
			}
		}
		else {
			if(isStatic) {
				return;
			}
			if(isFleeing()) {
				move();
			}
			else {
				if(target) {
					goal = target.transform.position;
				}
				if(!attack()) move();
			}
		}
	}
	
	bool isFleeing() {
		return faction != FACTION_POLICE && hitpoints <= 3;
	}
	
	bool attack() {
		attackCooldown -= MyTime.deltaTime;
		if(attackCooldown > 0) {
			return false;
		}
		if(!target)
			return false;
		float distToTarget = (target.transform.position - this.transform.position).magnitude;
		if(distToTarget > TARGET_HIT_RANGE) {
			return false;
		}
		if(faction == FACTION_REBEL) {
			target.hitpoints -= 2;
		}
		else if(faction == FACTION_POLICE) {
			target.hitpoints -= 4;
		}
		else {
			target.hitpoints -= 1;
		}
		audio.PlayOneShot(Globals.People.RandomHitAudio);
		attackCooldown = Random.Range(1.2f, 1.9f);
		return true;
	}
	
	void move() {
		bool is_fleeing = isFleeing();
		Vector3 moveLevel = computeAvoidLevel();
		Debug.DrawRay(this.transform.position, 2.0f*moveLevel, Color.blue);
		Vector3 move = moveLevel;
		if(is_fleeing) {
			Vector3 moveFlee = computeFlee();
			Debug.DrawRay(this.transform.position, 2.0f*moveFlee, Color.red);
			move += moveFlee;
		}
		else {
			Vector3 moveFollow = computeGoalFollow();
			Debug.DrawRay(this.transform.position, 2.0f*moveFollow, Color.yellow);
			Vector3 moveAvoid = computeAvoidOther();
			Debug.DrawRay(this.transform.position, 2.0f*moveAvoid, Color.green);
			move += moveFollow + moveAvoid;
		}
		// some randomness
		move += 0.05f * MoreMath.RandomInsideUnitCircleXZ;
		// limit max velocity
		float mag = move.magnitude;
		float speed = (target == null && !is_fleeing ? SPEED_NORMAL : SPEED_FAST);
		if(mag > speed) {
			move *= speed / mag;
		}
		// compute new position
		Vector3 npos = transform.position + MyTime.deltaTime * move;
		npos = new Vector3(npos.x, 0, npos.z);
		if(!Globals.City.IsBlocked(npos)) {
			transform.position = npos;
			// compute new rotation
			float angle_old = MoreMath.VectorAngle(transform.localRotation * Vector3.right);
			float angle_new = MoreMath.VectorAngle(move.normalized);
			float angle_final = MoreMath.SlerpAngle(angle_old, angle_new, ROTATION_MIX_STRENGTH * MyTime.deltaTime);
			transform.localRotation = MoreMath.RotAngle(-angle_final);
		}
		else {
			// new position is not possible
			if(!is_fleeing)
				isStatic = true;
		}
	}
	
	void checkTargets() {
		int cnt_total = 0;
		int cnt_balance = 0;
		Person closest_target = null;
		float closest_dist = 1000.0f;
		foreach(Person x in Globals.People.GetInRange(this, CHECK_TARGET_RADIUS)) {
			if(x.isDead)
				continue;
			cnt_total ++;
			if(x.faction != this.faction) {
				cnt_balance --;
				float dist = (x.transform.position - transform.position).magnitude;
				if(dist < closest_dist) {
					closest_dist = dist;
					closest_target = x;
				}
			}
			else {
				cnt_balance ++;
			}
		}
		if(cnt_balance > 2 && closest_target) {
			target = closest_target;
		}
		thread_level = cnt_balance;
	}
	
	void setNewGoal() {
		for(int i=0; i<3; i++) {
			// create new random goal
			Vector3 ngoal = MoreMath.RandomInsideBox(Globals.City.SizeMin, Globals.City.SizeMax);
			// check if goal is reachable
			if(!Globals.City.IsBlocked(ngoal)) {
				goal = ngoal;
				isStatic = false;
				break;
			}
		}
		goalWaitTime = Random.Range(0.0f, 2.0f);
		goalFollowTime = Random.Range(3.0f, 5.0f);
		isStatic = false;
	}

	bool isGoalReached() {
		return (transform.position - goal).magnitude < GOAL_REACHED_TOLERANCE;
	}
	
	Vector3 computeGoalFollow() {
		return SPEED_NORMAL * (goal - transform.position).normalized;
	}

	float avoidFalloff(float d, float d_min) {
		float z = Mathf.Max(d/d_min, 0.4f);
		return 1.0f / (z*z);
	}

	Vector3 computeAvoidOther() {
		Vector3 force = Vector3.zero;
		float d_min = 2.0f * RADIUS;
		foreach(Person x in Globals.People.GetInRange(this, AVOID_RADIUS)) {
			Vector3 delta = x.transform.position - transform.position;
			float str;
			if(target) {
				str = AVOID_STRENGTH_ATTACK;
			}
			else {
				str = (x.faction != this.faction ? AVOID_STRENGTH_OTHER : AVOID_STRENGTH_OWN);
			}
			force -= str * avoidFalloff(delta.magnitude, d_min) * delta.normalized;
		}
		return force;
	}
	
	Vector3 computeFlee() {
		Vector3 force = Vector3.zero;
		float d_min = 2.0f * RADIUS;
		foreach(Person x in Globals.People.GetInRange(this, AVOID_RADIUS)) {
			Vector3 delta = x.transform.position - transform.position;
			float str = (x.faction != this.faction ? 2.0f * AVOID_STRENGTH_OTHER : 0.5f*AVOID_STRENGTH_OWN);
			force -= str * avoidFalloff(delta.magnitude, d_min) * delta.normalized;
		}
		return force;
	}
	
	Vector3 computeAvoidLevel() {
		return new Vector3(0,0,0);
	}
	
}
