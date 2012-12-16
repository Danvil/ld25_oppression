using UnityEngine;
using System.Collections;
using System.Linq;

public class Rebel : MonoBehaviour {
	
	Person myself;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		if(myself.Squad && myself == myself.Squad.Leader) {
			// i am the boss!
			var q = from x in myself.PersonsInRange where x.faction == Faction.Police && !x.IsDead && Tools.Distance(myself,x) < 0.5f select x;
			Person enemy = Tools.GetNearest(myself, q);
			if(enemy) {
				// flee!
				myself.FollowTarget = enemy;
				myself.AttackTarget = null;
				myself.IsFast = true;
				myself.IsFleeing = true;
				myself.SetEnableRandomGoals(false);
			}
			else {
				myself.FollowTarget = null;
				myself.AttackTarget = null;
				myself.IsFast = false;
				myself.IsFleeing = false;
				myself.SetEnableRandomGoals(true);
			}
		}
		else {
			if(myself.Squad && (Tools.Distance(myself, myself.Squad.Leader) > 2.0f || myself.Squad.Leader.IsFleeing)) {
				myself.IsFleeing = false;
				// return to squad
				myself.FollowTarget = myself.Squad.Leader;
				myself.AttackTarget = null;
				myself.IsFast = true;
				myself.SetEnableRandomGoals(false);
			}
			else {
				// normal behaviour
				myself.IsFleeing = (myself.HitpointsCurrent <= myself.hitpointsMax/2 || myself.ThreatLevel <= -3);
				if(!myself.IsFleeing && myself.ThreatLevel >= 3) {
					// target
					var q = from x in myself.PersonsInRange where x.faction == Faction.Police && (!x.IsDead || x.DeathTime < 2.0f) select x;
					Person p = Tools.GetNearest(myself, q);
					myself.FollowTarget = p;
					myself.AttackTarget = p;
				}
				else {
					myself.AttackTarget = null;
					myself.FollowTarget = null;
				}
				myself.IsFast = myself.AttackTarget || myself.IsFleeing;
				// random goal
				myself.SetEnableRandomGoals(!(myself.FollowTarget || myself.IsFleeing));
			}
		}
	}
}
