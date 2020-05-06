using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    IEnumerator MoveForward(ModularRobot modularRobot);
    IEnumerator MoveBackward(ModularRobot modularRobot);
    IEnumerator MoveRight(ModularRobot modularRobot);
    IEnumerator MoveLeft(ModularRobot modularRobot);
    IEnumerator RotateToTheRight(ModularRobot modularRobot);
    IEnumerator RotateToTheLeft(ModularRobot modularRobot);
}
