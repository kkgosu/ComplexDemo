using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformations : MonoBehaviour
{
    private ModularRobot MR;
    private int midModule;
    private int centralModule;
    private int leftMidModule;
    private int rightMidModule;

    List<int> rightModules = new List<int>();
    List<int> leftModules = new List<int>();

    List<int> firstLeg = new List<int>();
    List<int> secondLeg = new List<int>();
    List<int> thirdLeg = new List<int>();
    List<int> fourthLeg = new List<int>();

    Dictionary<int, Module> modulez;

    private bool snakeToWheel;
    private bool isClose;

    public IEnumerator MakeSnake()
    {
        for (int i = 0; i < MR.modules.Count; i++)
        {
            if (i % 2 == 0)
            {
                MR.modules[i].drivers["q2"].Set(90);
            } else
            {
                MR.modules[i].drivers["q2"].Set(-90);
            }
        }

        yield return WaitWhileDriversAreBusy();
    }

    public IEnumerator Execute(params Func<IEnumerator>[] actions)
    {
        foreach (Func<IEnumerator> action in actions)
        {
            yield return StartCoroutine(action());
        }
    }

    public IEnumerator WheelToSnake()
    {
        midModule = GetTopMidModule();
        if (midModule != -1)
        {
            GaitControlTable controlTable = MR.gameObject.GetComponent<GaitControlTable>();

            MR.modules[midModule].surfaces["top"].Disconnect();
            MR.angles = WheelToSnakeAngles(MR.angles);
            controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, 2));

            yield return StartCoroutine(WaitUntilMoveEnds(controlTable));
        }
        print("MidModule: " + midModule);
        print("TAGG: WheelToSnake");
        yield return null;
    }

    public IEnumerator SnakeToWalker()
    {
        centralModule = GetCentralModule();
        print("Central Module: " + centralModule);

        yield return Execute(
            NewSnakeToWalker2Angles,
            SnakeToWalker3Angles,
            SnakeToWalker4Angles,
            SnakeToWalker5Angles
            );
    }

    public IEnumerator WalkerToSnake()
    {
        yield return Execute(
            WalkerToSnake1Angles,
            WalkerToSnake2Angles,
            WalkerToSnake3Angles,
            WalkerToSnake4Angles
            );
    }

    public IEnumerator SnakeToWheel()
    {
        yield return Execute(
            SnakeToWheel1
            );
    }

    private IEnumerator SnakeToWheel1()
    {
        int total = MR.modules.Count;
        float angle = 360f / total;

        for (int i = 0; i < total; i++)
        {
            MR.modules[i].drivers["q2"].Set(0);
        }
        yield return WaitWhileDriversAreBusy();
        snakeToWheel = true;

        int counter = 2;
        MR.modules[total - 1].drivers["q1"].Set(-10);
        while (!isClose)
        {
            if (counter < total / 5)
            {
                MR.modules[counter - 1].drivers["q1"].Set(0);
                MR.modules[counter + 1].drivers["q1"].Set(90);
                MR.modules[counter].drivers["q1"].Set(90);
            }

            MR.modules[total - counter].drivers["q1"].Set(0);
            MR.modules[total - counter - 1].drivers["q1"].Set(87);
            MR.modules[total - counter - 2].drivers["q1"].Set(87);
            yield return WaitWhileDriversAreBusy();

            counter++;
        }

        for (int i = 0; i < total; i++)
        {
            MR.modules[i].drivers["q1"].Set(angle);
        }

        yield return WaitWhileDriversAreBusy();
    }

    private IEnumerator WalkerToSnake5Angles()
    {
        print("WalkerToSnake5Angles");
        GaitControlTable controlTable = MR.gameObject.GetComponent<GaitControlTable>();
        RenameModulesForSnake();
        MR.angles = new float[MR.angles.Length];
        
        for (int i = 0; i < MR.modules.Count; i++)
        {
                MR.modules[i].drivers["q1"].Set(0);
        }

        

        for (int i = 0; i < MR.modules.Count; i++)
        {
            if (MR.modules[i].drivers["q2"].qValue != 0)
            {
                MR.modules[i].drivers["q2"].Set(0);
            }
        }

        yield return WaitWhileDriversAreBusy();

        MR.modules[1].drivers["q2"].Set(90);
        MR.modules[9].drivers["q2"].Set(90);
        MR.modules[10].drivers["q2"].Set(-90);
        MR.modules[11].drivers["q2"].Set(-90);
        MR.modules[12].drivers["q2"].Set(90);
        MR.modules[MR.modules.Count - 1].drivers["q2"].Set(90);

        yield return WaitWhileDriversAreBusy();

        for (int i = 0; i < MR.modules.Count - 1; i++)
        {
            while (IfAnyDriverIsBusy())
            {
                yield return new WaitForEndOfFrame();
            }
            float diff = MR.modules[i].drivers["q2"].qValue + MR.modules[i + 1].drivers["q2"].qValue;

            print("Diff: " + diff + ", when modules " + i + "=" + MR.modules[i].drivers["q2"].qValue + ", modules " + (i + 1 + "=" + MR.modules[i + 1].drivers["q2"].qValue));

            if (diff == 0)
            {
                if (i % 2 == 0)
                {
                    MR.modules[i].drivers["q2"].Set(-90);
                }
                else
                {
                    MR.modules[i].drivers["q2"].Set(90);
                }
            }
        }
    }
    private IEnumerator WaitWhileDriversAreBusy()
    {
        while (IfAnyDriverIsBusy())
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private bool IfAnyDriverIsBusy()
    {
        bool flag = false;
        foreach (Module module in MR.modules.Values)
        {
            foreach (Driver driver in module.drivers.Values)
            {
                if (driver.busy)
                {
                    flag = true;
                }
            }
        }
        return flag;
    }

    private IEnumerator WalkerToSnake4Angles()
    {
        for (int i = 0; i < MR.modules.Count; i++)
        {
            MR.modules[i].drivers["q1"].Set(0);
        }
        yield return WaitWhileDriversAreBusy();

        RenameModulesForSnake();
    }

    private IEnumerator WalkerToSnake1Angles()
    {
        for (int i = 0; i < MR.modules.Count; i++)
        {
            MR.modules[i].drivers["q1"].Set(0);
        }

        yield return WaitWhileDriversAreBusy();
    }

    private IEnumerator WalkerToSnake2Angles()
    {
        int connectedModule;
        firstLeg.Clear();
        secondLeg.Clear();
        thirdLeg.Clear();
        fourthLeg.Clear();

        connectedModule = 0;
        print("firstLeg!!");
        while (MR.modules[connectedModule].surfaces["bottom"].connectedSurface != null)
        {
            connectedModule = MR.modules[connectedModule].surfaces["bottom"].connectedSurface.module.id;
            firstLeg.Add(connectedModule);
        }

        connectedModule = 0;
        print("secondLeg!!");
        while (MR.modules[connectedModule].surfaces["top"].connectedSurface != null)
        {
            if (connectedModule == 0)
            {
                connectedModule = MR.modules[connectedModule].surfaces["right"].connectedSurface.module.id;
            }
            else
            {
                connectedModule = MR.modules[connectedModule].surfaces["top"].connectedSurface.module.id;
            }
            secondLeg.Add(connectedModule);
        }

        connectedModule = 0;
        print("thirdLeg!!");
        while (MR.modules[connectedModule].surfaces["top"].connectedSurface != null)
        {
            connectedModule = MR.modules[connectedModule].surfaces["top"].connectedSurface.module.id;
            thirdLeg.Add(connectedModule);
        }

        connectedModule = 0;
        print("fourthLeg!!");
        while (MR.modules[connectedModule].surfaces["bottom"].connectedSurface != null)
        {
            if (connectedModule == 0)
            {
                connectedModule = MR.modules[connectedModule].surfaces["left"].connectedSurface.module.id;
            }
            else
            {
                connectedModule = MR.modules[connectedModule].surfaces["bottom"].connectedSurface.module.id;
            }
            fourthLeg.Add(connectedModule);
        }

        MR.modules[0].drivers["q2"].Set(0);
        MR.modules[firstLeg[0]].drivers["q2"].Set(0);
        MR.modules[firstLeg[1]].drivers["q2"].Set(90);
        MR.modules[secondLeg[0]].drivers["q2"].Set(0);
        MR.modules[thirdLeg[0]].drivers["q2"].Set(90);
        MR.modules[fourthLeg[1]].drivers["q2"].Set(0);

        yield return WaitWhileDriversAreBusy();

        MR.modules[firstLeg[firstLeg.Count - 1]].drivers["q1"].Set(55);
        MR.modules[secondLeg[secondLeg.Count - 1]].drivers["q1"].Set(35);
        MR.modules[thirdLeg[thirdLeg.Count - 1]].drivers["q1"].Set(45);
        MR.modules[fourthLeg[fourthLeg.Count - 1]].drivers["q1"].Set(45);

        yield return WaitWhileDriversAreBusy();

        MR.modules[1].drivers["q1"].Set(-19);
        MR.modules[2].drivers["q1"].Set(13);
        MR.modules[3].drivers["q1"].Set(-12);
        MR.modules[4].drivers["q1"].Set(12);

        yield return WaitWhileDriversAreBusy();
    }

    private IEnumerator WalkerToSnake3Angles()
    {
        MR.modules[9].drivers["q1"].Set(90);
        MR.modules[10].drivers["q1"].Set(80);
        MR.modules[11].drivers["q1"].Set(90);
        MR.modules[12].drivers["q1"].Set(90);
        yield return WaitWhileDriversAreBusy();

        MR.modules[secondLeg[secondLeg.Count - 1]].surfaces["top"].Connect(MR.modules[firstLeg[firstLeg.Count - 1]].surfaces["bottom"]);
        MR.modules[thirdLeg[thirdLeg.Count - 1]].surfaces["top"].Connect(MR.modules[fourthLeg[fourthLeg.Count - 1]].surfaces["bottom"]);
        MR.modules[secondLeg[0]].surfaces["bottom"].Disconnect();
        MR.modules[fourthLeg[0]].surfaces["top"].Disconnect();

        yield return WaitWhileDriversAreBusy();
    }

    public double GetRotation(Vector3 from, Vector3 to)
    {
        double angle;
        double Z = to.z - from.z;
        double X = to.x - from.x;
        double lenght = Math.Sqrt(Math.Pow(Z, 2) + Math.Pow(X, 2));
        angle = (Math.Acos(X / lenght) * (Z / Math.Abs(Z))) * 180 / Math.PI;
        return angle;
    }

    private double GetDistance(Vector3 from, Vector3 to)
    {
        double dist = 0;
        double Z = to.z - from.z;
        double X = to.x - from.x;
        dist = Math.Sqrt(Math.Pow(Z, 2) + Math.Pow(X, 2));
        return dist;
    }



    private float[] WheelToSnakeAngles(float[] angles)
    {
        int offset = angles.Length / 4;
        int leftModule = midModule - offset;
        int rightModule = midModule + offset;

        if (leftModule < 0)
        {
            leftModule = angles.Length + leftModule;
        }
        if (rightModule >= angles.Length)
        {
            rightModule = angles.Length - rightModule;
        }

        int leftSecondModule = CheckEdgeModule(angles, leftModule);
        int rightSecondModule = CheckEdgeModule(angles, rightModule);

        for (int i = 0; i < angles.Length; i++)
        {
            angles[i] = 0;
        }

        angles[leftModule] = 90;
        angles[leftSecondModule] = 90;
        angles[rightModule] = 90;
        angles[rightSecondModule] = 90;

        print(leftModule + " " + leftSecondModule + " " + rightModule + " " + rightSecondModule);

        return angles;
    }

    private IEnumerator NewSnakeToWalker2Angles()
    {
        for (int i = 0; i < MR.modules.Count; i++)
        {
            MR.modules[i].drivers["q2"].Set(0);
        }
        yield return WaitWhileDriversAreBusy();

        int total = MR.modules.Count;
        int rightPart = total / 3;
        int leftPart = total / 3;
        if (total % 2 == 1)
        {
            rightPart = total / 3;
            leftPart = total / 3 + 1;
        }
        //получаем список "правых" и "левых" модулей
        int connectedModule = centralModule;
        while (MR.modules[connectedModule].surfaces["bottom"].connectedSurface != null)
        {
            connectedModule = MR.modules[connectedModule].surfaces["bottom"].connectedSurface.module.id;
            rightModules.Add(connectedModule);
        }

        connectedModule = centralModule;
        while (MR.modules[connectedModule].surfaces["top"].connectedSurface != null)
        {
            connectedModule = MR.modules[connectedModule].surfaces["top"].connectedSurface.module.id;
            leftModules.Add(connectedModule);
        }

        //оставляем только ту часть, которая будет сгибаться
        List<int> halfRightModules = rightModules.GetRange(rightModules.Count - rightPart, rightPart);
        List<int> halfLeftModules = leftModules.GetRange(leftModules.Count - leftPart, leftPart);

        yield return WaitWhileDriversAreBusy();

        MR.modules[halfLeftModules[0]].drivers["q2"].Set(90);
        MR.modules[halfRightModules[0]].drivers["q2"].Set(90);
        yield return WaitWhileDriversAreBusy();

        MR.modules[halfLeftModules[1]].drivers["q1"].Set(90);
        MR.modules[halfRightModules[0]].drivers["q1"].Set(90);

        StartCoroutine(Test(halfRightModules));
        StartCoroutine(Test1(halfLeftModules));

        yield return WaitWhileDriversAreBusy();
    }

    private IEnumerator Test(List<int> modules)
    {
        CreateCFG createCFG = GetComponent<CreateCFG>();
        float a = Mathf.Rad2Deg * createCFG.NewtonRaphson(4) * 1.41f;
        int total = 21;
        int rightPart = total / 3;
        float angleRight = a / rightPart;

        foreach (int id in modules)
        {
            print("Right: " + id);
            if (id != 1)
                MR.modules[id].drivers["q1"].Set(angleRight);
        }
        yield return WaitWhileDriversAreBusy();
        MR.modules[modules[modules.Count - 1]].drivers["q1"].Set(90 - 360f / (4.6f * 2));
        MR.modules[modules[0]].drivers["q1"].Set(90);
        yield return WaitWhileDriversAreBusy();
    }

    private IEnumerator Test1(List<int> modules)
    {
        CreateCFG createCFG = GetComponent<CreateCFG>();
        float a = Mathf.Rad2Deg * createCFG.NewtonRaphson(4) * 1.25f;
        int total = 21;
        int leftPart = total / 3 - 1;
        float angleLeft = a / leftPart;

        for (int i = 1; i < modules.Count; i++)
        {
            if (modules[i] != 19)
                MR.modules[modules[i]].drivers["q1"].Set(angleLeft);
        }
        yield return WaitWhileDriversAreBusy();
        MR.modules[modules[modules.Count - 1]].drivers["q1"].Set(90 - 360f / (4.6f * 2));
        MR.modules[modules[1]].drivers["q1"].Set(90);
        MR.modules[modules[2]].drivers["q1"].Set(MR.modules[modules[2]].drivers["q1"].qValue += 20);
        yield return WaitWhileDriversAreBusy();
        yield return new WaitForSeconds(1f);

    }

    private float[] SnakeToWalker2Angles(float[] angles)
    {
        CreateCFG createCFG = GetComponent<CreateCFG>();
        float a = Mathf.Rad2Deg * createCFG.NewtonRaphson(4) * 2;

        int rightPart = angles.Length / 3;
        int leftPart;

        if (angles.Length % 2 == 1)
        {
            leftPart = angles.Length / 3 + 1;
        }
        else
        {
            leftPart = angles.Length / 3;
        }

        //получаем список "правых" и "левых" модулей
        int connectedModule = centralModule;
        while (MR.modules[connectedModule].surfaces["bottom"].connectedSurface != null)
        {
            connectedModule = MR.modules[connectedModule].surfaces["bottom"].connectedSurface.module.id;
            rightModules.Add(connectedModule);
        }

        connectedModule = centralModule;
        while (MR.modules[connectedModule].surfaces["top"].connectedSurface != null)
        {
            connectedModule = MR.modules[connectedModule].surfaces["top"].connectedSurface.module.id;
            leftModules.Add(connectedModule);
        }

        //оставляем только ту часть, которая будет сгибаться
        List<int> halfRightModules = rightModules.GetRange(rightModules.Count - rightPart, rightPart);
        List<int> halfLeftModules = leftModules.GetRange(rightModules.Count - leftPart, leftPart);


        float angleRight = a / rightPart;
        float angleLeft = a / leftPart;

        foreach (int id in halfRightModules)
        {
            print("Right: " + id);
            angles[id] = angleRight;
        }

        angles[halfRightModules[halfRightModules.Count - 1]] = 90 - 360f / (3.5f * 2);
        angles[halfRightModules[0]] = 90;

        foreach (int id in halfLeftModules)
        {
            print("Left: " + id);
            angles[id] = angleLeft;
        }

        angles[halfLeftModules[halfLeftModules.Count - 1]] = 0;
        angles[halfLeftModules[0]] = 90;

        return angles;
    }

    private IEnumerator SnakeToWalker3Angles()
    {
        //разделяем модули на 4 ноги
        firstLeg = rightModules.GetRange(0, rightModules.Count / 2);
        secondLeg = rightModules.GetRange(rightModules.Count / 2, rightModules.Count - firstLeg.Count);
        thirdLeg = leftModules.GetRange(0, leftModules.Count / 2);
        fourthLeg = leftModules.GetRange(leftModules.Count / 2, leftModules.Count - thirdLeg.Count);

        MR.modules[firstLeg[firstLeg.Count - 1]].surfaces["bottom"].Disconnect();
        MR.modules[thirdLeg[thirdLeg.Count - 1]].surfaces["top"].Disconnect();
        MR.modules[secondLeg[secondLeg.Count - 1]].surfaces["bottom"].Connect(MR.modules[centralModule].surfaces["right"]);
        MR.modules[fourthLeg[fourthLeg.Count - 1]].surfaces["top"].Connect(MR.modules[centralModule].surfaces["left"]);

        for (int i = 0; i < MR.modules.Count; i++)
        {
            MR.modules[i].drivers["q1"].Set(0);
        }
        yield return WaitWhileDriversAreBusy();

    }

    private IEnumerator SnakeToWalker4Angles()
    {
        secondLeg.Reverse();
        fourthLeg.Reverse();

        for (int i = 0; i < MR.modules.Count; i++)
        {
            MR.modules[i].drivers["q2"].Set(0);
        }
        yield return WaitWhileDriversAreBusy();

        //поворачиваем суставы возле центрального модуля и следующие за ним
        MR.modules[centralModule].drivers["q2"].Set(-90);
        MR.modules[firstLeg[0]].drivers["q2"].Set(-90);
        MR.modules[firstLeg[1]].drivers["q2"].Set(-90);
        MR.modules[secondLeg[0]].drivers["q2"].Set(-90);
        MR.modules[thirdLeg[0]].drivers["q2"].Set(-90);
        MR.modules[fourthLeg[1]].drivers["q2"].Set(-90);

        yield return WaitWhileDriversAreBusy();

        RenameModulesForWalker();
    }

    private IEnumerator SnakeToWalker5Angles()
    {
        int total = MR.modules.Count;
        int offset = 0;
        if (total > 12)
        {
            for (int i = 1 + offset; i < 5 + offset; i++)
            {
                int nextModule = i + 4;
                MR.modules[nextModule].drivers["q1"].Set(20);
                MR.modules[nextModule + 4].drivers["q1"].Set(70);
            }
        }
        yield return WaitWhileDriversAreBusy();
    }

    private void RenameModulesForSnake()
    {
        modulez = new Dictionary<int, Module>(MR.modules);
        MR.modules.Clear();

        int connectedModule = 0;
        while (modulez[connectedModule].surfaces["top"].connectedSurface != null)
        {
            connectedModule = modulez[connectedModule].surfaces["top"].connectedSurface.module.id;
        }

        int counter = modulez.Count - 1;
        while (counter >= 0)
        {
            modulez[connectedModule].id = counter;
            modulez[connectedModule].name = "Module " + counter;
            MR.modules[counter] = modulez[connectedModule];

            if (modulez[connectedModule].surfaces["bottom"].connectedSurface != null)
            {
                connectedModule = modulez[connectedModule].surfaces["bottom"].connectedSurface.module.id;
            }
            counter--;
        }
    }

    private void RenameModulesForWalker()
    {
        modulez = new Dictionary<int, Module>(MR.modules);
        MR.modules.Clear();
        foreach (KeyValuePair<int, Module> keyValue in modulez)
        {
            print("BEFORE: " + keyValue.Key + ", " + keyValue.Value);
        }
        Module m0 = modulez[centralModule];
        m0.id = 0;
        m0.name = "Module " + 0;
        MR.modules[0] = m0;
        int counter = 1;
        RenameModules(counter, firstLeg);
        counter = 2;
        RenameModules(counter, secondLeg);
        counter = 3;
        RenameModules(counter, thirdLeg);
        counter = 4;
        RenameModules(counter, fourthLeg);

        foreach (KeyValuePair<int, Module> keyValue in MR.modules)
        {
            print("AFTER: " + keyValue.Key + ", " + keyValue.Value);
        }
    }

    private void RenameModules(int counter, List<int> leg)
    {
        foreach (int id in leg)
        {
            if (counter < MR.angles.Length)
            {
                modulez[id].id = counter;
                modulez[id].name = "Module " + counter;
                MR.modules[counter] = modulez[id];
                counter += 4;
            }

        }
    }

    private int CheckEdgeModule(float[] angles, int module)
    {
        int secondModule = module + 1;
        if (secondModule >= angles.Length)
        {
            secondModule = 0;
        }

        if (secondModule < 0)
        {
            secondModule = angles.Length - 1;
        }

        return secondModule;
    }

    private int GetTopMidModule()
    {
        float maxValue = -100;
        int id = -1;
        foreach (Module module in MR.modules.Values)
        {
            if (module.position.y > maxValue)
            {
                maxValue = module.position.y;
                id = module.id;
            }
        }
        return id;
    }

    private int GetCentralModule()
    {
        int total = MR.modules.Count;
        centralModule = midModule - total / 2;
        if (centralModule < 0)
        {
            centralModule = total + centralModule;
        }
        else if (centralModule >= total)
        {
            centralModule = total - centralModule;
        }
        return centralModule - 1;
    }

    private IEnumerator WaitUntilMoveEnds(GaitControlTable controlTable)
    {
        controlTable.BeginTillEnd();
        while (controlTable.inProgress || !controlTable.isReady)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MR = GetComponent<ModularRobot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (snakeToWheel)
        {
            if (GetDistance(MR.modules[0].position, MR.modules[MR.modules.Count - 1].position) < 0.29)
            {
                MR.modules[0].surfaces["bottom"].Connect(MR.modules[MR.modules.Count - 1].surfaces["top"]);
                isClose = true;
                snakeToWheel = false;
            }
        }
    }


}
