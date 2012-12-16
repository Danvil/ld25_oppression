using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {
	
	public GameObject pfMarkerPolice;
	public GameObject pfMarkerRebels;
	
	public Vector3[] rawGates;

	List<Vector3> gates = new List<Vector3>();
	
	public int faction;

	// Use this for initialization
	void Start () {
		for(int i=0; i<rawGates.Length; i++) {
			Vector3 pnt = this.transform.position + rawGates[i];
			if(!Globals.City.IsBlocked(pnt)) {
				gates.Add(pnt);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Vector3 q in gates)
			Debug.DrawLine(this.transform.position + new Vector3(0.5f,0,0.5f), q, Color.white);
	}
}
