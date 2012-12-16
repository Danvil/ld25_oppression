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
		myself.IsFleeing = true;
		myself.FollowTarget = myself.ClosestEnemy;
	}
}
