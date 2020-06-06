using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SnakeGoToPoint : MonoBehaviour
{
    WaveController_5 Straight;
    SnakeFold Fold;
    ModularRobot robot;
    public bool busy = false;
    double MaxFoldError = 4;
    double foldError = 2;
    double distError = 0.1;
    double Max_dist_for_1_wave = 1.5;
    double Max_height_of_wave = 1.5;
    double SizeOfModule = 0.38;
    Vector3 robotPosition = new Vector3(0, 0, 0);
    bool correct = false;
    Vector3 Position = new Vector3(0, 0, 0);
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
        angle = (Math.Acos(X / lenght) * (Z / Math.Abs(Z))) * 180 / Math.PI;
        print("GetRotation: " + angle);
        return angle;
    }

    public double GetDistance(Vector3 from, Vector3 to)
    {
        double dist = 0;
        double Z = to.z - from.z;
        double X = to.x - from.x;
        dist = Math.Sqrt(Math.Pow(Z, 2) + Math.Pow(X, 2));
        print("GetDistance: " + dist);
        return dist;
    }

    void Go()
    {
        bool dir = isReversed;
        robotPosition = robot.modules[middle].position;
        print("robotPosition: " + robotPosition.x);
        double direction = GetRotation(robotPosition, Position);
        double distance = Math.Abs(GetDistance(robot.modules[modul].position, Position));
        if ((((GetRotationOfRobot(isReversed) - direction) > 90) || ((GetRotationOfRobot(isReversed) - direction) < -90)) && (GetDistance(robot.modules[FirstModule()].position, Position) < GetDistance(robot.modules[FirstModule()].position, robot.modules[modul].position)))
        {
            dir = !isReversed;
            direction += 180;
            if (direction > 180)
                direction -= 360;
        }
        if (distance < distError && !Fold.busy && !Straight.busy)
        {
            busy = false;
            if (correct)
            {
                foldError /= 2;
                correct = false;
            }
        }
        else if (correct)
        {
            if (!Straight.busy)
            {
                Fold.Rotate((float)(GetRotationOfRobot(isReversed) - direction));
                foldError = MaxFoldError / 2;
                correct = false;
            }
        }
        else if (distance > distError && Math.Abs(GetRotationOfRobot(isReversed) - direction) > foldError && !Fold.busy)
        {
            Straight.Stop();
            correct = true;
        }
        else if (distance > distError && !Straight.busy && !Fold.busy)
        {
            int Dist_in_waves = 0;
            Dist_in_waves = (int)(distance / (Max_dist_for_1_wave * SizeOfModule * 2));
            if (Dist_in_waves > 0)
                Straight.Go(Dist_in_waves, Max_dist_for_1_wave, Max_height_of_wave, dir);
            else
                Straight.Go(1, distance, (Straight.MinMaxH(distance)[0] + Straight.MinMaxH(distance)[1]) / 2, dir);
            foldError = MaxFoldError;
        }
    }

    public void GoToPoint(Vector3 Position, bool isReversed = false, int modul = 1)
    {
        this.isReversed = isReversed;
        this.Position = Position;
        this.modul = modul;
        busy = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        robot = GetComponent<ModularRobot>();
        Straight = GetComponent<WaveController_5>();
        if (Straight == null)
        {
            Straight = gameObject.AddComponent<WaveController_5>();
        }
        Fold = GetComponent<SnakeFold>();
        if (Fold == null)
        {
            Fold = gameObject.AddComponent<SnakeFold>();
        }
        foldError = MaxFoldError / 2;
        middle = (int)robot.modules.Count / 2;
        if (middle % 2 != 0)
            middle++;
    }

    // Update is called once per frame
    void Update()
    {
        if (busy)
        {
            Go();
        }
    }
}
