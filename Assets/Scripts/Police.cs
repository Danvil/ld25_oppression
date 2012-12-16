using UnityEngine;
using System.Collections;
using System.Linq;
using MoreLinq;

public class Police : MonoBehaviour {
	
	Person myself;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		myself.IsFleeing = false;
		// target
		myself.FollowTarget = null;
		myself.AttackTarget = null;
		var q = from x in myself.PersonsInRange where x.faction != Faction.Police && (!x.IsDead || x.DeathTime < 2.0f) select x;
		if(q.Count() > 0) {
			Person target = q.MinBy(x => (x.transform.position - myself.transform.position).sqrMagnitude);
			myself.FollowTarget = target;
			myself.AttackTarget = target;
		}
		myself.IsFast = myself.AttackTarget;
		// random goal
		myself.SetEnableRandomGoals(!(
			myself.FollowTarget || (myself.Squad && myself.Squad.InSquadRange(myself))
		));
	}
}
