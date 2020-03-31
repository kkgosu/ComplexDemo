using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nikita_Test : MonoBehaviour {

	void Start () {
        var robotWalkerGO = new GameObject();
        var robotWheelGO = new GameObject();
        ModularRobot robotWalker = robotWalkerGO.AddComponent<ModularRobot>();
        ModularRobot robotWheel = robotWheelGO.AddComponent<ModularRobot>();
        robotWalker.Load(Application.dataPath + "/Resources/Configurations/2legs_1.xml"); ;
        robotWheel.Load(Application.dataPath + "/Resources/Configurations/Wheel14.xml");
        GaitControlTable walkerGCT = robotWalkerGO.AddComponent<GaitControlTable>();
        GaitControlTable wheelGCT = robotWheelGO.AddComponent<GaitControlTable>();
        walkerGCT.ReadFromFile(robotWalker, "Legs_GCT.txt");
        wheelGCT.ReadFromFile(robotWheel, "Test_Wheel_GCT.txt");

        wheelGCT.BeginLoop(5);
    }
}