using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PeopleManager : MonoBehaviour {
	
	public GameObject pfRebel;
	
	List<Rebel> rebels = new List<Rebel>();
	
	public void Add(Rebel x) {
		rebels.Add(x);
	}
	
	public IEnumerable<GameObject> GetInRange(Vector3 pos, float r) {
		foreach(Rebel x in rebels.ToArray()) {
			float d = (pos - x.transform.position).magnitude;
			if(d < r) {
				yield return x.gameObject;
			}
		}
	}

	public IEnumerable<GameObject> GetInRange(GameObject source, float r) {
		foreach(Rebel x in rebels.ToArray()) {
			float d = (source.transform.position - x.transform.position).magnitude;
			if(x != source && d < r) {
				yield return x.gameObject;
			}
		}
	}

	void Awake() {
		if(Globals.People != null) {
			throw new Exception("Only one instance of class PeopleManager allowed!");
		}
		Globals.People = this;
	}

	// Use this for initialization
	void Start () {
		for(int i=0; i<100; i++) {
			GameObject x = (GameObject)Instantiate(pfRebel);
			x.transform.position = MoreMath.RandomInsideUnitCircleXZ;
			x.transform.parent = this.transform;
			Add(x.GetComponent<Rebel>());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
