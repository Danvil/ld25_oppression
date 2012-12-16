using UnityEngine;
using System.Collections.Generic;

public class Squad : MonoBehaviour
{
	const float SQUAD_RANGE_MAX = 3.0f;
	
	List<Person> members = new List<Person>();
	
	public void Add(Person p) {
		p.Squad = this;
		members.Add(p);
	}
	
	public float DistanceToSquad(Person p) {
		return (this.transform.position - p.transform.position).magnitude;
	}
	
	public bool InSquadRange(Person p) {
		return DistanceToSquad(p) < SQUAD_RANGE_MAX;
	}
	
	void Start() {
	}
	
	void Update() {
		// force
		Vector3 swarm = Vector3.zero;
		foreach(Person p in members) {
			swarm += p.Velocity;
		}
		swarm = swarm.normalized;
		foreach(Person p in members) {
			p.AdditionalForces.Add(2.0f * swarm);
		}
	}
}
