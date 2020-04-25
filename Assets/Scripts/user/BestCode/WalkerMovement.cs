using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerMovement : Movement
{
    private int offset = 0;

    private float[] CreateSecondtStep(float[] angles)
    {
        for (int i = 1; i <= 12; i++)
        {
            if (angles[i] == 15)
            {
                angles[i] -= 30;
            }
            else if (angles[i] == -15)
            {
                angles[i] += 30;
            }
            else if (angles[i] == 70)
            {
                angles[i] = 50;
            }
            else if (angles[i] == 50)
            {
                angles[i] = 70;
            }
            else if (angles[i] == 20)
            {
                angles[i] = 40;
            }
            else if (angles[i] == 40)
            {
                angles[i] = 20;
            }
        }
        return angles;
    }

    private float[] CreateFirstStep(float[] angles)
    {
        angles[1] = 15;
        angles[2] = -15;
        angles[3] = -15;
        angles[4] = 15;

        angles[5] = 20;
        angles[6] = 40;
        angles[7] = 20;
        angles[8] = 40;

        angles[9] = 70;
        angles[10] = 50;
        angles[11] = 70;
        angles[12] = 50;

        return angles;
    }

    override public IEnumerator MoveBackward(ModularRobot modularRobot, float[] angles)
    {
        isMoving = true;
        float[] newAngles = angles;
        GaitControlTable controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        while (isMoving)
        {
            newAngles = CreateFirstStep(angles);
            controlTable.ReadFromFile(modularRobot, CreateGCT(newAngles, 1));
            yield return StartCoroutine(Move(controlTable));
            newAngles = CreateSecondtStep(angles);
            controlTable.ReadFromFile(modularRobot, CreateGCT(newAngles, 1));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    override public IEnumerator MoveForward(ModularRobot modularRobot, float[] angles)
    {
        throw new System.NotImplementedException();
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
