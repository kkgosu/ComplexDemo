using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SnakeSideways : MonoBehaviour
{
    public List<Driver> modules = new List<Driver>();
    int step = 0;
    public bool busy = false;
    int middle;
    public int FirstModuleOnGround;
    public int ConfigurationOfRobot;
    public int SideOnGroundHorizontal;
    public int SideOnGroundVertical;
    int crutch;
    public int level = 40;
    float a = 20;

    void AddModulesFromRobot()
    {
        foreach (Module m in GetComponent<ModularRobot>().modules.Values)
        {
            modules.Add(m.drivers["q1"]);
        }
    }

    bool DriversAreReady()
    {
        foreach (Driver drv in modules)
        {
            if (drv.busy)
                return false;
        }
        return true;
    }
    public void Move(double distance)
    {
        a = (float)(Math.Asin((distance / 2) / (ConfigurationOfRobot+1)) * 180 / Math.PI);
        busy = true;
    }

    void Go()
    {
        if (DriversAreReady())
        {
            a *= SideOnGroundHorizontal;
            switch (step)
            {
                case 1:
                    modules[middle - 1 - crutch * ConfigurationOfRobot].Set(level * SideOnGroundVertical);
                    //modules[middle + 1 + crutch * ConfigurationOfRobot].Set(level * SideOnGroundVertical);
                    //modules[middle].Set(10 * SideOnGroundVertical);
                    break;
                case 2:
                    modules[middle - 2 - crutch * ConfigurationOfRobot].Set(a);
                    //modules[middle + 2 + crutch * ConfigurationOfRobot].Set(-a * SideOnGroundVertical);
                    modules[middle - 4 - crutch * ConfigurationOfRobot].Set(-a / 2);
                    //modules[middle + 4 + crutch * ConfigurationOfRobot].Set(-a / 2 * SideOnGroundVertical);
                    //modules[middle].Set(10 * SideOnGroundVertical);
                    break;
                case 3:
                    modules[middle - 1 - crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle + 1 + crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle].Set(0);
                    break;
                case 4:
                    //modules[middle - 1 - crutch * ConfigurationOfRobot].Set(level * SideOnGroundVertical);
                    modules[middle + 1 + crutch * ConfigurationOfRobot].Set(level * SideOnGroundVertical);
                    //modules[middle].Set(10 * SideOnGroundVertical);
                    break;
                case 5:
                    //modules[middle - 2 - crutch * ConfigurationOfRobot].Set(a * SideOnGroundVertical);
                    modules[middle + 2 + crutch * ConfigurationOfRobot].Set(a);
                    //modules[middle - 4 - crutch * ConfigurationOfRobot].Set(a / 2 * SideOnGroundVertical);
                    modules[middle + 4 + crutch * ConfigurationOfRobot].Set(-a / 2);
                    //modules[middle].Set(10 * SideOnGroundVertical);
                    break;
                case 6:
                    //modules[middle - 1 - crutch * ConfigurationOfRobot].Set(0);
                    modules[middle + 1 + crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle].Set(0);
                    break;
                case 7:
                    float b = a / 2;
                    float x = 1 + 2 * Mathf.Cos((float)(b / 57.29577951)) + Mathf.Cos((float)((b - a) / 57.29577951));
                    float a1 = (float)(Mathf.Acos(x / (4)) * 57.29577951);

                    /* modules[middle - 4 - crutch * ConfigurationOfRobot].speed *= 0.5f;
                     modules[middle + 4 + crutch * ConfigurationOfRobot].speed *= 0.5f;
                     modules[middle - 1 - crutch * ConfigurationOfRobot].speed *= a1 / Math.Abs(a);
                     modules[middle + 1 + crutch * ConfigurationOfRobot].speed *= a1 / Math.Abs(a);
                     modules[middle - 5 - crutch * ConfigurationOfRobot].speed *= a1 / Math.Abs(a);
                     modules[middle + 5 + crutch * ConfigurationOfRobot].speed *= a1 / Math.Abs(a);*/

                    modules[middle - 2 - crutch * ConfigurationOfRobot].Set(0);
                    modules[middle + 2 + crutch * ConfigurationOfRobot].Set(0);

                    modules[middle - 4 - crutch * ConfigurationOfRobot].Set(0);
                    modules[middle + 4 + crutch * ConfigurationOfRobot].Set(0);

                    modules[middle - 1 - crutch * ConfigurationOfRobot].Set((-a1 - 10) * SideOnGroundVertical);
                    modules[middle + 1 + crutch * ConfigurationOfRobot].Set((-a1 - 10) * SideOnGroundVertical);

                    modules[middle - 5 - crutch * ConfigurationOfRobot].Set((a1 + 10) * SideOnGroundVertical);
                    modules[middle + 5 + crutch * ConfigurationOfRobot].Set((a1 + 10) * SideOnGroundVertical);
                    modules[middle].Set(-15 * SideOnGroundVertical);
                    //modules[2].Set(5);
                    //modules[10].Set(5);
                    break;
                case 8:
                    modules[middle - 2 - crutch * ConfigurationOfRobot].Set(-a);
                    modules[middle + 2 + crutch * ConfigurationOfRobot].Set(-a);

                    modules[middle - 4 - crutch * ConfigurationOfRobot].Set(a / 2);

                    modules[middle + 4 + crutch * ConfigurationOfRobot].Set(a / 2);

                    modules[middle - 1 - crutch * ConfigurationOfRobot].Set(0);
                    modules[middle + 1 + crutch * ConfigurationOfRobot].Set(0);

                    modules[middle - 5 - crutch * ConfigurationOfRobot].Set(0);
                    modules[middle + 5 + crutch * ConfigurationOfRobot].Set(0);
                    modules[middle].Set(0 * SideOnGroundVertical);
                    break;
                case 9:
                    /* modules[middle - 4 - crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle + 4 - crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle - 1 - crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle + 1 + crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle - 5 - crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle + 5 + crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;*/

                    modules[middle - 1 - crutch * ConfigurationOfRobot].Set(level * SideOnGroundVertical);
                    //modules[middle + 1 + crutch * ConfigurationOfRobot].Set(level * SideOnGroundVertical);
                    //modules[middle].Set(10 * SideOnGroundVertical);
                    break;
                case 10:
                    modules[middle - 2 - crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle + 2 + crutch * ConfigurationOfRobot].Set(0);

                    modules[middle - 4 - crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle + 4 + crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle].Set(10 * SideOnGroundVertical);
                    break;
                case 11:
                    modules[middle - 1 - crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle + 1 + crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle].Set(0);
                    break;
                case 12:
                    /* modules[middle - 4 - crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle + 4 - crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle - 1 - crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle + 1 + crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle - 5 - crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;
                     modules[middle + 5 + crutch * ConfigurationOfRobot].speed = modules[middle - 2 - crutch * ConfigurationOfRobot].speed;*/

                    //modules[middle - 1 - crutch * ConfigurationOfRobot].Set(level * SideOnGroundVertical);
                    modules[middle + 1 + crutch * ConfigurationOfRobot].Set(level * SideOnGroundVertical);
                    //modules[middle].Set(10 * SideOnGroundVertical);
                    break;
                case 13:
                    //modules[middle - 2 - crutch * ConfigurationOfRobot].Set(0);
                    modules[middle + 2 + crutch * ConfigurationOfRobot].Set(0);

                    //modules[middle - 4 - crutch * ConfigurationOfRobot].Set(0);
                    modules[middle + 4 + crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle].Set(10 * SideOnGroundVertical);
                    break;
                case 14:
                    //modules[middle - 1 - crutch * ConfigurationOfRobot].Set(0);
                    modules[middle + 1 + crutch * ConfigurationOfRobot].Set(0);
                    //modules[middle].Set(0);
                    break;
            }
            step++;
            if (step > 14)
            {
                step = 0;
                busy = false;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        AddModulesFromRobot();
        FirstModuleOnGround = 2;
        ConfigurationOfRobot = 1;
        SideOnGroundHorizontal = 1;
        SideOnGroundVertical = 1;
        crutch = 0;
        middle = (int)(modules.Count / 2);
        if (modules.Count % 2 != 0)
        {
            //middle++;
        }
        if ((middle - FirstModuleOnGround) % (ConfigurationOfRobot + 1) != 0)
        {
            crutch = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(busy)
            Go();
    }
}
