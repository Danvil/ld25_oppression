using UnityEngine;
using System.Collections;
using System.Linq;

public class Rebel : MonoBehaviour {
	
	const float SQUAD_DIST_NEAR = 0.5f;
	const float SQUAD_DIST_FAR = 0.8f;
	const float SQUAD_JOIN_RATE = 0.5f;
	const float SQUAD_CREATE_RATE = 0.01f;
	
	Person myself;
	bool isReturning = false;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
		isReturning = false;
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
			if(isReturning) {
				myself.IsFleeing = false;
				// return to squad
				myself.FollowTarget = myself.Squad.Leader;
				myself.AttackTarget = null;
				myself.IsFast = true;
				myself.SetEnableRandomGoals(false);
				if(!myself.Squad || Tools.Distance(myself,myself.Squad.Leader) < SQUAD_DIST_NEAR) {
					isReturning = false;
				}
			}
			else {
				if(myself.Squad && !myself.Squad.Leader.IsFleeing && Tools.Distance(myself,myself.Squad.Leader) > SQUAD_DIST_FAR) {
					isReturning = true;
				}
				// normal behaviour
				// look for leader
				if(!myself.Squad) {
					Person possibleLeader = (from x in myself.PersonsInRange where x.faction == Faction.Rebel && x.IsSquadLeader select x).FirstOrDefault();
					if(possibleLeader) {
						if(MoreMath.CheckOccurence(SQUAD_JOIN_RATE)) {
							// join squad
							possibleLeader.Squad.Add(myself);
						}
					}
					else {
						if(MoreMath.CheckOccurence(SQUAD_CREATE_RATE)) {
							// become leader
							myself.MakeSquad();
						}
					}
				}
				// flee if ...
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
