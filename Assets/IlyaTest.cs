using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IlyaTest : MonoBehaviour {

	ModularRobot robot;
	WaveController Wc;
	// Use this for initialization
	void Start () {
		robot = gameObject.AddComponent<ModularRobot>();
		robot.Load(Application.dataPath + "/Resources/Configurations/Turning Snake13.xml");
		var wg = gameObject.AddComponent<WaveControl> ();
		wg.AddModulesFromRobot (robot);
		Wc = gameObject.AddComponent<WaveController> ();
		Wc.AddModulesFromRobot (robot);
		GetComponent<WaveController> ().Init ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
