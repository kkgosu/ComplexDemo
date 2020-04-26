using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeLocomotionManager : MonoBehaviour {

	ModularRobot robot;
    public Vector3 position;

	void Start () {
		robot = gameObject.AddComponent<ModularRobot>();
		robot.Load(Application.dataPath + "/Resources/Configurations/Turning Snake13.xml");
        gameObject.AddComponent<lineRobAdmin>();
	}
	
	// Update is called once per frame
	void Update () {
        if (robot != null)
            position = robot.position;
	}
}
