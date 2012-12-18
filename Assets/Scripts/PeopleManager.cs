using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PeopleManager : MonoBehaviour {
	
	const int MAX_PEOPLE = 500;
	
	public AudioClip[] audioHit;
	public AudioClip[] audioDeath;
	
	AudioClip GetRandom(AudioClip[] v) {
		if(v.Length > 0) {
			int q = Random.Range(0, v.Length - 1);
			return v[q];
		}
		else {
			return null;
		}
	}
	
	public AudioClip RandomHitAudio {
		get {
			return GetRandom(audioHit);
		}
	}
	
	public AudioClip RandomDeathAudio {
		get {
			return GetRandom(audioDeath);
		}
	}
	
	public GameObject pfNeutral;
	public GameObject pfRebel;
	public GameObject pfPolice;

	public GameObject pfRampage;

	List<Person> people = new List<Person>();
	
	public void Add(Person x) {
		people.Add(x);
	}
	
	public void Kill(Person x) {
		if(people.Remove(x)) {
			Destroy(x.gameObject);
		}
	}
	
//	public IEnumerable<Person> GetInRange(Vector3 pos, float r) {
//		foreach(Person x in people.ToArray()) {
//			float d = (pos - x.transform.position).magnitude;
//			if(d < r) {
//				yield return x;
//			}
//		}
//	}

	public List<Person> GetInRange(Person source, float r) {
//		foreach(Person x in people.ToArray()) {
//			float d = (source.transform.position - x.transform.position).magnitude;
//			if(x != source && d < r) {
//				yield return x;
//			}
//		}
		Collider[] query = Physics.OverlapSphere(source.transform.position, r);
		List<Person> result = new List<Person>();
		result.Capacity = query.Length;
		foreach(Collider x in query) {
			Person p = x.gameObject.GetComponent<Person>();
			if(p && p != source) {
				result.Add(p);
			}
		}
		return result;
	}
	
	GameObject getFactionPrefab(Faction faction) {
		switch(faction) {
		case Faction.Rebel: return pfRebel;
		case Faction.Neutral: return pfNeutral;
		case Faction.Police: return pfPolice;
		default: return null;
		}
	}
	
	public Person Generate(Faction faction, Vector3 position) {
		if(people.Count > MAX_PEOPLE) {
			return null;
		}
		GameObject x = (GameObject)Instantiate(getFactionPrefab(faction));
		x.transform.position = position;
		x.transform.parent = this.transform;
		Person p = x.GetComponent<Person>();
		Add(p);
		return p;
	}

	void Awake() {
		if(Globals.People != null) {
			throw new System.Exception("Only one instance of class PeopleManager allowed!");
		}
		Globals.People = this;
	}

	// Use this for initialization
	void Start() {
		
		foreach(Commander c in Globals.Commanders) {
			c.Myself.MakeSquad();
			for(int i=0; i<30; i++) {
				Person x = Generate(Faction.Police, c.transform.position + MoreMath.RandomInsideUnitCircleXZ);
				c.Myself.Squad.Add(x);
			}
		}
		
		Vector3 centerRebel = 0.5f*(Globals.City.SizeMax + Globals.City.SizeMin) + new Vector3(0,0,1);
		Person rebelSquad = Generate(Faction.Rebel, MoreMath.RandomInsideUnitCircleXZ + centerRebel);
		rebelSquad.MakeSquad();		
		for(int i=0; i<20; i++) {
			Person x = Generate(Faction.Rebel, MoreMath.RandomInsideUnitCircleXZ + centerRebel);
			rebelSquad.Squad.Add(x);
		}
	
		Vector3 centerNeutral = 0.5f*(Globals.City.SizeMax + Globals.City.SizeMin) + new Vector3(-2,0,-.5f);
		for(int i=0; i<60; i++) {
			Generate(Faction.Neutral, MoreMath.RandomInsideUnitCircleXZ + centerNeutral);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
