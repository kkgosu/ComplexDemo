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

    List<int> firstLeg;
    List<int> secondLeg;
    List<int> thirdLeg;
    List<int> fourthLeg;

    Dictionary<int, Module> modulez;

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
            if (controlTable == null)
            {
                controlTable = MR.gameObject.AddComponent<GaitControlTable>();
            }

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
        centralModule = GetCentralModule(MR.angles.Length);
        print("Central Module: " + centralModule);
        GaitControlTable controlTable = MR.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = MR.gameObject.AddComponent<GaitControlTable>();
        }
        Dictionary<int, float> modulesQ2 = SnakeToWalker1Angles(MR.angles.Length);
        controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, 2, modulesQ2));
        yield return WaitUntilMoveEnds(controlTable);

         MR.angles = NewSnakeToWalker2Angles(MR.angles);
         controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, 2, 90, 1));
         yield return WaitUntilMoveEnds(controlTable);

          MR.angles = SnakeToWalker3Angles(MR.angles);
          controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, 2));
          yield return WaitUntilMoveEnds(controlTable);

          Dictionary<int, float> modulesQ24 = SnakeToWalker4Angles(MR.angles.Length);
          controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, 2, modulesQ24));
          yield return WaitUntilMoveEnds(controlTable);

          RenameModulesForWalker();
          CreateCFG createCFG = GetComponent<CreateCFG>();
          controlTable.ReadFromFile(MR, Movement.CreateGCT(createCFG.CreateWalker(MR.angles.Length), 2));
          yield return WaitUntilMoveEnds(controlTable);
    }

    public IEnumerator WalkerToSnake()
    {
        GaitControlTable controlTable = MR.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = MR.gameObject.AddComponent<GaitControlTable>();
        }

        RenameModulesForWalker();
        CreateCFG createCFG = GetComponent<CreateCFG>();
        controlTable.ReadFromFile(MR, Movement.CreateGCT(createCFG.CreateWalker(MR.angles.Length), 2));
        yield return WaitUntilMoveEnds(controlTable);
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

    private Dictionary<int, float> SnakeToWalker1Angles(int total)
    {
        int offset = total / 3;
        leftMidModule = midModule - offset;
        rightMidModule = midModule + offset;

        if (total % 2 == 1)
        {
            leftMidModule--;
        }

        if (leftMidModule < 0)
        {
            leftMidModule = total + leftMidModule;
        }
        if (rightMidModule >= total)
        {
            rightMidModule = total - rightMidModule;
        }

        Dictionary<int, float> keyValuePairs = new Dictionary<int, float>
        {
            { leftMidModule, 90 },
            { rightMidModule, 90 }
        };

        return keyValuePairs;
    }
    private float[] NewSnakeToWalker2Angles(float[] angles)
    {
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

        angles[leftModules[leftModules.Count - 1]] = 90;
        angles[halfLeftModules[halfLeftModules.Count - 1]] = 90;
        angles[halfLeftModules[0]] = 90;

        angles[rightModules[leftModules.Count - 1]] = 90;
        angles[halfRightModules[halfRightModules.Count - 1]] = 90;
        angles[halfRightModules[0]] = 90;

        return angles;
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
        } else
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

    private float[] SnakeToWalker3Angles(float[] angles)
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

        for (int i = 0; i < angles.Length; i++)
        {
            angles[i] = 0f;
        }
        return angles;
    }

    private Dictionary<int, float> SnakeToWalker4Angles(int total)
    {
        secondLeg.Reverse();
        fourthLeg.Reverse();

        //поворачиваем суставы возле центрального модуля и следующие за ним
        Dictionary<int, float> keyValuePairs = new Dictionary<int, float>
        {
            { 0, 90 },
            { firstLeg[0], 90 },
            { firstLeg[1], 90 },
            { secondLeg[0], -90 },
            { thirdLeg[0], 90 },
            { fourthLeg[1], -90 },
        };
        
        for (int i = 0; i < total; i++)
        {
            if (!keyValuePairs.ContainsKey(i))
            {
                keyValuePairs.Add(i, 0f);
            }
        }

        return keyValuePairs;
    }

    private void RenameModulesForWalker()
    {
        modulez = new Dictionary<int, Module>(MR.modules);
        MR.modules.Clear();
        foreach (KeyValuePair<int, Module> keyValue in modulez)
        {
            print("BEFORE: " + keyValue.Key + ", " + keyValue.Value);
        }
        MR.modules[0] = modulez[centralModule];
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
            if (counter < MR.angles.Length )
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

    private int GetCentralModule(int total)
    {
        centralModule = midModule - total / 2;
        if (centralModule < 0)
        {
            centralModule = total + centralModule;
        } else if (centralModule >= total)
        {
            centralModule = total - centralModule;
        }
        return centralModule;
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
       
    }


}
