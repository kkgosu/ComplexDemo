using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robots : MonoBehaviour {

    public Robot Build (string xmlPath) {
        Robot robot = new Robot();

        return robot;
    }

    public Robot Create (Module moduleType, Vector3 position, Quaternion rotation)
    {
        Robot robot = new Robot();

        return robot;
    }

    public Robot AddToExistingGameObject (GameObject parent)
    {
        Robot robot = new Robot();

        return robot;
    }

    public static ModularRobot CreateSnake (Module moduleType, Vector3 position, Quaternion rotation,
                              int modulesCount, int startingID = 1, string robotName = "") {
        var GO = new GameObject();
        GO.transform.name = string.Format("Robot {0}", robotName);
        ModularRobot robot = GO.AddComponent<ModularRobot>();
        robot.modules.Add(startingID, Modules.Create(moduleType, position, rotation, parent: GO.transform, ID: startingID));
        for (int i = 1; i < modulesCount; i++)
            robot.modules.Add(startingID + i,
                              robot.modules[startingID + i - 1].surfaces["top"].Add(moduleType, moduleType.surfaces["bottom"], parent: GO.transform, ID: startingID + i));
        return robot;
    }

    public static ModularRobot CreateWheel (Module moduleType, Vector3 position, Quaternion rotation,
                          int modulesCount, int startingID = 1, int arcCount = 4, float arcAngle = 45f, float linearAngle = 0f, string robotName = "")
    {
        var GO = new GameObject();
        GO.transform.name = string.Format("Robot {0}", robotName);
        ModularRobot robot = GO.AddComponent<ModularRobot>();
        int arc1end = startingID + arcCount - 1;
        int arc2start = startingID + (modulesCount / 2);
        int arc2end = arc2start + arcCount - 1;
        robot.modules.Add(startingID, Modules.Create(moduleType, position, rotation, angles: new float[] {arcAngle * (-1)}, parent: GO.transform, ID: startingID));
        for (int i = 1; i < modulesCount; i++)
            robot.modules.Add(startingID + i,
                              robot.modules[startingID + i - 1].surfaces["top"].Add(moduleType,
                                                                                    moduleType.surfaces["bottom"], ID: startingID + i,
                                                                                    parent: GO.transform,
                                                                                    angles: new float[] { (startingID + i) <= arc1end || 
                                                                                    ((startingID + i) >= arc2start && (startingID + i) <= arc2end)
                                                                                    ? arcAngle * (-1) : linearAngle * (-1)}
                                                                                   ));
        robot.modules[startingID + modulesCount - 1].surfaces["top"].Connect(robot.modules[startingID].surfaces["bottom"]);
        return robot;
    }
}