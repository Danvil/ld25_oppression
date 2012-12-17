using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {
	
	const float SUPPORT_DECREASE_RATE = 1.0f;
	const float SUPPORT_SWITCH = 100.0f;
	const float SUPPORT_MAX = 180.0f;
	const float SUPPORT_SWITCH_BIASED = 80.0f;
	const float SUPPORT_CHANGE_N_XkN = 30.0f; // the poor kid!
	const float SUPPORT_CHANGE_N_XkY = 5.0f; // his fault
	const float SUPPORT_CHANGE_X_YkN = 10.0f; // casulties should be avoided!
	const float SUPPORT_CHANGE_X_XkN = -10.0f; // no civilians
	const float SUPPORT_CHANGE_X_YkX = -10.0f; // we will all die!
	const float SUPPORT_CHANGE_X_XkY = 5.0f; // die bastard!
	const float GENERATE_RATE = 0.01f;
	const float GENERATE_RATE_SUPPORT = 0.001f;
	const float GENERATE_SIGMA = 25.0f;
	const float GENERATE_RANGE = 40.0f;
	
	public GameObject pfMarkerNeutral;
	public GameObject pfMarkerPolice;
	public GameObject pfMarkerRebels;
	
	public Vector3[] rawGates;

	List<Vector3> gates = new List<Vector3>();
	
	public float factionSupport = 0.0f;
	Faction faction = Faction.Neutral;
	
	float lastDecalSupport = 0.0f;
	
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
	
	public void ChangeSupport(float delta) {
		factionSupport += delta;
	}
	
	GameObject markerNeutral;
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
		markerNeutral = (GameObject)Instantiate(pfMarkerNeutral);
		markerNeutral.transform.parent = this.transform;
		markerNeutral.transform.localPosition = new Vector3(0,0,0);
		markerNeutral.SetActive(true);
		markerRebels = (GameObject)Instantiate(pfMarkerRebels);
		markerRebels.transform.parent = this.transform;
		markerRebels.transform.localPosition = new Vector3(0,0,0);
		markerRebels.SetActive(false);
		markerPolice = (GameObject)Instantiate(pfMarkerRebels);
		markerPolice.transform.parent = this.transform;
		markerPolice.transform.localPosition = new Vector3(0,0,0);
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

		// limit support
		if(factionSupport < -SUPPORT_MAX)
			factionSupport = -SUPPORT_MAX;
		if(factionSupport > SUPPORT_MAX)
			factionSupport = +SUPPORT_MAX;

		// generate
		float generate_rate = GENERATE_RATE + Mathf.Abs(factionSupport) * GENERATE_RATE_SUPPORT;
		if(gates.Count > 0 && MoreMath.CheckOccurence(generate_rate)) {
			float v = MoreMath.RandomNormalDist(factionSupport, GENERATE_SIGMA);
			Vector3 p = GetRandomGatePosition() + 0.05f*MoreMath.RandomInsideUnitCircleXZ;
			if(v <= -GENERATE_RANGE) {
				Globals.People.Generate(Faction.Rebel, p);
			}
			//else if(v >= GENERATE_RANGE) {
			//	Globals.People.Generate(Faction.Police, p);
			//}
			else {
				Globals.People.Generate(Faction.Neutral, p);
			}
		}
		
		markerRebels.SetActive(faction == Faction.Rebel);
		markerPolice.SetActive(faction == Faction.Police);
		
		float color_q = 0.5f * factionSupport / SUPPORT_SWITCH;
		Color color = new Color(0.5f - color_q, 0.5f - Mathf.Abs(color_q), 0.5f + color_q);
		var mat = markerNeutral.GetComponentInChildren<MeshRenderer>().material;
		mat.color = color;
		mat.SetColor("_Diffuse", color);
		
		if(Mathf.Abs(factionSupport - lastDecalSupport) > 10.0f) {
			if(factionSupport > lastDecalSupport) {
				Globals.DecalManager.CreatePlus(this.transform.position);
			}
			else {
				Globals.DecalManager.CreateMinus(this.transform.position);
			}
			lastDecalSupport = factionSupport;
		}
	}
	
	Vector3 GetRandomGatePosition() {
		return gates[Random.Range(0, gates.Count - 1)];
	}
	
	Vector3 OrthogonalRightDistance(Vector3 a, Vector3 b, Vector3 x) {
		Vector3 x0 = x - a;
		Vector3 n = (b - a).normalized;
		float q = Vector3.Dot(n, x0);
		Vector3 h = x0 - q*n;
		if(q < 0) {
			return x - a;
		}
		if(q > 1) {
			return x - b;
		}
		float s = Vector3.Dot(new Vector3(0,1,0), Vector3.Cross(n, h));
		if(s < 0) {
			return 100.0f * Vector3.one;
		}
		return h;
	}
	
	public Vector3 Distance(Vector3 x) {
		Vector3 x0 = x - this.transform.position;
		Vector3 h1 = OrthogonalRightDistance(new Vector3(0,0,0), new Vector3(1,0,0), x0);
		Vector3 h2 = OrthogonalRightDistance(new Vector3(1,0,0), new Vector3(1,0,1), x0);
		Vector3 h3 = OrthogonalRightDistance(new Vector3(1,0,1), new Vector3(0,0,1), x0);
		Vector3 h4 = OrthogonalRightDistance(new Vector3(0,0,1), new Vector3(0,0,0), x0);
		float d1 = h1.sqrMagnitude;
		float d2 = h2.sqrMagnitude;
		float d3 = h3.sqrMagnitude;
		float d4 = h4.sqrMagnitude;
		float d = Mathf.Min(new float[] { d1, d2, d3, d4});
		if(d == d1) return h1;
		else if(d == d2) return h2;
		else if(d == d3) return h3;
		else if(d == d4) return h4;
		else return Vector3.zero;
	}
}
