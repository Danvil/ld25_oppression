using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PeopleManager : MonoBehaviour {
	
	public GameObject pfRebel;
	public GameObject pfPolice;
	
	List<Person> rebels = new List<Person>();
	
	public void Add(Person x) {
		rebels.Add(x);
	}
	
	public IEnumerable<GameObject> GetInRange(Vector3 pos, float r) {
		foreach(Person x in rebels.ToArray()) {
			float d = (pos - x.transform.position).magnitude;
			if(d < r) {
				yield return x.gameObject;
			}
		}
	}

	public IEnumerable<GameObject> GetInRange(GameObject source, float r) {
		foreach(Person x in rebels.ToArray()) {
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
		Vector3 centerRebel = 0.5f*(Globals.City.SizeMax + Globals.City.SizeMin) + new Vector3(0,0,0);
		for(int i=0; i<50; i++) {
			GameObject x = (GameObject)Instantiate(pfRebel);		
			x.transform.position = MoreMath.RandomInsideUnitCircleXZ + centerRebel;
			x.transform.parent = this.transform;
			Add(x.GetComponent<Person>());
		}
		Vector3 centerPolice = 0.5f*(Globals.City.SizeMax + Globals.City.SizeMin) - new Vector3(0,0,2);
		for(int i=0; i<50; i++) {
			GameObject x = (GameObject)Instantiate(pfPolice);		
			x.transform.position = MoreMath.RandomInsideUnitCircleXZ + centerPolice;
			x.transform.parent = this.transform;
			Add(x.GetComponent<Person>());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
