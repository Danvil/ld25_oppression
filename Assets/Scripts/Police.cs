using UnityEngine;
using System.Collections;
using System.Linq;

public class Police : MonoBehaviour {
	
	const float FAR_SQUAD_RANGE = 1.0f;
	const float NEAR_SQUAD_RANGE = 0.5f;
	
	Person myself;
	
	bool isReturning = false;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		if(myself.Squad && myself == myself.Squad.Leader) {
			// i am the boss!
			myself.FollowTarget = null;
			myself.AttackTarget = null;
			bool isPlayerControled = Globals.IsPlayerControled(myself);
			myself.IsFast = isPlayerControled;
			myself.SetEnableRandomGoals(!isPlayerControled);
		}
		else {
			if(isReturning) {
				if(!myself.Squad || Tools.Distance(myself,myself.Squad.Leader) < NEAR_SQUAD_RANGE) {
					isReturning = false;
				}
				else {
					// return to squad
					myself.FollowTarget = myself.Squad.Leader;
					myself.AttackTarget = null;
					myself.IsFast = true;
					myself.SetEnableRandomGoals(false);
				}
			}
			else {
				if(myself.Squad && Tools.Distance(myself,myself.Squad.Leader) > FAR_SQUAD_RANGE) {
					isReturning = true;
				}
				// look for leader
				if(!myself.Squad) {
					Person possibleLeader = (from x in myself.PersonsInRange where x.faction == Faction.Police && x.IsSquadLeader select x).FirstOrDefault();
					if(possibleLeader) {
						//if(MoreMath.CheckOccurence(SQUAD_JOIN_RATE)) {
							// join squad
							possibleLeader.Squad.Add(myself);
						//}
					}
					else {
//						if(MoreMath.CheckOccurence(SQUAD_CREATE_RATE)) {
//							// become leader
//							myself.MakeSquad();
//						}
					}
				}
				// rest
				myself.IsFleeing = (myself.ThreatLevel <= -5);
				Person p;
				if(myself.IsFleeing) {
					// flee from threat
					var q = from x in myself.PersonsInRange where x.faction != Faction.Police && !x.IsDead select x;
					p = Tools.GetNearest(myself, q);
				}
				else {
					// find victim
					bool attackCivil = myself.IsRampageSquad;
					var q = from x in myself.PersonsInRange where (x.faction == Faction.Rebel || (attackCivil && x.faction == Faction.Neutral)) && (!x.IsDead || x.DeathTime < 2.0f) select x;
					p = Tools.GetNearest(myself, q);
				}
				myself.FollowTarget = p;
				myself.AttackTarget = p;
				myself.IsFast = p;
			}
		}
	}
}
