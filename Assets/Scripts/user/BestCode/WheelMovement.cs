using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMovement : Movement
{
    override public IEnumerator MoveBackward(ModularRobot modularRobot)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        }
        while (isMoving)
        {
            controlTable.ReadFromFile(modularRobot, CreateGCT(PreviousStep(modularRobot.angles), 2));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    override public IEnumerator MoveForward(ModularRobot modularRobot)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        }
        while (isMoving)
        {
            controlTable.ReadFromFile(modularRobot, CreateGCT(NextStep(modularRobot.angles), 2));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    override public IEnumerator MoveLeft(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    override public IEnumerator MoveRight(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    override public IEnumerator RotateToTheLeft(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    override public IEnumerator RotateToTheRight(ModularRobot modularRobot)
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
