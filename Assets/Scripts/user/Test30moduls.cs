using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test30moduls : MonoBehaviour
{
    ModularRobot MR;
    GaitControlTable gct;
    GameObject Rob;

    // Use this for initialization
    void Start () {
        CreateRobot();
	}
	void CreateRobot()
    {
        Rob = new GameObject();
        MR = Rob.AddComponent<ModularRobot>();
        MR.Load(Application.dataPath + "/Resources/Configurations/Sphere42.xml", Vector3.zero);
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            var _gct = gameObject.AddComponent<GaitControlTable>();
            _gct.ReadFromFile(MR, "Test_42_Sphere_v1_GCT.txt");
            _gct.BeginTillEnd();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            var _gct = gameObject.AddComponent<GaitControlTable>();
            _gct.ReadFromFile(MR, "Test_42_Sphere_v2_GCT.txt");
            _gct.BeginTillEnd();
        }
    }
}
