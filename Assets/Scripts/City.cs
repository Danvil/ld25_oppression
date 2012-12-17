using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class City : MonoBehaviour {
	
	const float SUPPORT_CHANGE_REBELS = -0.1f;
	const float SUPPORT_CHANGE_POLICE = +0.2f;
	const float SUPPORT_CHANGE_MAX = 10.0f;
	const float SUPPORT_UPDATE_RATE = 1.0f;
	const float DEATH_SUPPORT_CHANGE = 5.0f;

	public GameObject pfBuilding;
	public GameObject pfStreet;
	
	List<Building> buildings = new List<Building>();
	List<Street> streets = new List<Street>();
	
	int block_count = 3;
	int size;
	
	// tells if a square is blocked by a building
	int[,] occupancyLayer;
	
	public Vector3 SizeMin {
		get; private set;
	}
	
	public Vector3 SizeMax {
		get; private set;
	}
	
	public bool IsBlocked(int x, int y) {
		if(	   x < 0 || size <= x
			|| y < 0 || size <= y) {
			return true;
		}
		else {
			return occupancyLayer[x,y] == 1;
		}
	}
	
	public bool IsBlocked(Vector3 p) {
		return IsBlocked(
			Mathf.FloorToInt(p.x),
			Mathf.FloorToInt(p.z));
	}
	
	public IEnumerable<Building> GetBuildingsInRange(Vector3 position, float radius) {
		float r2 = radius;
		foreach(Building x in buildings.ToArray()) {
			if((position - x.transform.position).sqrMagnitude < r2) {
				yield return x;
			}
		}
	}
	
	public void WitnessDeath(Faction victim, Faction murderer) {
		float supportDelta = 0.0f;
		if(murderer == Faction.Police) {
			supportDelta -= DEATH_SUPPORT_CHANGE;
		}
		if(murderer == Faction.Rebel) {
			supportDelta += DEATH_SUPPORT_CHANGE;
		}
		foreach(Building x in buildings) {
			x.ChangeSupport(supportDelta);
		}
	}
	
	void Awake() {
		if(Globals.City != null) {
			throw new Exception("Only one city allowed!");
		}
		Globals.City = this;
		size = 2 + 1 + block_count*4;
		SizeMin = Vector3.zero;
		SizeMax = new Vector3((float)size+1.0f, 0.0f, (float)size+1.0f);
	}

	// Use this for initialization
	void Start () {
		createCityRegular();
	}
	
	bool ok(int i, int j) {
		return 0 <= i && i < size && 0 <= j && j < size;
	}

	float next_street_support_update = 0.0f;

	// Update is called once per frame
	void Update () {
		if(MyTime.time < next_street_support_update)
			return;
		next_street_support_update = MyTime.time + SUPPORT_UPDATE_RATE;
		// compute street support based on near people
		float r = Mathf.Sqrt(2.0f);
		float[,] support = new float[size,size];
		for(int i=0; i<size; i++) {
			for(int j=0; j<size; j++) {
				if(occupancyLayer[j,i] == 0) {
					// street
					Vector3 pos = new Vector3((float)j + 0.5f, 0.0f, (float)i + 0.5f);
					Collider[] query = Physics.OverlapSphere(pos, r);
					List<Person> result = new List<Person>();
					result.Capacity = query.Length;
					float s = 0.0f;
					foreach(Collider x in query) {
						Person p = x.gameObject.GetComponent<Person>();
						if(p && !p.AttackTarget) {
							if(p.faction == Faction.Rebel) s += SUPPORT_CHANGE_REBELS;
							if(p.faction == Faction.Police) s += SUPPORT_CHANGE_POLICE;
						}
					}
					support[j,i] = s;
				}
				else {
					support[j,i] = 0.0f;
				}
			}
		}
		float[,] bsupport = new float[size,size];
		// compute support based on near street support
		for(int i=0; i<size; i++) {
			for(int j=0; j<size; j++) {
				float q = 0.0f;
				if(occupancyLayer[j,i] == 1) {
					if(ok(j-1,i-1)) q += support[j-1,i-1];
					if(ok(j  ,i-1)) q += support[j  ,i-1];
					if(ok(j+1,i-1)) q += support[j+1,i-1];
					if(ok(j-1,i  )) q += support[j-1,i  ];
					if(ok(j  ,i  )) q += support[j  ,i  ];
					if(ok(j+1,i  )) q += support[j+1,i  ];
					if(ok(j-1,i+1)) q += support[j-1,i+1];
					if(ok(j  ,i+1)) q += support[j  ,i+1];
					if(ok(j+1,i+1)) q += support[j+1,i+1];
				}
				else {
					q = support[j,i];
				}
				bsupport[j,i] = q;
			}
		}
		// update building support
		foreach(Building b in buildings) {
			int x = (int)b.transform.position.x;
			int y = (int)b.transform.position.z;
			float s = MoreMath.Clamp(bsupport[x,y], -SUPPORT_CHANGE_MAX, +SUPPORT_CHANGE_MAX);
			b.ChangeSupport(s);
		}
	}
	
	void createCityRegular() {
		// init city layer
		occupancyLayer = new int[size,size];
		for(int i=0; i<size; i++) {
			for(int j=0; j<size; j++) {
				occupancyLayer[i,j] = 0;
			}
		}
		for(int i=0; i<size; i++) {
			occupancyLayer[i,0] = 1;
			occupancyLayer[i,size-1] = 1;
			occupancyLayer[0,i] = 1;
			occupancyLayer[size-1,i] = 1;
		}
		// set houses (3x3)
		for(int i=0; i<block_count; i++) {
			for(int j=0; j<block_count; j++) {
				int x = 2 + j*4;
				int y = 2 + i*4;
				if(i==block_count/2 && j==block_count/2) {
					// square
				}
				else {
					occupancyLayer[x,y] = 1;
					//occupancyLayer[x+1,y] = 1;
					occupancyLayer[x+2,y] = 1;
					occupancyLayer[x,y+1] = 1;
					occupancyLayer[x+2,y+1] = 1;
					occupancyLayer[x,y+2] = 1;
					occupancyLayer[x+1,y+2] = 1;
					occupancyLayer[x+2,y+2] = 1;
				}
			}
		}
		// create
		for(int i=0; i<size; i++) {
			for(int j=0; j<size; j++) {
				GameObject u;
				if(occupancyLayer[j,i] == 1) {
					// create building
					u = (GameObject)Instantiate(pfBuilding);
					buildings.Add(u.GetComponent<Building>());
				}
				else {
					// create street
					u = (GameObject)Instantiate(pfStreet);
					streets.Add(u.GetComponent<Street>());
				}
				u.transform.position = new Vector3(j,0,i);
				u.transform.parent = this.transform;
			}
		}
	}
}
