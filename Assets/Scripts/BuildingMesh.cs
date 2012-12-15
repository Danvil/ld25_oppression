using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingMesh : MonoBehaviour {

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
	
	static void AddSide(
		List<Vector3> vertices, List<int> indices, List<Vector2> uv,
		Vector3[] add_vertices, Vector2[] add_uv
	) {
		vertices.AddRange(add_vertices);
		int i = indices.Count;
		indices.AddRange(new int[] {0,1,2,1,2,3});
		uv.AddRange(add_uv);
	}
	
	Mesh createMeshImpl() {
		Debug.Log("Creating mesh");
		float a = 1.0f;
		float b = 1.0f;
		float h = 1.0f;
		
		List<Vector3> vertices = new List<Vector3>();
		List<int> indices = new List<int>();
		List<Vector2> uv = new List<Vector2>();
		
		// x-
		AddSide(vertices, indices, uv,
			new Vector3[4] {
				new Vector3(0,0,b),
				new Vector3(0,0,0),
				new Vector3(0,h,0),
				new Vector3(0,h,b),
			},
			new Vector2[4] {
				new Vector2(0,0),
				new Vector2(1,0),
				new Vector2(1,1),
				new Vector2(0,1),
			}
		);
		// x+
		AddSide(vertices, indices, uv,
			new Vector3[4] {
				new Vector3(a,0,b),
				new Vector3(a,0,0),
				new Vector3(a,h,0),
				new Vector3(a,h,b),
			},
			new Vector2[4] {
				new Vector2(0,0),
				new Vector2(1,0),
				new Vector2(1,1),
				new Vector2(0,1),
			}
		);
		// z-
		AddSide(vertices, indices, uv,
			new Vector3[4] {
				new Vector3(0,0,0),
				new Vector3(a,0,0),
				new Vector3(a,h,0),
				new Vector3(0,h,0),
			},
			new Vector2[4] {
				new Vector2(0,0),
				new Vector2(1,0),
				new Vector2(1,1),
				new Vector2(0,1),
			}
		);
		// z+
		AddSide(vertices, indices, uv,
			new Vector3[4] {
				new Vector3(0,0,b),
				new Vector3(a,0,b),
				new Vector3(a,h,b),
				new Vector3(0,h,b),
			},
			new Vector2[4] {
				new Vector2(0,0),
				new Vector2(1,0),
				new Vector2(1,1),
				new Vector2(0,1),
			}
		);
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.uv = uv.ToArray();
		return mesh;
//		// vertices
//		Vector3[] vertices = new Vector3[8] {
//			new Vector3(0,0,0),
//			new Vector3(a,0,0),
//			new Vector3(a,b,0),
//			new Vector3(0,b,0),
//			new Vector3(0,0,h),
//			new Vector3(a,0,h),
//			new Vector3(a,b,h),
//			new Vector3(0,b,h),
//		};
//		mesh.vertices = vertices;
//		// indices
//		int[] indices = new int[30] {
//			3,0,4,3,4,7,// x-
//			1,2,6,1,6,5,// x+
//			0,1,5,0,5,4,// z-
//			2,3,7,2,7,6,// z+
//			7,4,5,7,5,6,// y+
//		};	
//		mesh.triangles = indices;
//		// uv
//		Vector2[] uv = new Vector2[N+1];
//		mesh.uv = uv;
//		return mesh;
	}

}
