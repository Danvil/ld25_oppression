using UnityEngine;
using System.Collections;

public class Police : MonoBehaviour {
	
	Person myself;
	
	// Use this for initialization
	void Start () {
		myself = GetComponent<Person>();
	}
	
	// Update is called once per frame
	void Update () {
		myself.FollowTarget = myself.ClosestEnemy;
		myself.AttackTarget = myself.ClosestEnemy;
	}
}
