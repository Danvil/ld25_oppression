using UnityEngine;
using System.Collections;
using System.Linq;

public class Police : MonoBehaviour {
	
	const float FAR_SQUAD_RANGE = 1.5f;
	const float NEAR_SQUAD_RANGE = 0.5f;
	
	Person myself;
	
	bool isReturning = true;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		if(myself == myself.Squad.Leader) {
			// i am the boss!
			myself.FollowTarget = null;
			myself.AttackTarget = null;
			myself.IsFast = false;
			myself.SetEnableRandomGoals(true);
		}
		else {
			if(isReturning) {
				// return to squad
				myself.FollowTarget = myself.Squad.Leader;
				myself.AttackTarget = null;
				myself.IsFast = true;
				myself.SetEnableRandomGoals(false);
				if(Tools.Distance(myself,myself.Squad.Leader) < NEAR_SQUAD_RANGE) {
					isReturning = false;
				}
			}
			else {
				if(myself.Squad && Tools.Distance(myself,myself.Squad.Leader) > FAR_SQUAD_RANGE) {
					isReturning = true;
				}
				myself.IsFleeing = (myself.ThreatLevel <= -5);
				Person p;
				if(myself.IsFleeing) {
					// flee from threat
					var q = from x in myself.PersonsInRange where x.faction != Faction.Police && !x.IsDead select x;
					p = Tools.GetNearest(myself, q);
				}
				else {
					// find victim
					var q = from x in myself.PersonsInRange where x.faction != Faction.Police && (!x.IsDead || x.DeathTime < 2.0f) select x;
					p = Tools.GetNearest(myself, q);
				}
				myself.FollowTarget = p;
				myself.AttackTarget = p;
				myself.IsFast = p;
			}
		}
	}
}
