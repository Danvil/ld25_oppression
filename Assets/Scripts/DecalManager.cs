using UnityEngine;
using System.Collections.Generic;

public class DecalManager : MonoBehaviour {

	int decalId = 1;

	List<GameObject> decals = new List<GameObject>();

	public GameObject prefabDecalBlood;

	public GameObject prefabDecalEnv;

	public GameObject prefabDecalNewIndividum;

	public GameObject prefabDecalNewSpecies;

	void add(GameObject x, string prefix) {
		decals.Add(x);
		x.transform.parent = this.transform;
		x.name = prefix + "_" + System.String.Format("{0:D5}", decalId++);
	}

	public void CreateBlood(Vector3 position, float radius) {
		GameObject x = (GameObject)Instantiate(prefabDecalBlood);
		add(x, "blood");
		Decal decal = x.GetComponent<Decal>();
		//decal.Color = new Color(1.0f, color.g, color.b, 0.0f);
		//decal.wobbelRadius = wobbelRadius;
		//decal.follow = null;
		decal.transform.position = position + new Vector3(0,Random.Range(-0.001f,+0.001f),0);
		//decal.fixedZ = height;
		decal.transform.localScale = radius * Vector3.one;
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
