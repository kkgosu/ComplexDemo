using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformations : MonoBehaviour
{
    public bool isExecuting = false;

    public void ExecuteSteps(params Action[] steps)
    {
        foreach (Action step in steps)
        {
            step();
            while (isExecuting)
            {

            }
        }
    }
    public void MoveWheelBack(ModularRobot modularRobot, GaitControlTable controlTable)
    {
        controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        controlTable.ReadFromFile(modularRobot, "WheelBack.txt");
        StartCoroutine(MoveWheelBack(controlTable));
        print("TAGG: MoveWheelBack");

    }

    private IEnumerator MoveWheelBack(GaitControlTable controlTable)
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        yield return WaitUntilMoveEnds(controlTable);

    }
    public void WheelToSnake(ModularRobot modularRobot, GaitControlTable controlTable)
    {
        int midModule = getTopMidModule(modularRobot);
        if (midModule != -1)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
            modularRobot.modules[midModule].surfaces["top"].Disconnect();
            controlTable.ReadFromFile(modularRobot, "SnakeToWalker_1.txt");
            StartCoroutine(TransformWheelToSnake(controlTable));
        }
        print("TAGG: WheelToSnake");
    }

    private IEnumerator TransformWheelToSnake(GaitControlTable controlTable)
    {
        yield return WaitUntilMoveEnds(controlTable);
    }

    public void SnakeToWalker(ModularRobot modularRobot, GaitControlTable controlTable)
    {
        StartCoroutine(TransformSnakeToWalker(modularRobot, controlTable));
        print("TAGG: SnakeToWalker");
    }

    private IEnumerator TransformSnakeToWalker(ModularRobot modularRobot, GaitControlTable controlTable)
    {
        controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        controlTable.ReadFromFile(modularRobot, "SnakeToWalker_2.txt");
        yield return WaitUntilMoveEnds(controlTable);

        modularRobot.modules[417].surfaces["top"].Disconnect();
        modularRobot.modules[47].surfaces["top"].Disconnect();
        modularRobot.modules[412].surfaces["top"].Connect(modularRobot.modules[42].surfaces["right"]);
        modularRobot.modules[413].surfaces["bottom"].Connect(modularRobot.modules[42].surfaces["left"]);

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
        isExecuting = true;
        while (controlTable.inProgress || !controlTable.isReady)
        {
            yield return new WaitForEndOfFrame();
        }
        isExecuting = false;
    }

    private int getTopMidModule(ModularRobot modularRobot)
    {
        float maxValue = -1;
        int id = -1;
        foreach (Module module in modularRobot.modules.Values)
        {
            if (module.y > maxValue)
            {
                maxValue = module.y;
                id = module.id;
            }
        }
        return id;
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
