using UnityEngine;
using System.Collections;

public class Selector : MonoBehaviour {
	
	Commander selected = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// left click -> select commander
		if(Input.GetMouseButtonDown(0)) {
	        // Shoot ray from mouse position
	        Ray ray = Globals.MainCamera.ScreenPointToRay(Input.mousePosition);
	        RaycastHit[] hits = Physics.RaycastAll(ray);
	        foreach(RaycastHit hit in hits) {
				if(hit.transform.gameObject.layer == 8) {
					Debug.Log(hit.transform.gameObject.name);
					Commander c = hit.transform.gameObject.GetComponent<Commander>();
					if(c) {
						selected = c;
						return;
					}
				}
	        }
	    }
			
		// right click -> set commander goal
		if(selected && Input.GetMouseButtonDown(0)) {
			// http://answers.unity3d.com/questions/9737/how-can-i-make-the-character-follow-the-mouse-poin.html
			
		    // Generate a plane that intersects the transform's position with an upwards normal.
		    Plane playerPlane = new Plane(Vector3.up, selected.transform.position);
		
		    // Generate a ray from the cursor position
		    Ray ray = Globals.MainCamera.ScreenPointToRay(Input.mousePosition);
		
		    // Determine the point where the cursor ray intersects the plane.
		    // This will be the point that the object must look towards to be looking at the mouse.
		    // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
		    //   then find the point along that ray that meets that distance.  This will be the point
		    //   to look at.
		    float hitdist = 0.0f;
		    // If the ray is parallel to the plane, Raycast will return false.
		    if(playerPlane.Raycast(ray, out hitdist)) {
		        // Get the point along the ray that hits the calculated distance.
		       selected.Target = ray.GetPoint(hitdist);
		    }
		}
		
		this.gameObject.GetComponentInChildren<MeshRenderer>().enabled = selected;
		if(selected) {
			this.transform.position = selected.transform.position;
		}
		
	}
}
