using UnityEngine;
using System.Collections.Generic;

public class DecalManager : MonoBehaviour {

	int decalId = 1;

	List<GameObject> decals = new List<GameObject>();

	public GameObject prefabDecalBlood;

	public GameObject prefabDecalPlus;

	public GameObject prefabDecalMinus;

	void add(GameObject x, string prefix) {
		decals.Add(x);
		x.transform.parent = this.transform;
		x.name = prefix + "_" + System.String.Format("{0:D5}", decalId++);
	}

	public void CreateBlood(Vector3 position, float radius) {
		GameObject x = (GameObject)Instantiate(prefabDecalBlood);
		add(x, "blood");
		Decal decal = x.GetComponent<Decal>();
		decal.transform.position = position + new Vector3(0,Random.Range(-0.0009f,+0.0009f),0);
		decal.transform.localScale = radius * Vector3.one;
	}

	public void CreatePlus(Vector3 position) {
		GameObject x = (GameObject)Instantiate(prefabDecalPlus);
		add(x, "plus");
		Decal decal = x.GetComponent<Decal>();
		decal.transform.localPosition = position;
	}

	public void CreateMinus(Vector3 position) {
		GameObject x = (GameObject)Instantiate(prefabDecalMinus);
		add(x, "minus");
		Decal decal = x.GetComponent<Decal>();
		decal.transform.localPosition = position;
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
