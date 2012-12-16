using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Person : MonoBehaviour {

	const float SPEED_NORMAL = 0.12f;
	const float SPEED_FAST = 0.27f;
	const float RADIUS = 0.04f;
	const float PERSON_LOOK_RADIUS = 0.65f;
	const float AVOID_OTHER_STRENGTH = 0.2f;
	const float AVOID_LEVEL_STRENGTH = 1.3f;
	const float TARGET_HIT_RANGE = 0.1f;
	const float DEATH_COOLDOWN = 10.0f;
	const float DEATH_FALLTIME = 0.7f;
	const float BUILDING_RANGE = 5.0f;
	const float ROTATION_MIX_STRENGTH = 0.3f;
	const float VELOCITY_MIX_STRENGTH = 0.10f;
	const float ATTACK_COOLDOWN = 1.6f;
	const float UPDATE_IN_RANGE_COOLDOWN = 0.597f;
	const bool RENDER_GIZMOS = true;
	
	public GameObject pfMarkerPolice;
	public GameObject pfMarkerRebels;

	public int hitpointsMax = 10;
	public int damage = 1;
	public Faction faction;
	
	public int HitpointsCurrent { get; private set; }
	public List<Building> BuildingsInRange { get; private set; }
	public List<Person> PersonsInRange { get; private set; }
	public int ThreatLevel { get; private set; }
	public Vector3 Velocity { get; private set; }
	public bool IsDead { get; private set; }
	public float DeathTime { get; private set; }
	
	public bool IsUnmovable { get; set; }
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
		Squad.Leader = this;
		Squad.Add(this);
		IsSquadLeader = true;
		squadMarker = (GameObject)Instantiate(faction == Faction.Police ? pfMarkerPolice : pfMarkerRebels);
		squadMarker.transform.parent = this.transform;
		squadMarker.transform.localPosition = Vector3.zero;
		squadMarker.transform.localRotation = Quaternion.identity;
		squadMarker.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
	}
	
	Person murderer = null;	
	float attackCooldown = 0.0f;
	RandomGoalPicker randomGoalPicker;
	float updateInRangeCooldown = 0.0f;
	
	public void SetEnableRandomGoals(bool q) {
		if(randomGoalPicker)
			randomGoalPicker.IsEnabled = q;
	}		
	
	// Use this for initialization
	void Start () {	
		HitpointsCurrent = hitpointsMax;
		
		PersonsInRange = new List<Person>();
		BuildingsInRange = new List<Building>();
		ThreatLevel = 0;
		Velocity = Vector3.zero;
		IsDead = false;
		DeathTime = 0.0f;
	
		IsUnmovable = false;
		FollowTarget = null;
		IsFleeing = false;
		AttackTarget = null;
		AdditionalForces = new List<Vector3>();
		IsFast = false;
		
		randomGoalPicker = GetComponent<RandomGoalPicker>();
		SetEnableRandomGoals(false);
		updateInRangeCooldown = Random.Range(0.0f, UPDATE_IN_RANGE_COOLDOWN);

	}
	
	// Update is called once per frame
	void Update () {
		updateInRangeCooldown -= MyTime.deltaTime;
		if(updateInRangeCooldown < 0.0f) {
			updateInRangeCooldown = UPDATE_IN_RANGE_COOLDOWN;
			// update in range not every frame
			BuildingsInRange = Globals.City.GetBuildingsInRange(transform.position, BUILDING_RANGE).ToList();	
			updatePersonsInRange();
		}
		else {
			// purge dead
			PersonsInRange = (from x in PersonsInRange where !x.IsDead select x).ToList();
		}
		
		// bleeding
		HitpointsCurrent = MoreMath.Clamp(HitpointsCurrent, 0, hitpointsMax);
		float bleed_freq = 1.0f - (float)HitpointsCurrent / (float)hitpointsMax;
		if(MoreMath.CheckOccurence(bleed_freq)) {
			float r = 0.01f * (float)(hitpointsMax - HitpointsCurrent);
			Globals.DecalManager.CreateBlood(transform.position, r);
		}
		
		// death
		if(IsDead || HitpointsCurrent <= 0) {
			if(!IsDead) {
				die();
			}
			DeathTime += MyTime.deltaTime;
			float angle = MoreMath.Clamp(DeathTime / DEATH_FALLTIME, 0.0f, 1.0f) * 90.0f;
			this.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0,0,1));
			float p = DeathTime / DEATH_COOLDOWN;
			this.transform.position = new Vector3(this.transform.position.x, 0.05f * (1.0f - 2.0f*p), this.transform.position.z);
			if(DeathTime > DEATH_COOLDOWN) {
				Globals.People.Kill(this);
			}
			return;
		}
		
		// movement and attack
		if(!attack() && attackCooldown <= 0) move();
	}
	
	void die() {
		audio.PlayOneShot(Globals.People.RandomDeathAudio);
		IsDead = true;
		foreach(Building b in BuildingsInRange) {
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
		Globals.DecalManager.CreateBlood(AttackTarget.transform.position, 0.03f*(float)damage);
		attackCooldown = ATTACK_COOLDOWN + Random.Range(-0.3f, +0.3f);
		return true;
	}
	
	void move() {
		Vector3 moveLevel = computeAvoidLevel();
		if(RENDER_GIZMOS) Debug.DrawRay(this.transform.position + new Vector3(0,0.05f,0), moveLevel, Color.blue);
		
		Vector3 moveAvoid = computeAvoidOther();
		if(RENDER_GIZMOS) Debug.DrawRay(this.transform.position + new Vector3(0,0.05f,0), moveAvoid, Color.yellow);
		
		Vector3 moveFollow = computeFollow();
		if(RENDER_GIZMOS) Debug.DrawRay(this.transform.position + new Vector3(0,0.05f,0), moveFollow, Color.red);

		Vector3 moveRndGoal = (randomGoalPicker ? randomGoalPicker.Force : Vector3.zero);
		if(RENDER_GIZMOS) Debug.DrawRay(this.transform.position + new Vector3(0,0.05f,0), moveRndGoal, Color.green);
		
		Vector3 moveOther = Vector3.zero;
		foreach(Vector3 v in AdditionalForces) {
			moveOther += v;
		}
		AdditionalForces.Clear();
		if(RENDER_GIZMOS) Debug.DrawRay(this.transform.position + new Vector3(0,0.05f,0), moveOther, Color.white);
		
		Vector3 move = moveFollow + moveRndGoal + moveOther;
		if(!IsUnmovable) {
			move += moveLevel + moveAvoid;
			// some randomness
			move += 0.10f * MoreMath.RandomInsideUnitCircleXZ;
		}
		
		// limit max velocity
		float speed = IsFast ? SPEED_FAST : SPEED_NORMAL;
		float mag = move.magnitude;
		if(mag > 0.01f) {
			move *= speed / mag;
		}
		else {
			move = Vector3.zero;
		}
		Velocity = MoreMath.Interpolate(Velocity, move, VELOCITY_MIX_STRENGTH);
		Velocity = new Vector3(Velocity.x, 0.0f, Velocity.z);
		
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
			//Debug.Log("Blocked");
			// new position is not possible
		}
	}
	
	bool isThreat(Faction f) {
		switch(this.faction) {
		default: case Faction.Neutral: return f != Faction.Neutral;
		case Faction.Rebel: return f == Faction.Police;
		case Faction.Police: return f == Faction.Rebel;
		}
	}
	
	void updatePersonsInRange() {
		this.PersonsInRange = Globals.People.GetInRange(this, PERSON_LOOK_RADIUS).ToList();
		int cnt_total = 0;
		int cnt_balance = 0;
		foreach(Person x in this.PersonsInRange) {
			if(x.IsDead)
				continue;
			cnt_total ++;
			if(isThreat(x.faction)) {
				cnt_balance --;
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
			force -= AVOID_OTHER_STRENGTH * avoidFalloff(delta.magnitude, d_min) * delta.normalized;
		}
		return force;
	}
	
	Vector3 computeAvoidLevel() {
		Vector3 force = Vector3.zero;
		float d_min = 2.0f * RADIUS;
		foreach(Building b in BuildingsInRange) {
			Vector3 h = b.Distance(this.transform.position);
			force += AVOID_LEVEL_STRENGTH * avoidFalloff(h.magnitude, d_min) * h.normalized;
		}
		return force;
	}
	
}
