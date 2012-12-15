using UnityEngine;
using System.Collections;

public class QuadMesh : MonoBehaviour {

	// Use this for initialization
	void Start () {
		CreateMesh();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void CreateMesh() {
		Mesh mesh = createMeshImpl();
		// update mesh filter
		var meshFilter = GetComponent<MeshFilter>();
		if(meshFilter) {
			meshFilter.mesh = mesh;
		}
		// update mesh collider
		var meshCollider = GetComponent<MeshCollider>();
		if(meshCollider) {
			meshCollider.sharedMesh = mesh;
		}
	}
	
	Mesh createMeshImpl() {
		float a = 1.0f;
		float b = 1.0f;
		// vertices
		Vector3[] vertices = new Vector3[4] {
			new Vector3(0,0,0),
			new Vector3(a,0,0),
			new Vector3(a,0,b),
			new Vector3(0,0,b)
		};
		// indices
		int[] indices = new int[6] {
			0,2,1,0,3,2,
		};	
		// uv
		Vector2[] uv = new Vector2[4] {
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(1,1),
			new Vector2(0,1)
		};
		// create mesh
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.uv = uv;
		return mesh;
	}}
