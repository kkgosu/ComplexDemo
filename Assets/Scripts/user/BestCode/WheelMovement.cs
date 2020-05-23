using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMovement : Movement
{
    private ModularRobot MR;

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

        while (isMoving)
        {
            float last = MR.modules[MR.modules.Count - 1].drivers["q1"].qValue;
            for (int i = MR.modules.Count - 2; i >= 0; i--)
            {
                MR.modules[i+1].drivers["q1"].Set(MR.modules[i].drivers["q1"].qValue, 2);
            }
            MR.modules[0].drivers["q1"].Set(last, 2);
            yield return WaitWhileDriversAreBusy();
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

    protected IEnumerator WaitWhileDriversAreBusy()
    {
        while (IfAnyDriverIsBusy())
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private bool IfAnyDriverIsBusy()
    {
        bool flag = false;
        foreach (Module module in MR.modules.Values)
        {
            foreach (Driver driver in module.drivers.Values)
            {
                if (driver.busy)
                {
                    flag = true;
                }
            }
        }
        return flag;
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
