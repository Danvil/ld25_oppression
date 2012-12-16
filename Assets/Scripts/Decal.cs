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

	public float fixedZ = Random.Range(-0.001f, +0.001f);
	
	public float alphaBase = 1.0f;

	Vector3 wobbelCurrent;
	Vector3 wobbelTarget;

	public Color Color {
		set {
			renderer.material.color = value;
			renderer.material.SetColor("_TintColor", value);
		}
	}

	public float Alpha {
		get {
			return this.GetComponentInChildren<MeshRenderer>().material.GetColor("_TintColor").a;
		}
		set {
			var renderer = this.GetComponentInChildren<MeshRenderer>();
			Color col = renderer.material.GetColor("_TintColor");
			col.a = value;
			renderer.material.SetColor("_TintColor", col);
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
	
	float alphaFromTimeleft() {
		return 0.25f * MoreMath.Parabel(timeleft, lifetime*0.5f, 1.0f, 0.0f, 0.0f);
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
		}
		else {
			return; // do not move anymore
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
		// set fixed z (C# ftw!)
		Vector3 pTmp = this.transform.position;
		pTmp.y = fixedZ;
		this.transform.position = pTmp;
	}
}
