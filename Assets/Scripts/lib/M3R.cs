using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M3R : Module {
    int count = 0;
    private void Update()
    {
        if (count >= 1000)
        {
           // print("ID: " + id + ", local center is " + transform.GetLocalCenter().ToString("0.0000") + transform);
            count = 0;
        }
        count++;
    }
}
