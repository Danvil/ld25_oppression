using UnityEngine;
using System.Collections.Generic;

public class Globals : MonoBehaviour {

	public static Camera MainCamera;

	public static City City;
	
	public static PeopleManager People;
	
	public static DecalManager DecalManager;
	
	public static List<Commander> Commanders = new List<Commander>();
	
	public static bool IsPlayerControled(Person x) {
		foreach(var c in Commanders) {
			if(x == c.Myself) {
				return true;
			}
		}
		return false;
	}
	
	void Awake() {
		MainCamera = GameObject.Find("Camera").GetComponent<Camera>();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
