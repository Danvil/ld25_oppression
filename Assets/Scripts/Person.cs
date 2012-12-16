using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Person : MonoBehaviour {

	const float SPEED_NORMAL = 0.05f;
	const float SPEED_FAST = 0.20f;
	const float RADIUS = 0.04f;
	const float PERSON_LOOK_RADIUS = 0.50f;
	const float AVOID_STRENGTH = 0.1f;
	const float TARGET_HIT_RANGE = 0.1f;
	const float DEATH_COOLDOWN = 10.0f;
	const float DEATH_FALLTIME = 0.7f;
	const float DEATH_WITNESS_RANGE = 2.0f;
	const float ROTATION_MIX_STRENGTH = 0.3f;
	const float VELOCITY_MIX_STRENGTH = 0.04f;
	
	public GameObject pfMarkerPolice;
	public GameObject pfMarkerRebels;

	public int hitpointsMax = 10;
	public int damage = 1;
	public Faction faction;
	
	public int HitpointsCurrent { get; private set; }
	public List<Person> PersonsInRange { get; private set; }
	public Person ClosestEnemy { get; private set; }
	public int ThreatLevel { get; private set; }
	public Vector3 Velocity { get; private set; }
	
	public Person FollowTarget { get; set; }
	public bool IsFleeing { get; set; }
	public Person AttackTarget { get; set; }
	public List<Vector3> AdditionalForces { get; set; }
	public bool IsFast { get; set; }
		
	public Squad Squad { get; set; }
	public bool IsSquadLeader { get; private set; }
	GameObject squadMarker;
	
	public void MakeSquad() {
		if(IsSquadLeader)
			return;
		gameObject.AddComponent<Squad>();
		Squad = GetComponent<Squad>();
		Squad.Add(this);
		IsSquadLeader = true;
		squadMarker = (GameObject)Instantiate(faction == Faction.Police ? pfMarkerPolice : pfMarkerRebels);
		squadMarker.transform.parent = this.transform;
		squadMarker.transform.localPosition = Vector3.zero;
		squadMarker.transform.localRotation = Quaternion.identity;
		squadMarker.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
	}
	
	bool isDead = false;
	float deathTime = 0.0f;
	Person murderer;	
	float attackCooldown;
	RandomGoalPicker randomGoalPicker;
	
	// Use this for initialization
	void Start () {
		HitpointsCurrent = hitpointsMax;
		
		PersonsInRange = new List<Person>();
		ClosestEnemy = null;
		ThreatLevel = 0;
		Velocity = Vector3.zero;
		
		randomGoalPicker = GetComponent<RandomGoalPicker>();
		if(randomGoalPicker)
			randomGoalPicker.IsEnabled = true;

		FollowTarget = null;
		IsFleeing = false;
		AttackTarget = null;
		AdditionalForces = new List<Vector3>();
		IsFast = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(randomGoalPicker)
			randomGoalPicker.IsEnabled = (!FollowTarget && !IsFleeing);
		
		// get persons in range
		updatePersonsInRange();
		
		// bleeding
		HitpointsCurrent = MoreMath.Clamp(HitpointsCurrent, 0, hitpointsMax);
		float bleed_freq = 1.0f - (float)HitpointsCurrent / (float)hitpointsMax;
		if(MoreMath.CheckOccurence(bleed_freq)) {
			float r = 0.01f * (float)(hitpointsMax - HitpointsCurrent);
			Globals.DecalManager.CreateBlood(transform.position, r);
		}
		
		if(isDead || HitpointsCurrent <= 0) {
			if(!isDead) {
				die();
			}
			deathTime += MyTime.deltaTime;
			float angle = MoreMath.Clamp(deathTime / DEATH_FALLTIME, 0.0f, 1.0f) * 90.0f;
			this.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0,0,1));
			float p = deathTime / DEATH_COOLDOWN;
			this.transform.position = new Vector3(this.transform.position.x, 0.05f * (1.0f - 2.0f*p), this.transform.position.z);
			if(deathTime > DEATH_COOLDOWN) {
				Globals.People.Kill(this);
			}
			return;
		}
		
		if(!attack()) move();
	}
	
	void die() {
		audio.PlayOneShot(Globals.People.RandomDeathAudio);
		isDead = true;
		foreach(Building b in Globals.City.GetBuildingsInRange(transform.position, DEATH_WITNESS_RANGE)) {
			b.WitnessDeath(faction, murderer.faction);
		}
	}
	
	bool attack() {
		attackCooldown -= MyTime.deltaTime;
		if(attackCooldown > 0) {
			return false;
		}
		if(!AttackTarget)
			return false;
		float distToTarget = (AttackTarget.transform.position - this.transform.position).magnitude;
		if(distToTarget > TARGET_HIT_RANGE) {
			return false;
		}
		if(AttackTarget.HitpointsCurrent > 0) {
			AttackTarget.HitpointsCurrent -= damage;
			if(AttackTarget.HitpointsCurrent <= 0) {
				AttackTarget.murderer = this;
			}
		}
		audio.PlayOneShot(Globals.People.RandomHitAudio);
		Globals.DecalManager.CreateBlood(transform.position, 0.03f*(float)damage);
		attackCooldown = Random.Range(1.2f, 1.9f);
		return true;
	}
	
	void move() {
		Vector3 moveLevel = computeAvoidLevel();
		Debug.DrawRay(this.transform.position, 2.0f*moveLevel, Color.blue);
		
		Vector3 moveAvoid = computeAvoidOther();
		Debug.DrawRay(this.transform.position, 2.0f*moveAvoid, Color.yellow);
		
		Vector3 moveFollow = computeFollow();
		Debug.DrawRay(this.transform.position, 2.0f*moveFollow, Color.red);

		Vector3 moveRndGoal = (randomGoalPicker ? randomGoalPicker.Force : Vector3.zero);
		Debug.DrawRay(this.transform.position, 2.0f*moveRndGoal, Color.green);
		
		Vector3 moveOther = Vector3.zero;
		foreach(Vector3 v in AdditionalForces) {
			moveOther += v;
		}
		AdditionalForces.Clear();
		Debug.DrawRay(this.transform.position, 2.0f*moveOther, Color.white);
		
		Vector3 move = moveLevel + moveAvoid + moveFollow + moveRndGoal + moveOther;
		
		// some randomness
		move += 0.05f * MoreMath.RandomInsideUnitCircleXZ;
		// limit max velocity
		float speed = IsFast ? SPEED_NORMAL : SPEED_FAST;
		move *= speed / move.magnitude;
		Velocity = MoreMath.Interpolate(Velocity, move, VELOCITY_MIX_STRENGTH);
		
		// compute new position
		Vector3 npos = transform.position + MyTime.deltaTime * Velocity;
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
		}
	}
	
//	bool isEnemy(Person x) {
//		return ((faction == Faction.Neutral || faction == Faction.Police) && x.faction != faction)
//				|| (faction == Faction.Rebel && x.faction == Faction.Police);
//	}
	
	void updatePersonsInRange() {
		this.PersonsInRange = Globals.People.GetInRange(this, PERSON_LOOK_RADIUS).ToList();
		this.ClosestEnemy = null;
		int cnt_total = 0;
		int cnt_balance = 0;
		float closest_dist = 1000.0f;
		foreach(Person x in this.PersonsInRange) {
			if(x.isDead)
				continue;
			cnt_total ++;
			if(x.faction != faction) {
				cnt_balance --;
				float dist = (x.transform.position - this.transform.position).magnitude;
				if(dist < closest_dist) {
					closest_dist = dist;
					this.ClosestEnemy = x;
				}
			}
			else {
				cnt_balance ++;
			}
		}
		this.ThreatLevel = cnt_balance;
	}
	
	float avoidFalloff(float d, float d_min) {
		float z = Mathf.Max(d/d_min, 0.4f);
		return 1.0f / (z*z);
	}
	
	Vector3 computeFollow() {
		if(FollowTarget) {
			return (IsFleeing ? -1.0f : +1.0f) * (FollowTarget.transform.position - this.transform.position).normalized;
		}
		else {
			return Vector3.zero;
		}
	}
	
	Vector3 computeAvoidOther() {
		Vector3 force = Vector3.zero;
		float d_min = 2.0f * RADIUS;
		foreach(Person x in PersonsInRange) {
			Vector3 delta = x.transform.position - transform.position;
			force -= AVOID_STRENGTH * avoidFalloff(delta.magnitude, d_min) * delta.normalized;
		}
		return force;
	}
	
	Vector3 computeAvoidLevel() {
		return new Vector3(0,0,0);
	}
	
}
