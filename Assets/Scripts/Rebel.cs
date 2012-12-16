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
		if(myself.ThreatLevel >= 3) {
			myself.AttackTarget = myself.ClosestEnemy;
			myself.FollowTarget = myself.ClosestEnemy;
		}
		else {
			myself.AttackTarget = null;
			myself.FollowTarget = null;
		}
		myself.IsFast = myself.AttackTarget;
		// random goal
		myself.SetEnableRandomGoals(!(
			myself.FollowTarget || myself.IsFleeing || (myself.Squad && myself.Squad.InSquadRange(myself))
		));
	}
}
