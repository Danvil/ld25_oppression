using UnityEngine;
using System.Collections;
using MoreLinq;
using System.Linq;

public class Screens : MonoBehaviour {
	
	bool tutorial = true;
	bool gameover = false;
	
	void Awake() {
		MyTime.Pause = true;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameover = !(from x in Globals.Commanders where !x.GetComponent<Person>().IsDead select x).Any();
	}
	
	void OnGUI() {
		if(tutorial) {
			GUI.Label(new Rect(Screen.width/2-200, 120, 400, 20), "Oppression!");
			GUI.Label(new Rect(Screen.width/2-200, 150, 400, 200),
				"Oppress your people by hitting them with your bribed police forces!\n"+
				"Blue=Police, Red=Rebels, Gray=Civilians\n"+
				"\n"+
				"Controls:\n"+
				"Left Click to select commander and move.\n"+
				"Left Alt to toggle rampage mode!\n"+
				"Do not loose your commanders or you loose!");
			if(GUI.Button(new Rect(Screen.width/2-50, 350, 100, 20), "Start!")) {
				tutorial = false;
				MyTime.Pause = false;
			}
		}
		if(gameover) {
			GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-10, 100, 20), "Game Over!");
		}
	}
}
