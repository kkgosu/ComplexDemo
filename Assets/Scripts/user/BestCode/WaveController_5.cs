using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveController_5 : MonoBehaviour
{

    //	bool isNegative = false;
    //	int num = isNegative ? -1 : 1;
    public int waveDist = 0;
    public bool busy = false;
    private struct io
    {
        public float bottom;
        public float wave;
        public float vertex;
        public List<float[]> intermAngles;
    }
    private io angles = new io();               //углы текущей конфигурации волны
    int NumberOfModulesInwave;                  //величина гребня волны в модулях
    WaveGenerator WaveGen;

    public int SideOnGroundVertical = 1;        //переменная нужна что бы определить знак угла поворота привода. 1 означает, что для поворота от земли подаётся положительный угол.
    public int FirstModuleOnGround;             //переменная показывает какой модуль первым лежит на земле нужной стороной
    public int ConfigurationOfRobot = 2;        //показывает через сколько модулей идёт чередование. 1 означает что все модули на земле нужной стороной, 2 что через 1 и т.д.
    public int DirectionOfMotion = 1;           //направление движения
    public float MaxAngle = 90;
    public float MinAngle = -90;


    public double[] MinMaxH(double dl)
    {
        NumberOfModulesInwave = 4;
        angles.intermAngles.Clear();
        double bottom = 4 - dl;
        double maxH = 2 * Math.Sin(Math.Acos(bottom / (2 * 2)));
        double minH = 0;
        if (bottom > 2)
            minH = Math.Sin(Math.Acos((bottom - 2) / (2)));
        else
            minH = 1 + Math.Sin(Math.Acos((bottom) / (2)));
        /*if (bottom <= 2 * Math.Sqrt(2))
        {
            minH = Math.Sin(Math.Cos(bottom / (2 * Math.Sqrt(2))));
        }
        else
        {
            minH = 0;
        }*/
        double[] ret = new double[2] { minH, maxH };
        return ret;
    }

    public bool Check(double dl, double h)
    {
        NumberOfModulesInwave = 4;
        angles.intermAngles.Clear();
        double bottom = 4 - dl;
        double bottomAngle = Math.Atan(2 * h / bottom);

        double side = h / Math.Sin(bottomAngle);
        double waveAngle = 2 * Math.Acos(side / 2);
        if (Math.Abs(waveAngle) > (Math.PI / 2))
        {
            Debug.LogError("Angle value is higher than possible maximum for this algorithm.");
            angles.intermAngles.Clear();
            return false;
        }

        double a = bottomAngle + waveAngle / 2;
        if (Math.Abs(a) > (Math.PI / 2))
        {
            Debug.LogError("Angle value is higher than possible maximum for this algorithm.");
            angles.intermAngles.Clear();
            return false;
        }

        double vertexAngle = 2 * bottomAngle - waveAngle;
        if (Math.Abs(vertexAngle) > (Math.PI / 2))
        {
            Debug.LogError("Angle value is higher than possible maximum for this algorithm.");
            angles.intermAngles.Clear();
            return false;
        }
        double da = (a / (int)(a * 600 / Math.PI));
        float[] Figure = new float[6];
        for (double i = a - da; i > 0; i -= da)
        {
            Figure[0] = (float)i;
            Figure[5] = (float)(a - i);
            double c = bottom + 1;
            double m = c - Math.Cos(Figure[0]) - Math.Cos(Figure[5]);
            double p = Math.Sin(Figure[0]) - Math.Sin(Figure[5]);
            double s = m / (Math.Cos(Math.Atan(p / m)));
            double g = Math.Acos((s - 1) / 2);
            if (Math.Abs(g) > (Math.PI / 2))
            {
                Debug.LogError("Angle value is higher than possible maximum for this algorithm.");
                angles.intermAngles.Clear();
                return false;
            }
            Figure[1] = -(float)(Figure[0] - g + Math.Atan(p / m));
            if (Math.Abs(Figure[1]) > (Math.PI / 2))
            {
                Debug.LogError("Angle value is higher than possible maximum for this algorithm.");
                angles.intermAngles.Clear();
                return false;
            }
            Figure[2] = -(float)g;
            Figure[3] = -(float)g;
            Figure[4] = (float)(g + Math.Atan(p / m) - Figure[5]);
            for (int j = 0; j < 6; j++)
            {
                Figure[j] *= (float)(180 / Math.PI);
            }
            angles.intermAngles.Add(Figure.Clone() as float[]);
        }
        angles.bottom = (float)(a * 180 / Math.PI);
        angles.wave = -(float)(waveAngle * 180 / Math.PI);
        angles.vertex = -(float)(vertexAngle * 180 / Math.PI);
        return true;
    }

    public void Wave(bool isReversed)
    {
        WaveGen.Sequences.Clear();
        float[] Figure = new float[(NumberOfModulesInwave + 2) * ConfigurationOfRobot];
        for (int i = 0; i < Figure.Length; i++)
        {
            Figure[i] = 0;
        }
        if (angles.intermAngles.Count != 0)
        {
            for (int i = 0; i < angles.intermAngles.Count; i++)
            {       //Почему i = 1?
                for (int j = 0; j < 6; j++)
                {
                    Figure[j * ConfigurationOfRobot] = angles.intermAngles[i][j] * SideOnGroundVertical;
                }
                WaveGen.Sequences.Add(Figure.Clone() as float[]);
            }
        }

        Figure[0 * ConfigurationOfRobot] = 0;
        Figure[1 * ConfigurationOfRobot] = angles.bottom * SideOnGroundVertical;
        Figure[2 * ConfigurationOfRobot] = angles.wave * SideOnGroundVertical;
        Figure[3 * ConfigurationOfRobot] = angles.vertex * SideOnGroundVertical;
        Figure[4 * ConfigurationOfRobot] = angles.wave * SideOnGroundVertical;
        Figure[5 * ConfigurationOfRobot] = angles.bottom * SideOnGroundVertical;
        WaveGen.Sequences.Add(Figure.Clone() as float[]);
        if (isReversed)
        {
            WaveGen.StartMo(WaveGen.modules.Count + 2, (NumberOfModulesInwave - 2) * 2, isReversed, 2);
        }
        else
        {
            WaveGen.StartMo(-3, WaveGen.modules.Count + 2, isReversed, 2);
        }
        WaveGen.busy = true;
    }

    public bool DriversAreReady()
    {
        foreach (Driver drv in WaveGen.modules)
        {
            if (drv.busy)
                return false;
        }
        return true;
    }

    public void Stop()
    {
        WaveGen.Stop();
        waveDist = 0;
        WaveGen.TotalWaves = 0;
    }

    public void Go(int Dist_in_waves, double dl, double h, bool isReversed)
    {
        if (Dist_in_waves > 0)
        {
            if (Check(dl, h))
            {
                waveDist = Dist_in_waves;
                Wave(isReversed);
            }
        }
        busy = WaveGen.busy;
    }

    private void Start()
    {
        WaveGen = GetComponent<WaveGenerator>();
        if (WaveGen == null)
        {
            WaveGen = gameObject.AddComponent<WaveGenerator>();
        }
        FirstModuleOnGround = 1;
        angles.intermAngles = new List<float[]>();
    }
    // Update is called once per frame
    void Update()
    {
        if (WaveGen.TotalWaves >= waveDist)
            WaveGen.Stop();
        busy = WaveGen.busy;
    }
}