using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformations : MonoBehaviour
{
    private ModularRobot MR;
    private int midModule;
    private int centralModule;
    private int leftEndModule;
    private int rightEndModule;
    private int leftMidModule;
    private int rightMidModule;
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
        if (midModule != -2)
        {
            leftEndModule = midModule;
            if (midModule + 1 >= MR.angles.Length)
            {
                rightEndModule = 0;
            } else
            {
                rightEndModule = midModule + 1;
            }
            if (midModule == -1)
            {
                midModule = MR.angles.Length - 1;
            }
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
        Dictionary<int, float> modulesQ2 = SnakeToWalker2Angles(MR.angles.Length);
        controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, 2, modulesQ2));
        yield return WaitUntilMoveEnds(controlTable);

        SnakeToWalker2(MR.angles);
        controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, 2));
        yield return WaitUntilMoveEnds(controlTable);

        /*        MR.modules[16].surfaces["top"].Disconnect();
                MR.modules[6].surfaces["top"].Disconnect();
                MR.modules[11].surfaces["top"].Connect(MR.modules[1].surfaces["right"]);
                MR.modules[12].surfaces["bottom"].Connect(MR.modules[1].surfaces["left"]);

                controlTable.ReadFromFile(MR, "SnakeToWalker_3.txt");
                yield return WaitUntilMoveEnds(controlTable);

                controlTable.ReadFromFile(MR, "SnakeToWalker_4.txt");
                yield return WaitUntilMoveEnds(controlTable);

                controlTable.ReadFromFile(MR, "SnakeToWalker_5.txt");
                yield return WaitUntilMoveEnds(controlTable);*/
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

    private Dictionary<int, float> SnakeToWalker2Angles(int total)
    {
        int offset = total / 3;
        leftMidModule = midModule - offset;
        rightMidModule = midModule + offset;

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

    private float[] SnakeToWalker2(float[] angles)
    {
        CreateCFG createCFG = GetComponent<CreateCFG>();
        float a = Mathf.Rad2Deg * createCFG.NewtonRaphson(4) * 2;

        int[] rightPart = new int[angles.Length / 3];
        int[] leftPart = new int[angles.Length / 3];

        float angleRight = a / rightPart.Length;
        float angleLeft = a / leftPart.Length;

        for (int i = 0; i < rightPart.Length; i++)
        {
            if (midModule + 1 + i < angles.Length)
            {
                rightPart[i] = midModule + 1 + i;
            } else 
            {
                rightPart[i] = midModule + 1 + i - angles.Length;
            }
        }

        for (int i = 0; i < leftPart.Length; i++)
        {
            if (midModule - i >= 0)
            {
                leftPart[i] = midModule - i;
            }
            else
            {
                leftPart[i] = midModule - i + angles.Length;
            }
        }

        foreach (int id in rightPart)
        {
            print("Right: " + id);
            angles[id] = angleRight;
        }

        
        angles[rightPart[rightPart.Length - 1]] = 90;
        angles[rightPart[0]] = 90 - 360 / (4 * 2);

        foreach (int id in leftPart)
        {
            print("Left: " + id);
            angles[id] = angleLeft;
        }
        
        angles[leftPart[leftPart.Length - 1]] = 90;
        angles[leftPart[0]] = 90 - 360 / (4 * 2);


        /*        array[lastFlat - 1] = 90 - 360 / (lastFlat * 2);
                for (int i = lastFlat; i < total; i++)
                {
                    array[i] = angle;
                }
                array[total - 1] = array[lastFlat - 1];*/

        return angles;
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
/*            print("ID: " + module.id);
            print("Y Value: " + module.position.y);*/
            if (module.position.y > maxValue)
            {
                maxValue = module.position.y;
                id = module.id;
            }
        }
        return id - 1;
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
