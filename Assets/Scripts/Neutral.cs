using UnityEngine;
using System.Collections;
using System.Linq;

public class Neutral : MonoBehaviour {

	Person myself;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		// target
		var q = from x in myself.PersonsInRange where x.faction == Faction.Police || x.IsDead select x;
		Person enemy = Tools.GetNearest(myself, q);	
		myself.IsFleeing = enemy;
		myself.AttackTarget = (myself.HitpointsCurrent == myself.hitpointsMax ? enemy : null);
		myself.FollowTarget = enemy;
		myself.SetEnableRandomGoals(!myself.FollowTarget && !myself.IsFleeing);
		myself.IsFast = myself.IsFleeing;
	}
}
