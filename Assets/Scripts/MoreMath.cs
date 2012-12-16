using UnityEngine;

public static class MoreMath
{
	/** Parabel definiert durch Scheitelpunkt (sx, sy) und Steigung a
	 * http://de.wikipedia.org/wiki/Parabel_%28Mathematik%29#Scheitelform
	 */
	public static float Parabel(float x, float sx, float sy, float a) {
		float q = x - sx;
		return a*q*q + sy;
	}

	/** Parabel definiert durch Scheitelpunkt (sx, sy) und einen weiteren Punkt (px, py) */
	public static float Parabel(float x, float sx, float sy, float px, float py) {
		float dx = px - sx;
		float a = (py - sy) / (dx * dx);
		return Parabel(x, sx, sy, a);
	}

	public static float Clamp(float x, float min, float max) {
		return Mathf.Max(min, Mathf.Min(max, x));
	}

	public static int Clamp(int x, int min, int max) {
		return (x < min ? min : (x > max ? max : x));
	}

	/** Computes a an integer and 0 <= b < n, such that a*n + b = i and returns b. */
	public static int ModPos(int i, int n) {
		while(i < 0) i += n;
		return i % n;
	}

	public static float Clamp(float x) {
		return Clamp(x, 0.0f, 1.0f);
	}

	public static float VectorAngle(Vector3 v) {
		return Mathf.Atan2(v.z, v.x);
	}

	public static Quaternion RotAngle(float angle) {
		return Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.up);
	}

	public static Quaternion RotHeading(Vector3 heading) {
		return RotAngle(VectorAngle(heading));
	}

	public static Color IndicatorColor(float x) {
		x = Clamp(x, 0, 1);
		return new Color(1-x, x, 0);
	}

	public static Color IndicatorColorPosNeg(float x) {
		x = Clamp(x, -1, 1);
		return new Color(0.5f*(1.0f + x), 0.0f, 0.5f*(1.0f - x));
	}

	public static Vector3 RandomInsideUnitCircleXZ {
		get {
			Vector2 u = Random.insideUnitCircle;
			return new Vector3(u.x, 0.0f, u.y);
		}
	}

	public static Vector3 RandomInsideBox(Vector3 min, Vector3 max) {
		return new Vector3(
			Random.Range(min.x,max.x),
			Random.Range(min.y,max.y),
			Random.Range(min.z,max.z));
	}

	public static float PoissonProbability(float frequency, float dt) {
		return 1.0f - Mathf.Exp(- frequency * dt);
	}

	public static bool CheckOccurence(float frequency) {
		return Random.value < PoissonProbability(frequency, MyTime.deltaTime);
	}

	public static float SlerpAngle(float x, float y, float p) {
		float d = y - x;
		if(d > Mathf.PI) {
			d = 2.0f * Mathf.PI - d;
		}
		return x + p * d;
	}

}