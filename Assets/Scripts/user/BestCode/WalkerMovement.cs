using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerMovement : Movement
{
    private static float stepAngle = 21f;
    private static float alphaBig = 70f;
    private static float alphaSmall = 90 - alphaBig;
    private static float betaBig = 50f;
    private static float betaSmall = 90 - betaBig;

    override public IEnumerator MoveBackward(ModularRobot modularRobot, float[] angles)
    {
        return DefaultStep(modularRobot, angles, 0);
    }

    override public IEnumerator MoveForward(ModularRobot modularRobot, float[] angles)
    {
        return DefaultStep(modularRobot, angles, 2);
    }

    override public IEnumerator MoveLeft(ModularRobot modularRobot, float[] angles)
    {
        return DefaultStep(modularRobot, angles, 3);
    }

    override public IEnumerator MoveRight(ModularRobot modularRobot, float[] angles)
    {
        return DefaultStep(modularRobot, angles, 1);
    }

    override public IEnumerator RotateToTheLeft(ModularRobot modularRobot, float[] angles)
    {
        return DefaultRotation(modularRobot, angles, 30);
    }

    override public IEnumerator RotateToTheRight(ModularRobot modularRobot, float[] angles)
    {
        return DefaultRotation(modularRobot, angles, -30);
    }

    private IEnumerator DefaultStep(ModularRobot modularRobot, float[] angles, int offset)
    {
        isMoving = true;
        
        GaitControlTable controlTable = modularRobot.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        }
        float[] newAngles = CreateFirstStep(angles);
        ChangeDirection(newAngles, offset);
        controlTable.ReadFromFile(modularRobot, CreateGCT(newAngles, 1));
        yield return StartCoroutine(Move(controlTable));
        while (isMoving)
        {
            newAngles = CreateSecondtStep(angles);
            controlTable.ReadFromFile(modularRobot, CreateGCT(newAngles, 1));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    private float[] CreateSecondtStep(float[] angles)
    {
        for (int i = 1; i <= 12; i++)
        {
            if (angles[i] == stepAngle)
            {
                angles[i] = -stepAngle;
            }
            else if (angles[i] == -stepAngle)
            {
                angles[i] = stepAngle;
            }
            else if (angles[i] == alphaBig)
            {
                angles[i] = betaBig;
            }
            else if (angles[i] == betaBig)
            {
                angles[i] = alphaBig;
            }
            else if (angles[i] == alphaSmall)
            {
                angles[i] = betaSmall;
            }
            else if (angles[i] == betaSmall)
            {
                angles[i] = alphaSmall;
            }
        }
        return angles;
    }

    private float[] CreateFirstStep(float[] angles)
    {
        angles[1] = stepAngle;
        angles[2] = -stepAngle;
        angles[3] = -stepAngle;
        angles[4] = stepAngle;

        angles[5] = alphaSmall;
        angles[6] = betaSmall;
        angles[7] = alphaSmall;
        angles[8] = betaSmall;

        angles[9] = alphaBig;
        angles[10] = betaBig;
        angles[11] = alphaBig;
        angles[12] = betaBig;

        return angles;
    }

    /// <summary>
    /// Меняет направление движения робота
    /// </summary>
    /// <param name="angles">Массив углов суставов</param>
    /// <param name="offset">0 - назад, 1 - направо, 2 - прямо, 3 - налево</param>
    /// <returns></returns>
    private float[] ChangeDirection(float[] angles, int offset)
    {
        if (offset == 0)
        {
            return angles;
        }
        float last = angles[4];
        for (int i = 3; i >= 1; i--)
        {
            angles[i + 1] = angles[i];
        }
        angles[1] = last;
        return ChangeDirection(angles, offset - 1);
    }

    private IEnumerator DefaultRotation(ModularRobot modularRobot, float[] angles, int angleToRotate)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        }

        float[] newAngles = RotateRobotByAngle(angles, angleToRotate, 1);
        controlTable.ReadFromFile(modularRobot, CreateGCT(newAngles, 2));
        yield return StartCoroutine(Move(controlTable));

        newAngles = RotateRobotByAngle(newAngles, angleToRotate, 2);
        controlTable.ReadFromFile(modularRobot, CreateGCT(newAngles, 2));
        yield return StartCoroutine(Move(controlTable));

        newAngles = RotateRobotByAngle(newAngles, 30, -1);
        controlTable.ReadFromFile(modularRobot, CreateGCT(newAngles, 2));
        yield return StartCoroutine(Move(controlTable));

        isMoving = false;
    }

    /// <summary>
    /// Поворачиваем робота на определенный угол
    /// </summary>
    /// <param name="angles">Углы всех суставов</param>
    /// <param name="angle">Угол, на который поворачиваем. Для поворота вправо - положительное число, для поворота влево - отрицательное</param>
    /// <param name="pair">Номер пары противоположенных ног. 1 - ноги с модулями 1,5,9.., 3,7,11...; 2 - ноги с модулями 2,6,10.., 4,8,12...</param>
    /// <returns>Новые углы всех суставов</returns>
    private float[] RotateRobotByAngle(float[] angles, int angle, int pair)
    {
        if (pair == 1)
        {
            angles[1] += angle;
            angles[3] += angle;

            angles[5] = alphaSmall;
            angles[6] = betaSmall;
            angles[7] = alphaSmall;
            angles[8] = betaSmall;

            angles[9] = alphaBig;
            angles[10] = betaBig;
            angles[11] = alphaBig;
            angles[12] = betaBig;
        } else if (pair == 2)
        {
            angles[2] += angle;
            angles[4] += angle;

            angles[5] = betaSmall;
            angles[6] = alphaSmall;
            angles[7] = betaSmall;
            angles[8] = alphaSmall;

            angles[9] = betaBig;
            angles[10] = alphaBig;
            angles[11] = betaBig;
            angles[12] = alphaBig;
        } else
        {
            angles[1] = 0;
            angles[2] = 0;
            angles[3] = 0;
            angles[4] = 0;

            angles[5] = alphaSmall;
            angles[6] = betaSmall;
            angles[7] = alphaSmall;
            angles[8] = betaSmall;

            angles[9] = alphaBig;
            angles[10] = betaBig;
            angles[11] = alphaBig;
            angles[12] = betaBig;
        }

        return angles;
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
