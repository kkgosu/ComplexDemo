using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovementNew : Movement
{
    public override IEnumerator MoveBackward(ModularRobot modularRobot, float[] angles)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        while (isMoving)
        {
            controlTable.ReadFromFile(modularRobot, CreateGCTForSnake(angles, 3));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    public override IEnumerator MoveForward(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator MoveLeft(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator MoveRight(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator RotateToTheLeft(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator RotateToTheRight(ModularRobot modularRobot, float[] angles)
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
