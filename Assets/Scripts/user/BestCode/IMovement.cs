using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    IEnumerator MoveForward(ModularRobot modularRobot, float[] angles);
    IEnumerator MoveBackward(ModularRobot modularRobot, float[] angles);
    IEnumerator MoveRight(ModularRobot modularRobot, float[] angles);
    IEnumerator MoveLeft(ModularRobot modularRobot, float[] angles);
    IEnumerator RotateToTheRight(ModularRobot modularRobot, float[] angles);
    IEnumerator RotateToTheLeft(ModularRobot modularRobot, float[] angles);
}
