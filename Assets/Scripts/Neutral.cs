using UnityEngine;
using System.Collections;

public class Neutral : MonoBehaviour {

	Person myself;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		myself.IsFleeing = myself.ClosestEnemy;
		myself.FollowTarget = myself.ClosestEnemy;
		myself.SetEnableRandomGoals(!myself.FollowTarget && !myself.IsFleeing);
		myself.IsFast = myself.IsFleeing;
	}
}
