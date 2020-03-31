using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModulePositionTest : MonoBehaviour {

    ModularRobot testRobot;
    List<GameObject> testObjects;
    GameObject testObject;
    GaitControlTable gct, gct_walker;

    int numberOfModules = 6;

    Vector3[] startPosition = new Vector3[6];
    Vector3 distance;

    Text[] dist = new Text[6];
    int count = 0;

    RobotInsectoid robot;

	void Start () {
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(0, 0, 90);
        testRobot = Robots.CreateSnake(Modules.M3R, Vector3.zero, rot, numberOfModules, robotName: "Position Test");
        // Uncomment this line to specify referenced module id:
        //testRobot.referModuleId = 2;


        for (int i = 0; i < 6; i++)
        {
            dist[i] = UIHelper.AddText("distance");
            dist[i].rectTransform.anchorMax = new Vector2(0.5f, 0.9f - 0.025f * i);
            dist[i].rectTransform.anchorMin = new Vector2(0.5f, 0.9f - 0.025f * i);
        }

        testObjects = new List<GameObject>();
        for (int i = 1; i <= numberOfModules; i++) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(cube.GetComponent(typeof(Collider)));
            cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            cube.GetComponent<Renderer>().material.color = new Color32((byte)(i * 255/numberOfModules),
                                                                       (byte)(255 - i * 255 / numberOfModules),
                                                                       (byte)(255 - i * 255 / numberOfModules),
                                                                       (byte)255);
            testObjects.Add(cube);
        }
        testObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(testObject.GetComponent(typeof(Collider)));
        testObject.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        testObject.GetComponent<Renderer>().material.color = Color.cyan;

        for (int i = 1; i <= 6; i++)
        {
            startPosition[i-1] = testRobot.modules[i].position;
        }

        robot = gameObject.AddComponent<RobotInsectoid>();
        robot.Create(new Vector3 (5, 0.17f, 5));

        gct = robot.gameObject.AddComponent<GaitControlTable>();
        Driver[] drvs = new Driver[12];
        drvs[0] = robot.modules[1, 3].drivers["q1"];
        drvs[1] = robot.modules[1, 2].drivers["q1"];
        drvs[2] = robot.modules[1, 1].drivers["q1"];
        drvs[3] = robot.modules[2, 3].drivers["q1"];
        drvs[4] = robot.modules[2, 2].drivers["q1"];
        drvs[5] = robot.modules[2, 1].drivers["q1"];
        drvs[6] = robot.modules[3, 3].drivers["q1"];
        drvs[7] = robot.modules[3, 2].drivers["q1"];
        drvs[8] = robot.modules[3, 1].drivers["q1"];
        drvs[9] = robot.modules[4, 3].drivers["q1"];
        drvs[10] = robot.modules[4, 2].drivers["q1"];
        drvs[11] = robot.modules[4, 1].drivers["q1"];
        gct.SetHeader(drvs);
        gct.AddLine("0, 10, 80, 0, 45, 45, 0, 10, 80, 0, 45, 45");
        gct.AddLine("45, 10, 80, 0, 45, 45, 45, 10, 80, 0, 45, 45");
        gct.AddLine("45, 45, 45, 0, 45, 45, 45, 45, 45, 0, 45, 45");
        gct.AddLine("45, 45, 45, 0, 10, 80, 45, 45, 45, 0, 10, 80");
        gct.AddLine("-45, 45, 45, 0, 10, 80, -45, 45, 45, 0, 10, 80");
        gct.Begin();

        gct = testRobot.gameObject.AddComponent<GaitControlTable>();
        gct.ReadFromFile(testRobot, "Test_Snake_GCT.txt");
        gct.Begin();
	}

    void Update()
    {
        if (testObjects.Count > 0)
        {
            int count = 1;
            foreach (GameObject cube in testObjects)
            {
                cube.transform.position = testRobot.modules[count].position;
                print(string.Format("Position of Module {0} is {1}.", count, testRobot.modules[count].position.ToString("0.0000")));
                count++;
            }
        }
        if (testObject != null && testRobot != null)
        {
            testObject.transform.position = testRobot.position;
            print(string.Format("Position of Robot \'{0}\' is {1}.", testRobot.name, testRobot.position.ToString("0.0000")));
        }

        count = 1;
        foreach (Text t in dist)
        {
            distance = testRobot.modules[count].position - startPosition[count-1];
            Debug.LogWarning(string.Format("Distance is {1}, vector is {0}.", distance, Mathf.Sqrt(distance.x * distance.x + distance.z * distance.z)));
            dist[count-1].text = string.Format("Distance of M{0} is {1}.", count, Mathf.Sqrt(distance.x * distance.x + distance.z * distance.z).ToString("0.000"));
            count++;
        }
    }
}
