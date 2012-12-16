using UnityEngine;
using System.Collections;
using MoreLinq;

public class Police : MonoBehaviour {
	
	Person myself;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		Person target = myself.PersonsInRange.Count == 0 ? null : myself.PersonsInRange.MinBy(x =>
			(x.transform.position - myself.transform.position).magnitude
			+ (x.faction==Faction.Police ? 1000.0f : (x.faction==Faction.Neutral ? 0.25f : 0.0f )));
		if(target && target.faction != Faction.Police) {
			myself.FollowTarget = target;
			myself.AttackTarget = target;
		}
		else {
			myself.FollowTarget = null;
			myself.AttackTarget = null;
		}
	}
}
