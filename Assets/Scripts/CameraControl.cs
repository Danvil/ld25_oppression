using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float speed = 5.0f;
    
	// Use this for initialization
	void Start () {
		Vector3 cityCenter = 0.5f*(Globals.City.SizeMax + Globals.City.SizeMin);
		transform.position = new Vector3(cityCenter.x, 0, cityCenter.z);
	}
	
	// Update is called once per frame
	void Update () {
		float dx = MyTime.deltaTime * speed;
		float v = dx * Input.GetAxis("Vertical");
        float h = dx * Input.GetAxis("Horizontal");
		this.transform.Translate(h, 0, v);
	}
}
