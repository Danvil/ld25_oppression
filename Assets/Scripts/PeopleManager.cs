using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PeopleManager : MonoBehaviour {
	
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
	
	List<Person> rebels = new List<Person>();
	
	public void Add(Person x) {
		rebels.Add(x);
	}
	
	public void Kill(Person x) {
		if(rebels.Remove(x)) {
			Destroy(x.gameObject);
		}
	}
	
	public IEnumerable<Person> GetInRange(Vector3 pos, float r) {
		foreach(Person x in rebels.ToArray()) {
			float d = (pos - x.transform.position).magnitude;
			if(d < r) {
				yield return x;
			}
		}
	}

	public IEnumerable<Person> GetInRange(Person source, float r) {
		foreach(Person x in rebels.ToArray()) {
			float d = (source.transform.position - x.transform.position).magnitude;
			if(x != source && d < r) {
				yield return x;
			}
		}
	}

	void Awake() {
		if(Globals.People != null) {
			throw new System.Exception("Only one instance of class PeopleManager allowed!");
		}
		Globals.People = this;
	}

	// Use this for initialization
	void Start() {
		Vector3 centerRebel = 0.5f*(Globals.City.SizeMax + Globals.City.SizeMin) + new Vector3(0,0,1);
		for(int i=0; i<30; i++) {
			GameObject x = (GameObject)Instantiate(pfRebel);		
			x.transform.position = MoreMath.RandomInsideUnitCircleXZ + centerRebel;
			x.transform.parent = this.transform;
			Person p = x.GetComponent<Person>();
			p.faction = Faction.Rebel;
			Add(p);
		}
		Vector3 centerNeutral = 0.5f*(Globals.City.SizeMax + Globals.City.SizeMin) + new Vector3(-2,0,-.5f);
		for(int i=0; i<50; i++) {
			GameObject x = (GameObject)Instantiate(pfNeutral);		
			x.transform.position = MoreMath.RandomInsideUnitCircleXZ + centerNeutral;
			x.transform.parent = this.transform;
			Person p = x.GetComponent<Person>();
			p.faction = Faction.Neutral;
			Add(p);
		}
		Vector3 centerPolice = 0.5f*(Globals.City.SizeMax + Globals.City.SizeMin) + new Vector3(0,0,-2);
		for(int i=0; i<30; i++) {
			GameObject x = (GameObject)Instantiate(pfPolice);		
			x.transform.position = MoreMath.RandomInsideUnitCircleXZ + centerPolice;
			x.transform.parent = this.transform;
			Person p = x.GetComponent<Person>();
			p.faction = Faction.Police;
			Add(p);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
