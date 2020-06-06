using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRobAdmin : MonoBehaviour
{
    //WaveController_5 Wc; // Общий родительский класс для WC4 и WC5, который будешь использовать в виде типа данных здесь,
    // GetComponent<typeof(<Имя_Родительского_Класса>)>().
    private SnakeGoToPoint ToPoint;
    SnakeMovement ToWheel;
                         

    // Start is called before the first frame update
    void Start()
    {
        ToPoint = gameObject.AddComponent<SnakeGoToPoint>();
        ToWheel = gameObject.AddComponent<SnakeMovement>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
