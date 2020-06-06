using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SnakeClimbToWheel : MonoBehaviour
{
    SnakeGoToPoint GoTo;
    SnakeFold Fold;
    ModularRobot robot;
    public ModularRobot wheel;
    bool busy = false;
    public Vector3 Point = new Vector3(0, 0, 0);
    int modToGo = 0;
    public int step = 0;
    bool isReversed = true;
    double angleSide = 0;
    SnakeSideways Side;
    double dist;
    Vector3 znack = new Vector3(0, 0, 0);


    int FirstModule()
    {
        if (isReversed)
            return robot.modules.Count;
        else
            return 1;
    }
    int LastModule()
    {
        if (isReversed)
            return 1;
        else
            return robot.modules.Count;
    }

    int index(int nomer)
    {
        while (nomer > wheel.modules.Count)
            nomer -= wheel.modules.Count;
        while (nomer < 0)
            nomer += wheel.modules.Count;
        return nomer;
    }

    public void ClimbOnWheel(ModularRobot wheel)
    {
        this.wheel = wheel;
        modToGo = 0;
        double dist = 10000;
        for (int i = 0; i < wheel.modules.Count; i++)
        {
            if (wheel.modules[i].drivers["q1"].qValue != 0)
            {
                if (GoTo.GetDistance(robot.modules[8].position, wheel.modules[i].position) < dist)
                {
                    modToGo = i;
                    dist = GoTo.GetDistance(robot.modules[8].position, wheel.modules[i].position);
                }
            }
        }
        step = 1;
        busy = true;
        isReversed = true;
        this.dist = 1000;
    }
    public bool DriversAreReady()
    {
        foreach (Module mod in robot.modules.Values)
        {
            if (mod.drivers["q1"].busy)
                return false;
        }
        return true;
    }

    public void Go()
    {

        if (!GoTo.busy && DriversAreReady())
        {
            double angleBefor = GoTo.GetRotation(wheel.modules[modToGo].position, wheel.modules[index(modToGo + wheel.modules.Count / 2)].position);
            angleBefor -= 180;
            angleBefor = angleBefor * Mathf.PI / 180;
            Vector3 posBefor = new Vector3((float)(wheel.modules[modToGo].position.x + 0.38 * 1.8 * Math.Cos(angleBefor)), 0, (float)(wheel.modules[modToGo].position.z + 0.38 * 1.8 * Math.Sin(angleBefor)));
            print("posBeforeX: " + posBefor.x);
            print("posBeforeY: " + posBefor.y);
            print("posBeforeZ: " + posBefor.z);
            Point = posBefor;
            switch (step)
            {
                case 1:
                    angleSide = GoTo.GetRotation(wheel.modules[modToGo].position, robot.modules[8].position);
                    angleSide = angleSide * Mathf.PI / 180;
                    Vector3 a = new Vector3(Mathf.Cos((float)angleBefor), 0, Mathf.Sin((float)angleBefor));
                    Vector3 b = new Vector3(Mathf.Cos((float)angleSide), 0, Mathf.Sin((float)angleSide));
                    znack = Vector3.Cross(a, b);
                    angleSide = angleBefor - (Math.PI / 2) * (znack.y / Math.Abs(znack.y));
                    if (angleSide > Math.PI)
                        angleSide -= (2 * Math.PI);
                    if (angleSide < (Math.PI * (-1)))
                    {
                        angleSide += (2 * Math.PI);
                    }
                    Vector3 posSide = new Vector3((float)(posBefor.x + 0.38 * 9 * Math.Cos(angleSide)), 0, (float)(posBefor.z + 0.38 * 9 * Math.Sin(angleSide)));

                    GoTo.GoToPoint(posSide, isReversed, 8);
                    step++;
                    break;
                case 2:
                    Vector3 q = new Vector3((float)(posBefor.x + 0.38 * 0.4 * Math.Cos(angleSide)), 0, (float)(posBefor.z + 0.38 * 0.4 * Math.Sin(angleSide)));
                    GoTo.GoToPoint(q, isReversed, 8);
                    step++;
                    break;
                case 3:
                    if (!Fold.busy)
                    {
                        if (Math.Abs(GoTo.GetRotation(robot.modules[LastModule()].position, robot.modules[FirstModule()].position) - (angleSide * 180 / Math.PI)) > 1)
                        {
                            //float rot = (float)(GoTo.GetRotation(robot.modules[LastModule()].position, robot.modules[FirstModule()].position) - (angleSide * 180 / Math.PI));
                            Fold.Rotate((float)(GoTo.GetRotation(robot.modules[LastModule()].position, robot.modules[FirstModule()].position) - (angleSide * 180 / Math.PI)));
                        }
                        else
                            step++;
                    }
                    break;
                case 4:
                    Side.Move(-1 * znack.y / Math.Abs(znack.y));
                    step++;
                    break;
                case 5:
                    if (!Side.busy && !Fold.busy)
                    {
                        if (Math.Abs(GoTo.GetRotation(robot.modules[LastModule()].position, robot.modules[FirstModule()].position) - (angleSide * 180 / Math.PI)) > 1)
                        {
                            //float rot = (float)(GoTo.GetRotation(robot.modules[LastModule()].position, robot.modules[FirstModule()].position) - (angleSide * 180 / Math.PI));
                            Fold.Rotate((float)(GoTo.GetRotation(robot.modules[LastModule()].position, robot.modules[FirstModule()].position) - (angleSide * 180 / Math.PI)));
                        }
                        else
                            step++;
                    }
                    break;
                case 6:
                    robot.modules[10].drivers["q1"].speed /= 2;
                    robot.modules[6].drivers["q1"].speed /= 2;
                    robot.modules[10].drivers["q1"].Set(60);
                    robot.modules[6].drivers["q1"].Set(60);
                    step++;
                    break;
                case 7:
                    robot.modules[10].drivers["q1"].speed *= 2;
                    robot.modules[6].drivers["q1"].speed *= 2;
                    robot.modules[13].drivers["q1"].Set(90 * -1 * znack.y / Math.Abs(znack.y));
                    robot.modules[3].drivers["q1"].Set(90 * znack.y / Math.Abs(znack.y));
                    robot.modules[12].drivers["q1"].Set(30);
                    robot.modules[4].drivers["q1"].Set(30);
                    robot.modules[14].drivers["q1"].Set(90);
                    robot.modules[2].drivers["q1"].Set(90);
                    step++;
                    break;
                case 8:
                    //robot.modules[4].drivers["q1"].Set(0);
                    //robot.modules[6].drivers["q1"].Set(90);
                    robot.modules[12].drivers["q1"].Set(0);
                    robot.modules[10].drivers["q1"].Set(90);
                    step++;
                    break;
                case 9:
                    if (dist < GoTo.GetDistance(robot.modules[15].position, wheel.modules[15].position))
                    {
                        robot.modules[15].surfaces["top"].Connect(wheel.modules[15].surfaces["left"]);
                        robot.modules[3].drivers["q1"].Set(90 * -1 * znack.y / Math.Abs(znack.y));
                        step++;
                        dist = 1000;
                    }
                    else
                        dist = GoTo.GetDistance(robot.modules[15].position, wheel.modules[15].position);
                    break;
                case 10:
                    robot.modules[4].drivers["q1"].Set(0);
                    robot.modules[6].drivers["q1"].Set(90);
                    step++;
                    break;
                case 11:
                    if (dist < GoTo.GetDistance(robot.modules[1].position, wheel.modules[15].position))
                    {
                        robot.modules[1].surfaces["bottom"].Connect(wheel.modules[15].surfaces["right"]);
                        step++;
                        dist = 1000;
                    }
                    else
                        dist = GoTo.GetDistance(robot.modules[1].position, wheel.modules[15].position);
                    break;
                case 12:
                    robot.modules[13].drivers["q1"].Set(0);
                    robot.modules[3].drivers["q1"].Set(0);
                    step++;
                    break;
                case 13:
                    step = 0;
                    busy = false;
                    break;

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        robot = GetComponent<ModularRobot>();
        GoTo = GetComponent<SnakeGoToPoint>();
        if (GoTo == null)
        {
            GoTo = gameObject.AddComponent<SnakeGoToPoint>();
        }
        Fold = GetComponent<SnakeFold>();
        if (Fold == null)
        {
            Fold = gameObject.AddComponent<SnakeFold>();
        }
        Side = GetComponent<SnakeSideways>();
        if (Side == null)
        {
            Side = gameObject.AddComponent<SnakeSideways>();
        }
        step = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ClimbOnWheel(GameObject.Find("Modular Robot test123").GetComponent<ModularRobot>());
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Side.Move(1);
        }
        if (busy)
        {
            Go();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Fold.Rotate(90);
        }
    }
}
