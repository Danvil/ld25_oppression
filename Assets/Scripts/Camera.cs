using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

	public float speed = 3.0F;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float dx = MyTime.deltaTime * speed;
		float v = dx * Input.GetAxis("Vertical");
        float h = dx * Input.GetAxis("Horizontal");
		this.transform.Translate(h, v, 0);
	}
}
