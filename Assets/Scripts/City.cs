using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class City : MonoBehaviour {
	
	public GameObject pfBuilding;
	public GameObject pfStreet;
	
	List<Building> buildings = new List<Building>();
	List<Street> streets = new List<Street>();
	
	public Vector2 SizeMin {
		get; private set;
	}
	
	public Vector2 SizeMax {
		get; private set;
	}
	
	void Awake() {
		if(Globals.City != null) {
			throw new Exception("Only one city allowed!");
		}
		Globals.City = this;
	}

	// Use this for initialization
	void Start () {
		createCityRegular();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void createCityRegular() {
		const int n = 3;
		const int size = 1 + n*4;
		// init city layer
		int[,] layer = new int[size,size];
		for(int i=0; i<size; i++) {
			for(int j=0; j<size; j++) {
				layer[i,j] = 0;
			}
		}
		// set houses (3x3)
		for(int i=0; i<n; i++) {
			for(int j=0; j<n; j++) {
				int x = 1 + j*4;
				int y = 1 + i*4;
				if(i==n/2 && j==n/2) {
					// square
				}
				else {
					layer[x,y] = 1;
					layer[x+1,y] = 1;
					layer[x+2,y] = 1;
					layer[x,y+1] = 1;
					layer[x+2,y+1] = 1;
					layer[x,y+2] = 1;
					layer[x+1,y+2] = 1;
					layer[x+2,y+2] = 1;
				}
			}
		}
		// create
		SizeMin = new Vector3(-size/2, 0, -size/2);
		SizeMax = new Vector3(size-size/2+1, 0, size-size/2+1);
		for(int i=0; i<size; i++) {
			for(int j=0; j<size; j++) {
				GameObject u;
				if(layer[i,j] == 1) {
					// create building
					u = (GameObject)Instantiate(pfBuilding);
					buildings.Add(u.GetComponent<Building>());
				}
				else {
					// create street
					u = (GameObject)Instantiate(pfStreet);
					streets.Add(u.GetComponent<Street>());
				}
				u.transform.position = new Vector3(j-size/2,0,i-size/2);
				u.transform.parent = this.transform;
			}
		}
	}
}
