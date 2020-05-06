using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SnakeGoToPoint : MonoBehaviour
{
    WaveController_5 straight;
    SnakeFold fold;
    ModularRobot robot;
    public bool busy = false;
    double maxFoldError = 4;
    double foldError = 2;
    double distError = 0.1;
    double maxDistFor1wave = 1.5;
    double maxHeightOfWave = 1.5;
    double sizeOfModule = 0.38;
    Vector3 robotPosition = new Vector3(0, 0, 0);
    bool correct = false;
    Vector3 position = new Vector3(0, 0, 0);
    bool isReversed = false;
    int modul = 0;
    int middle = 0;

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

   double GetRotationOfRobot(bool isReversed)
    {
        Vector3 posFirst;
        Vector3 posLast;
        posFirst = robot.modules[FirstModule()].position;
        posLast = robot.modules[LastModule()].position;
        return GetRotation(posFirst, posLast);
    }

    public double GetRotation(Vector3 from, Vector3 to)
    {
        double angle;
        double Z = to.z - from.z;
        double X = to.x - from.x;
        double lenght = Math.Sqrt(Math.Pow(Z, 2) + Math.Pow(X, 2));
        angle = (Math.Acos(X / lenght) * (Z/Math.Abs(Z))) * 180 / Math.PI;
        return angle;
    }

    public double GetDistance(Vector3 from, Vector3 to)
    {
        double dist = 0;
        double Z = to.z - from.z;
        double X = to.x - from.x;
        dist = Math.Sqrt(Math.Pow(Z, 2) + Math.Pow(X, 2));
        return dist;
    }

    void Go()
    {
        bool dir = isReversed;
        robotPosition = robot.modules[middle].position;
        double direction = GetRotation(robotPosition, position);
        double distance = Math.Abs(GetDistance(robot.modules[modul].position, position));
        if((((GetRotationOfRobot(isReversed) - direction) > 90) || ((GetRotationOfRobot(isReversed) - direction) < -90)) && (GetDistance(robot.modules[FirstModule()].position, position) < GetDistance(robot.modules[FirstModule()].position, robot.modules[modul].position)))
        {
            dir = !isReversed;
            direction += 180;
            if (direction > 180)
                direction -= 360;
        }
        if (distance < distError && !fold.busy && !straight.busy)
        {
            busy = false;
            if(correct)
            {
                foldError /= 2;
                correct = false;
            }           
        }
        else if (correct)
        {
            if (!straight.busy)
            {
                fold.Rotate((float)(GetRotationOfRobot(isReversed) - direction));
                foldError = maxFoldError/2;
                correct = false;
            }
        }
        else if (distance > distError && Math.Abs(GetRotationOfRobot(isReversed) - direction) > foldError && !fold.busy)
        {
            straight.Stop();
            correct = true;
        }
        else if (distance > distError && !straight.busy && !fold.busy)
        {
            int Dist_in_waves = 0;
            Dist_in_waves = (int) (distance / (maxDistFor1wave* sizeOfModule * 2));
            if (Dist_in_waves > 0)
                straight.Go(Dist_in_waves, maxDistFor1wave, maxHeightOfWave, dir);
            else
                straight.Go(1, distance, (straight.MinMaxH(distance)[0] + straight.MinMaxH(distance)[1]) / 2, dir);
            foldError = maxFoldError;
        }
    }

    public void GoToPoint(Vector3 Position, bool isReversed = false, int modul = 1)
    {
        this.isReversed = isReversed;
        this.position = Position;
        this.modul = modul;
        busy = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        robot = GetComponent<ModularRobot>();
        straight = GetComponent<WaveController_5>();
        if (straight == null)
        {
            straight = gameObject.AddComponent<WaveController_5>();
        }
        fold = GetComponent<SnakeFold>();
        if (fold == null)
        {
            fold = gameObject.AddComponent<SnakeFold>();
        }
        foldError = maxFoldError / 2;
        middle = (int)robot.modules.Count / 2;
        if (middle % 2 != 0)
            middle++;
    }

    // Update is called once per frame
    void Update()
    {
        if(busy)
        {
            Go();
        }
    }
}
