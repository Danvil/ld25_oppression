using UnityEngine;
using System.Collections;

public class Commander : MonoBehaviour {
	
	public Person Myself { get; private set; }
	
	void Awake() {
		Globals.Commander = this;
		Myself = GetComponent<Person>();
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		// http://answers.unity3d.com/questions/9737/how-can-i-make-the-character-follow-the-mouse-poin.html
		
	    // Generate a plane that intersects the transform's position with an upwards normal.
	    Plane playerPlane = new Plane(Vector3.up, transform.position);
	
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
	        var targetPoint = ray.GetPoint(hitdist);
	
			Vector3 f = (targetPoint - this.transform.position).normalized;
			Myself.AdditionalForces.Add(f);
	    }
	}
}
