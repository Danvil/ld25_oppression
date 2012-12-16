using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {
	
	const float SUPPORT_DECREASE_RATE = 1.0f;
	const float SUPPORT_SWITCH = 100.0f;
	const float SUPPORT_SWITCH_BIASED = 80.0f;
	const float SUPPORT_CHANGE_N_XkN = 30.0f;
	const float SUPPORT_CHANGE_N_XkY = 10.0f;
	const float SUPPORT_CHANGE_X_YkN = 20.0f;
	const float SUPPORT_CHANGE_X_XkN = -10.0f;
	const float SUPPORT_CHANGE_X_YkX = 15.0f;
	const float SUPPORT_CHANGE_X_XkY = 5.0f;
	const float GENERATE_RATE = 0.05f;
	const float GENERATE_SIGMA = 30.0f;
	const float GENERATE_RANGE = 60.0f;
	
	public GameObject pfMarkerPolice;
	public GameObject pfMarkerRebels;
	
	public Vector3[] rawGates;

	List<Vector3> gates = new List<Vector3>();
	
	public float factionSupport = 0.0f;
	Faction faction = Faction.Neutral;
	
	public void WitnessDeath(Faction victim, Faction murderer) {
		if(faction == Faction.Neutral) {
			if(murderer == Faction.Rebel && victim == Faction.Neutral) {
				factionSupport += SUPPORT_CHANGE_N_XkN;
			}
			if(murderer == Faction.Police && victim == Faction.Neutral) {
				factionSupport -= SUPPORT_CHANGE_N_XkN;
			}
			if(murderer == Faction.Rebel && victim == Faction.Police) {
				factionSupport += SUPPORT_CHANGE_N_XkY;
			}
			if(murderer == Faction.Police && victim == Faction.Rebel) {
				factionSupport -= SUPPORT_CHANGE_N_XkY;
			}
		}
		else if(faction == Faction.Police) {
			if(murderer == Faction.Rebel && victim == Faction.Neutral) {
				factionSupport += SUPPORT_CHANGE_X_YkN;
			}
			if(murderer == Faction.Police && victim == Faction.Neutral) {
				factionSupport += SUPPORT_CHANGE_X_XkN;
			}
			if(murderer == Faction.Rebel && victim == Faction.Police) {
				factionSupport += SUPPORT_CHANGE_X_YkX;
			}
			if(murderer == Faction.Police && victim == Faction.Rebel) {
				factionSupport += SUPPORT_CHANGE_X_XkY;
			}
		}
		else if(faction == Faction.Rebel) {
			if(murderer == Faction.Rebel && victim == Faction.Neutral) {
				factionSupport -= SUPPORT_CHANGE_X_XkN;
			}
			if(murderer == Faction.Police && victim == Faction.Neutral) {
				factionSupport -= SUPPORT_CHANGE_X_YkN;
			}
			if(murderer == Faction.Rebel && victim == Faction.Police) {
				factionSupport -= SUPPORT_CHANGE_X_XkY;
			}
			if(murderer == Faction.Police && victim == Faction.Rebel) {
				factionSupport -= SUPPORT_CHANGE_X_YkX;
			}
		}
	}
	
	GameObject markerRebels;
	GameObject markerPolice;

	// Use this for initialization
	void Start () {
		for(int i=0; i<rawGates.Length; i++) {
			Vector3 pnt = this.transform.position + rawGates[i];
			if(!Globals.City.IsBlocked(pnt)) {
				gates.Add(pnt);
			}
		}
		markerRebels = (GameObject)Instantiate(pfMarkerRebels);
		markerRebels.transform.Translate(new Vector3(0,1,0));
		markerRebels.transform.parent = this.transform;
		markerRebels.SetActive(false);
		markerPolice = (GameObject)Instantiate(pfMarkerRebels);
		markerPolice.transform.Translate(new Vector3(0,1,0));
		markerPolice.transform.parent = this.transform;
		markerPolice.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Vector3 q in gates)
			Debug.DrawLine(this.transform.position + new Vector3(0.5f,0,0.5f), q, Color.white);	
		// adapt faction support
		factionSupport -= Mathf.Sign(factionSupport) * MyTime.deltaTime * SUPPORT_DECREASE_RATE;
		if(faction == Faction.Neutral) {
			if(factionSupport >= SUPPORT_SWITCH) {
				faction = Faction.Police;
			}
			if(factionSupport <= -SUPPORT_SWITCH) {
				faction = Faction.Rebel;
			}
		}
		else if(faction == Faction.Rebel) {
			if(factionSupport >= -SUPPORT_SWITCH_BIASED) {
				faction = Faction.Neutral;
			}
		}
		else if(faction == Faction.Police) {
			if(factionSupport <= SUPPORT_SWITCH_BIASED) {
				faction = Faction.Neutral;
			}
		}
		markerRebels.SetActive(faction == Faction.Rebel);
		markerPolice.SetActive(faction == Faction.Police);
		// generate
		if(gates.Count > 0 && MoreMath.CheckOccurence(GENERATE_RATE)) {
			float v = MoreMath.RandomNormalDist(factionSupport, GENERATE_SIGMA);
			Vector3 p = GetRandomGatePosition() + 0.05f*MoreMath.RandomInsideUnitCircleXZ;
			if(v <= -GENERATE_RANGE) {
				Globals.People.Generate(Faction.Rebel, p);
			}
			else if(v >= GENERATE_RANGE) {
				Globals.People.Generate(Faction.Police, p);
			}
			else {
				Globals.People.Generate(Faction.Neutral, p);
			}
		}
	}
	
	Vector3 GetRandomGatePosition() {
		return gates[Random.Range(0, gates.Count - 1)];
	}
	

}
