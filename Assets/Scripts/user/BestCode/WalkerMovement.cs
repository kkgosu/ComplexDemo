using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerMovement : Movement
{
    private float[] CreateSecondtStep(float[] angles)
    {
        for (int i = 1; i <= 12; i++)
        {
            if (angles[i] == 15)
            {
                angles[i] -= 30;
            } else if (angles[i] == -15)
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
        for (int i = 1; i <= 12; i++)
        {
            if (angles[i] == 0)
            {
                if (angles[i] % 2 == 1)
                {
                    angles[i] = -15;
                } else
                {
                    angles[i] = 15;
                }
            } else if (angles[i] == 70)
            {
                if (angles[i] % 2 == 1)
                {
                    angles[i] = 50;
                }
            } else if (angles[i] == 20)
            {
                if (angles[i] % 2 == 1)
                {
                    angles[i] = 40;
                }
            }
        }
        return angles;
    }
    override public IEnumerator MoveBackward(ModularRobot modularRobot, float[] angles)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        controlTable.ReadFromFile(modularRobot, CreateGCT(CreateFirstStep(angles), 1));
        yield return StartCoroutine(Move(controlTable));
        while (isMoving)
        {
            controlTable.ReadFromFile(modularRobot, CreateGCT(CreateSecondtStep(angles), 1));
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
