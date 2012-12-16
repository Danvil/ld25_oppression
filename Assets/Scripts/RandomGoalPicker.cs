using UnityEngine;

public class RandomGoalPicker : MonoBehaviour
{
	const float GOAL_REACHED_TOLERANCE = 0.01f;
	
	public bool IsEnabled { get; set; }
	
	public Vector3 Goal { get; private set; }
	
	public Vector3 Force { get; private set; }
	
	float goalWaitTime;
	float goalFollowTime;

	bool isGoalReached() {
		return (this.transform.position - Goal).magnitude < GOAL_REACHED_TOLERANCE;
	}
	
	void setNewGoal() {
		for(int i=0; i<3; i++) {
			// create new random goal
			Vector3 ngoal = this.transform.position + MoreMath.RandomInsideBox(new Vector3(-1,0,-1), new Vector3(+1,0,+1));
				//MoreMath.RandomInsideBox(Globals.City.SizeMin, Globals.City.SizeMax);
			// check if goal is reachable
			if(!Globals.City.IsBlocked(ngoal)) {
				Goal = ngoal;
				goalFollowTime = Random.Range(3.0f, 5.0f);
				break;
			}
		}
		goalWaitTime = Random.Range(0.0f, 2.0f);
	}
	
	void Start() {
		IsEnabled = true;
		Goal = this.transform.position;
		goalWaitTime = 0.0f;
		goalFollowTime = 0.0f;
	}

	void Update() {
		Force = Vector3.zero;
		if(!IsEnabled) {
			return;
		}
		goalFollowTime -= MyTime.deltaTime;
		if(goalFollowTime < 0.0f || isGoalReached()) {
			goalWaitTime -= MyTime.deltaTime;
			if(goalWaitTime < 0.0f) {
				setNewGoal();
			}
		}
		if(!isGoalReached()) {
			Force = (Goal - this.transform.position).normalized;
		}
	}
}
