using UnityEngine;
using System.Collections.Generic;

public class DecalManager : MonoBehaviour {

	int decalId = 1;

	List<GameObject> decals = new List<GameObject>();

	public GameObject prefabDecalEnv;

	public GameObject prefabDecalNewIndividum;

	public GameObject prefabDecalNewSpecies;

	void add(GameObject x, string prefix) {
		decals.Add(x);
		x.transform.parent = this.transform;
		x.name = prefix + "_" + System.String.Format("{0:D5}", decalId++);
	}

	public void CreateEnv(Transform follow, float radius, Color color, float wobbelRadius=0.0f, float height=1.0f) {
		GameObject x = (GameObject)Instantiate(prefabDecalEnv);
		add(x, "env");
		Decal decal = x.GetComponent<Decal>();
		decal.Color = new Color(color.r, color.g, color.b, 0.0f);
		decal.wobbelRadius = wobbelRadius;
		decal.follow = follow;
		decal.fixedZ = -height;
		x.transform.localScale = radius * Vector3.one;
	}

	public void CreateNewIndividum(Transform follow, float radius, float height=0.1f) {
		GameObject x = (GameObject)Instantiate(prefabDecalNewIndividum);
		add(x, "ringI");
		Decal decal = x.GetComponent<Decal>();
		decal.wobbelRadius = 0.0f;
		decal.follow = follow;
		decal.fixedZ = -height;
		x.transform.localScale = radius * Vector3.one;
	}

	public void CreateNewSpecies(Transform follow, float radius, float height=0.1f) {
		GameObject x = (GameObject)Instantiate(prefabDecalNewSpecies);
		add(x, "ringS");
		Decal decal = x.GetComponent<Decal>();
		decal.wobbelRadius = 0.0f;
		decal.follow = follow;
		decal.fixedZ = -height;
		x.transform.localScale = radius * Vector3.one;
	}

	void Awake() {
		Globals.DecalManager = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject x in decals.ToArray()) {
			Decal decal = x.GetComponent<Decal>();
			if(decal.IsDead) {
				decals.Remove(x);
				Destroy(x);
			}
		}
	}
}
