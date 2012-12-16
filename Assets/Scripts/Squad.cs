using UnityEngine;
using System.Collections.Generic;

public class Squad : MonoBehaviour
{
	const float FORCE_STRENGTH = 0.7f;
	const float LEADER_WEIGHT = 10.0f;
	const float DIST_FALLOFF = 3.0f;
	
	List<Person> members = new List<Person>();
	
	public Person Leader { get; set; }
	
	public void Add(Person p) {
		p.Squad = this;
		members.Add(p);
	}
	
	void Start() {
	}
	
	void Update() {
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
	}
}
