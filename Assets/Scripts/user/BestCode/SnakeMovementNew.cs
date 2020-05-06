﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovementNew : Movement
{
    public override IEnumerator MoveBackward(ModularRobot modularRobot)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        }
        while (isMoving)
        {
            controlTable.ReadFromFile(modularRobot, CreateGCTForSnake(modularRobot.angles, 3));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    public override IEnumerator MoveForward(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator MoveLeft(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator MoveRight(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator RotateToTheLeft(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator RotateToTheRight(ModularRobot modularRobot)
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
