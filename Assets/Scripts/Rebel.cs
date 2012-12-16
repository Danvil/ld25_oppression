using UnityEngine;
using System.Collections;

public class Rebel : MonoBehaviour {
	
	Person myself;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		myself.IsFleeing = (myself.HitpointsCurrent <= myself.hitpointsMax || myself.ThreatLevel <= -3);
		myself.FollowTarget = myself.ClosestEnemy;
		if(myself.ThreatLevel >= 3) {
			myself.AttackTarget = myself.ClosestEnemy;
		}
		else {
			myself.AttackTarget = myself.ClosestEnemy;
		}
	}
}
