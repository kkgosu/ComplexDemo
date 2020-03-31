using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class GaitControlTable_Test : MonoBehaviour {

    GaitControlTable gct;
    ModularRobot robot;
	WaveController Wc;

    public bool inProgress = false;

	// Use this for initialization
	void Start () {
        /*gct = transform.gameObject.AddComponent<GaitControlTable>();
        var module = Modules.Create(Modules.M3R, Vector3.zero, new Quaternion(0,0,0,0));

        gct.SetHeader (new Driver[] {module.drivers.q1, module.drivers.q1, module.drivers.q1, module.drivers.q1});
        Debug.LogError("Length is " + gct.header.Count);
        string[] lines = {
            "--15, +10 (0.3), max, --10",
            "30     (1.2), --40, 0, min (0.5)",
            "30, --15,     ++15, default",
            "--min, 0, 0, 0",
            "12, abg, 34, 56",
            "--30, 10, ---10, 10",
            "--5",
            "10 - default, max - 5, min + 45, default"
        };
        foreach (string line in lines)
            gct.AddLine(line);
        */
        robot = gameObject.AddComponent<ModularRobot>();
		robot.Load(Application.dataPath + "/Resources/Configurations/Snake13.xml");
		var wg = gameObject.AddComponent<WaveControl> ();
		wg.AddModulesFromRobot (robot);
        //robot.gameObject.AddComponent<COM_Controller>();
		Wc = gameObject.AddComponent<WaveController> ();
		Wc.AddModulesFromRobot (robot);
		GetComponent<WaveController> ().Init ();
        /*
        var _gct = gameObject.AddComponent<GaitControlTable>();
        _gct.ReadFromFile(robot, "Test_2_Snake_GCT.txt");
        _gct.Begin();
        */
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !inProgress)
        {
            inProgress = true;
            //var _gct = gameObject.AddComponent<GaitControlTable>();
            //_gct.ReadFromFile(robot, "Test_2_Snake_GCT.txt");
            //_gct.BeginLoop();
            //_gct.isKeyboardControlled = true;
        }

    }
}
