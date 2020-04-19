using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMovement : Movement
{
    override public IEnumerator MoveBackward(ModularRobot modularRobot, float[] angles)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        while (isMoving)
        {
            controlTable.ReadFromFile(modularRobot, CreateGCT(PreviousStep(angles), 2));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    override public IEnumerator MoveForward(ModularRobot modularRobot, float[] angles)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        while (isMoving)
        {
            controlTable.ReadFromFile(modularRobot, CreateGCT(NextStep(angles), 2));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    override public IEnumerator MoveLeft(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
    }

    override public IEnumerator MoveRight(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
    }

    override public IEnumerator RotateToTheLeft(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
    }

    override public IEnumerator RotateToTheRight(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
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
