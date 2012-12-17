using UnityEngine;
using System.Collections;

public class Commander : MonoBehaviour {
	
	public Person Myself { get; private set; }
	
	public Vector3 Target { get; set; }
	
	public bool IsRampage { get; set; }
	
	void Awake() {
		Globals.Commanders.Add(this);
		Myself = GetComponent<Person>();
		IsRampage = false;
	}
	
	// Use this for initialization
	void Start () {
		Target = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {		
		Myself.IsUnmovable = true;
		Vector3 d = Target - this.transform.position;
		if(d.magnitude > 0.03f) {
			Myself.AdditionalForces.Add(d.normalized);
		}
		transform.FindChild("Rampage").gameObject.SetActive(IsRampage);
	}
}
