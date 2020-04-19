using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Общий класс для классов, отвечающих за передвижение робота
/// </summary>
public abstract class Movement : MonoBehaviour, IMovement
{
    public bool isMoving = false;
   
    protected float[] NextStep(float[] angles)
    {
        float last = angles[angles.Length - 1];
        for (int i = angles.Length - 2; i >= 0; i--)
        {
            angles[i + 1] = angles[i];
        }
        angles[0] = last;
        return angles;
    }

    protected float[] PreviousStep(float[] angles)
    {
        float first = angles[0]; 
        for (int i = 1; i < angles.Length; i++)
        {
            angles[i - 1] = angles[i];
        }
        angles[angles.Length - 1] = first;
        
        return angles;
    }

    protected string CreateGCT(float[] angles, int time)
    {
        StringBuilder builder = new StringBuilder("header = \"");
        for (int i = 0; i < angles.Length; i++)
        {
            builder.Append(i);
            if (i != angles.Length - 1)
            {
                builder.Append(",");
            }
            else
            {
                builder.Append("\"\n");
            }
        }
        string header = builder.ToString();
        builder.Clear();

        for (int i = 0; i < angles.Length; i++)
        {
            string angle = angles[i].ToString().Replace(",", ".");
            builder.Append(angle).Append("(" + time + ")");
            if (i != angles.Length - 1)
            {
                builder.Append(",");
            }
            else
            {
                builder.Append("\n");
            }
        }

        string values = builder.ToString();
        string path = Application.dataPath + "/Resources/Gait Control Tables/" + "teztz" + ".gct";
        File.WriteAllText(path, header + values);
        return path;
    }
    protected IEnumerator Move(GaitControlTable controlTable)
    {
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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public abstract IEnumerator MoveForward(ModularRobot modularRobot, float[] angles);
    public abstract IEnumerator MoveBackward(ModularRobot modularRobot, float[] angles);
    public abstract IEnumerator MoveRight(ModularRobot modularRobot, float[] angles);
    public abstract IEnumerator MoveLeft(ModularRobot modularRobot, float[] angles);
    public abstract IEnumerator RotateToTheRight(ModularRobot modularRobot, float[] angles);
    public abstract IEnumerator RotateToTheLeft(ModularRobot modularRobot, float[] angles);
}
