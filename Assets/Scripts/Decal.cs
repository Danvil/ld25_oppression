using UnityEngine;
using System.Collections;

public class Decal : MonoBehaviour {

	public enum FadeMode {
		Constant, FadeInOut, FadeOut
	}

	public FadeMode fadeMode = FadeMode.Constant;

	public float lifetime;
	float timeleft;

	public float wobbelRadius = 0.0f;

	public Transform follow = null;
	
	public float alphaBase = 1.0f;
	
	public Vector3 velocity = Vector3.zero;

	Vector3 wobbelCurrent;
	Vector3 wobbelTarget;

	public Color Color {
		set {
			var renderers = this.GetComponentsInChildren<MeshRenderer>();
			foreach(var r in renderers) {
				r.material.color = value;
				r.material.SetColor("_TintColor", value);
			}
		}
	}

	public float Alpha {
		get {
			return this.GetComponentInChildren<MeshRenderer>().material.GetColor("_TintColor").a;
		}
		set {
			var renderers = this.GetComponentsInChildren<MeshRenderer>();
			foreach(var r in renderers) {
				Color col = r.material.GetColor("_TintColor");
				col.a = value;
				r.material.SetColor("_TintColor", col);
			}
		}
	}

	public bool IsDead {
		get { return timeleft <= 0.0f; }
	}

	// Use this for initialization
	void Start () {
		Alpha = 0.0f;
		wobbelCurrent = Vector3.zero;
		wobbelTarget = wobbelCurrent;
		timeleft = lifetime;
	}

	// Update is called once per frame
	void Update () {
		timeleft -= MyTime.deltaTime;
		switch(fadeMode) {
			case FadeMode.Constant:
				Alpha = alphaBase;
				break;
			case FadeMode.FadeInOut:
				Alpha = alphaBase * MoreMath.Parabel(timeleft, lifetime*0.5f, 1.0f, 0.0f, 0.0f);
				break;
			case FadeMode.FadeOut:
				Alpha = alphaBase * MoreMath.Parabel(timeleft, lifetime, 1.0f, 0.0f, 0.0f);
				break;
		}
		
		// following
		if(follow) {
			this.transform.position = follow.position;
			return;
		}
		
		// wobbeling
		if(wobbelRadius > 0.0f) {
			Vector3 d = wobbelTarget - wobbelCurrent;
			d.z = 0.0f;
			if(d.magnitude <= 0.1f) {
				wobbelTarget = wobbelRadius * MoreMath.RandomInsideUnitCircleXZ;
			}
			else {
				wobbelCurrent += MyTime.deltaTime * d.normalized * 0.1f;
			}
			this.transform.position += wobbelCurrent;
		}
		
		this.transform.position += MyTime.deltaTime * velocity;

	}
}
