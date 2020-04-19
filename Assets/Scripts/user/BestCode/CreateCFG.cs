using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCFG : MonoBehaviour
{
    private readonly float moduleLength = 0.2766909f;
    private readonly float EPSILON = 0.001f;

    private float n;

    public float[] CreateWalker(int total)
    {
        float[] array = new float[total];
        if (total > 12)
        {
            for (int i = 1; i < 5; i++)
            {
                int nextModule = i + 4;
                array[nextModule] = 45;
                array[nextModule + 4] = 45;
            }
        }
        
        return array;
    } 

     public float[] CreateSnake(int total)
    {
        float[] array = new float[total];
        for (int i = 0; i < total; i++)
        {
            array[i] = 0;
        }
        return array;
    }

    public float[] CreatePerfectWheel(int total)
    {
        float angle = 360f / total;
        print("Angle " + angle);
        float[] array = new float[total];
        for(int i = 0; i < total; i++)
        {
            array[i] = angle;
        }
        return array;
    }

    //Нормально работает на lastFlat 3-5 при total = 21'
    public float[] CreateRoundedWheel(int total)
    {
        float[] array = new float[total];
        int lastFlat = 5;

        n = lastFlat * 1f / (total - lastFlat);
        print("n: " + n);

        float a = Mathf.Rad2Deg * NewtonRaphson(4) * 2;
        float angle = a / (total - lastFlat);
        print("TOTAL ANGLE: " + a);
        print("CALCULATED ANGLE: " + angle);

        for(int i = 0; i < lastFlat; i++)
        {
            array[i] = 0f;
        }
        array[lastFlat - 1] = 90 - 360 / (lastFlat * 2);
        for (int i = lastFlat; i < total; i++)
        {
            array[i] = angle;
        }
        array[total - 1] = array[lastFlat - 1];

        return array;
    }

    private float Func1(float x)
    {
        return Mathf.Sin(x) - n * x;
    }

    private float DerivFunc(float x)
    {
        return Mathf.Cos(x) - n;
    }

    // Function to find the root
    private float NewtonRaphson(float x)
    {
        float h = Func1(x) / DerivFunc(x);

        while (Mathf.Abs(h) >= EPSILON)
        {
            h = Func1(x) / DerivFunc(x);
            x -= h;
        }
        return x; //radians
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
