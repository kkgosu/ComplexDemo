using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformations : MonoBehaviour
{
    private ModularRobot MR;
    private int midModule;
    public IEnumerator Execute(params Func<IEnumerator>[] actions)
    {
        foreach (Func<IEnumerator> action in actions)
        {
            yield return StartCoroutine(action());
        }
    }

    public IEnumerator WheelToSnake()
    {
        midModule = getTopMidModule();
        if (midModule != -2)
        {
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
        GaitControlTable controlTable = MR.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = MR.gameObject.AddComponent<GaitControlTable>();
        }
        Dictionary<int, float> modulesQ2 = SnakeToWalker2Angles(MR.angles.Length);
        controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, 2, modulesQ2));
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
        int leftModule = midModule - offset;
        int rightModule = midModule + offset;

        if (leftModule < 0)
        {
            leftModule = total + leftModule;
        }
        if (rightModule >= total)
        {
            rightModule = total - rightModule;
        }

        Dictionary<int, float> keyValuePairs = new Dictionary<int, float>
        {
            { leftModule, 90 },
            { rightModule, 90 }
        };

        return keyValuePairs;
    }

    private float[] SnakeToWalker2(float[] angles)
    {
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

    private int getTopMidModule()
    {
        float maxValue = -100;
        int id = -1;
        foreach (Module module in MR.modules.Values)
        {
            print("ID: " + module.id);
            print("Y Value: " + module.position.y);
            if (module.position.y > maxValue)
            {
                maxValue = module.position.y;
                id = module.id;
            }
        }
        return id - 1;
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
