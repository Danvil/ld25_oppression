using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour {

	public static Camera MainCamera;

	public static City City;
	
	public static PeopleManager People;
	
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
