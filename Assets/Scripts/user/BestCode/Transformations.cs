using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformations : MonoBehaviour
{
    public IEnumerator Execute(ModularRobot modularRobot, GaitControlTable controlTable, params Func<ModularRobot, IEnumerator>[] actions)
    {
        foreach (Func<ModularRobot, IEnumerator> action in actions)
        {
            yield return StartCoroutine(action(modularRobot));
        }
    }

    public IEnumerator WheelToSnake(ModularRobot modularRobot)
    {
        int midModule = getTopMidModule(modularRobot);
        if (midModule != -1)
        {
            GaitControlTable controlTable = modularRobot.gameObject.GetComponent<GaitControlTable>();
            if (controlTable == null)
            {
                controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
            }
            
            Movement movement = modularRobot.gameObject.GetComponent<Movement>();
            if (movement == null)
            {
                movement = modularRobot.gameObject.AddComponent<Movement>();
            }
            /*            modularRobot.modules[midModule].surfaces["top"].Disconnect();
                        controlTable.ReadFromFile(modularRobot, "SnakeToWalker_1.txt");*/
            modularRobot.angles = SnakeToWalker1(modularRobot.angles, midModule);
            controlTable.ReadFromFile(modularRobot, movement.CreateGCT(modularRobot.angles, 2));

            yield return StartCoroutine(TransformWheelToSnake(controlTable));
        }
        print("TAGG: WheelToSnake");
        yield return null;
    }

    private float[] SnakeToWalker1(float[] angles, int midModule)
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

        int leftSecondModule = leftModule + 1;
        if (leftSecondModule >= angles.Length)
        {
            leftSecondModule = 0;
        }

        int rightSecondModule = leftModule - 1;
        if (rightSecondModule < 0)
        {
            rightSecondModule = angles.Length - 1;
        }

        angles[leftModule] = 90;
        angles[leftSecondModule] = 90;
        angles[rightModule] = 90;
        angles[rightSecondModule] = 90;

        return angles;
    }

    private IEnumerator TransformWheelToSnake(GaitControlTable controlTable)
    {
        yield return WaitUntilMoveEnds(controlTable);
    }

    public IEnumerator SnakeToWalker(ModularRobot modularRobot)
    {
        yield return StartCoroutine(TransformSnakeToWalker(modularRobot));
        print("TAGG: SnakeToWalker");
    }

    private IEnumerator TransformSnakeToWalker(ModularRobot modularRobot)
    {
        GaitControlTable controlTable = modularRobot.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        }
        controlTable.ReadFromFile(modularRobot, "SnakeToWalker_2.txt");
        yield return WaitUntilMoveEnds(controlTable);

        modularRobot.modules[16].surfaces["top"].Disconnect();
        modularRobot.modules[6].surfaces["top"].Disconnect();
        modularRobot.modules[11].surfaces["top"].Connect(modularRobot.modules[1].surfaces["right"]);
        modularRobot.modules[12].surfaces["bottom"].Connect(modularRobot.modules[1].surfaces["left"]);

        controlTable.ReadFromFile(modularRobot, "SnakeToWalker_3.txt");
        yield return WaitUntilMoveEnds(controlTable);

        controlTable.ReadFromFile(modularRobot, "SnakeToWalker_4.txt");
        yield return WaitUntilMoveEnds(controlTable);

        controlTable.ReadFromFile(modularRobot, "SnakeToWalker_5.txt");
        yield return WaitUntilMoveEnds(controlTable);
    }

    private IEnumerator WaitUntilMoveEnds(GaitControlTable controlTable)
    {
        controlTable.BeginTillEnd();
        while (controlTable.inProgress || !controlTable.isReady)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    //fix ids in xml and txt
    private int getTopMidModule(ModularRobot modularRobot)
    {
        float maxValue = 100;
        int id = -1;
        foreach (Module module in modularRobot.modules.Values)
        {
            print("ID: " + module.id);
            print("Y Value: " + module.y);
            if (module.y < maxValue)
            {
                maxValue = module.y;
                id = module.id;
            }
        }
        return id - 1;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}
