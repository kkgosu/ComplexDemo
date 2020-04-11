using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformations : MonoBehaviour
{
    public IEnumerator Execute(ModularRobot modularRobot, GaitControlTable controlTable, params Func<ModularRobot, GaitControlTable, IEnumerator>[] actions)
    {
        foreach (Func<ModularRobot, GaitControlTable, IEnumerator> action in actions)
        {
            yield return StartCoroutine(action(modularRobot, controlTable));
        }
    }

    public IEnumerator MoveWheelBack(ModularRobot modularRobot, GaitControlTable controlTable)
    {
        controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        controlTable.ReadFromFile(modularRobot, "WheelBack.txt");
        yield return StartCoroutine(MoveWheelBack(controlTable));
        print("TAGG: MoveWheelBack");
    }

    private IEnumerator MoveWheelBack(GaitControlTable controlTable)
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        yield return WaitUntilMoveEnds(controlTable);

    }
    public IEnumerator WheelToSnake(ModularRobot modularRobot, GaitControlTable controlTable)
    {
        int midModule = getTopMidModule(modularRobot);
        if (midModule != -1)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
            modularRobot.modules[midModule].surfaces["top"].Disconnect();
            controlTable.ReadFromFile(modularRobot, "SnakeToWalker_1.txt");
            yield return StartCoroutine(TransformWheelToSnake(controlTable));
        }
        print("TAGG: WheelToSnake");
        yield return null;
    }

    private IEnumerator TransformWheelToSnake(GaitControlTable controlTable)
    {
        yield return WaitUntilMoveEnds(controlTable);
    }

    public IEnumerator SnakeToWalker(ModularRobot modularRobot, GaitControlTable controlTable)
    {
        yield return StartCoroutine(TransformSnakeToWalker(modularRobot, controlTable));
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
