using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
	ModularRobot MR;
	GameObject r;
    void Start(){
        CreateRobot();
    }
        /*
		r = new GameObject ();
		//MR = r.AddComponent<ModularRobot> ();
		//MR.Load (Application.dataPath + "/Resources/Configurations/Snake8.xml", new Vector3(2,0,2));
		//MR.ControlTable.Load(Application.dataPath + "/Resources/Gait Control Tables/Snake8.gct", MR.modules);
		//Linear LR = r.AddComponent<Linear>();
		//LR.Create (8, "M3R", alternation: true);
		SimpleModularRobot SMR = r.AddComponent<SimpleModularRobot>();
		SMR.Create (10, "M3R");
		SMR.Reconfugurate (SimpleModularRobot.RobotTypes.Wheel);
		Debug.Log (SMR.modules.Count.ToString());
		SMR.Connect (1, SMR.modules.Count);*/

	void CreateRobot()
	{
		r= new GameObject();
		MR = r.AddComponent<ModularRobot>();
		MR.Load(Application.dataPath + "/Resources/Configurations/Snake8.xml", Vector3.zero);
        //MR.ControlTable.Load(Application.dataPath + "/Resources/Gait Control Tables/Snake4.gct", MR.modules);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
       
	}
}
