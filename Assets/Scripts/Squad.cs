using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Squad : MonoBehaviour
{
	const float FORCE_STRENGTH = 0.7f;
	const float LEADER_WEIGHT = 10.0f;
	const float DIST_FALLOFF = 3.0f;
	
	List<Person> members = new List<Person>();
	
	GameObject rampage;
	
	public Person Leader { get; set; }
	
	public bool IsRampage { get; set; }
	
	public void Add(Person p) {
		p.Squad = this;
		members.Add(p);
	}
	
	void Start() {
		IsRampage = false;
		rampage = (GameObject)Instantiate(Globals.People.pfRampage);
		rampage.transform.parent = this.transform;
		rampage.transform.localPosition = Vector3.zero;
		rampage.SetActive(false);
	}
	
	void Update() {
		if(Leader.IsDead) {
			foreach(Person x in members) {
				x.Squad = null;
			}
			members.Clear();
		}
		members = (from x in members where !x.IsDead select x).ToList();
		// force
		Vector3 swarm = Vector3.zero;
		foreach(Person p in members) {
			swarm += (p == Leader ? LEADER_WEIGHT : 1.0f) * p.Velocity;
		}
		Vector3 force = FORCE_STRENGTH * swarm.normalized;
		foreach(Person p in members) {
			if(p == Leader) continue;
			float w = FORCE_STRENGTH / (1.0f + DIST_FALLOFF*Tools.Distance(p, Leader));
			p.AdditionalForces.Add(w*force);
		}
		rampage.SetActive(IsRampage);
	}
}
