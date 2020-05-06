using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveController_5 : MonoBehaviour {

    //	bool isNegative = false;
    //	int num = isNegative ? -1 : 1;
    public int waveDist = 0;
    public bool busy = false;
	private struct io{
		public float bottom;
		public float wave;
		public float vertex;
		public List<float[]> intermAngles;
	}
	private io angles = new io ();				//углы текущей конфигурации волны
	int numberOfModulesInwave;					//величина гребня волны в модулях
	WaveGenerator waveGen;

	public int sideOnGroundVertical =  1;		//переменная нужна что бы определить знак угла поворота привода. 1 означает, что для поворота от земли подаётся положительный угол.
	public int firstModuleOnGround;				//переменная показывает какой модуль первым лежит на земле нужной стороной
	public int configurationOfRobot = 2;		//показывает через сколько модулей идёт чередование. 1 означает что все модули на земле нужной стороной, 2 что через 1 и т.д.
	public int directionOfMotion = 1;			//направление движения
	public float maxAngle = 90;
	public float minAngle = -90;


    public double[] MinMaxH(double dl)
    {
        numberOfModulesInwave = 4;
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
        double[] ret = new double[2]{ minH, maxH };
        return ret;
    }

	public bool Check (double dl, double h){
		numberOfModulesInwave = 4;
		angles.intermAngles.Clear ();
		double bottom = 4 - dl;
		double bottomAngle = Math.Atan (2 * h / bottom);

		double side = h / Math.Sin (bottomAngle);
		double waveAngle = 2*Math.Acos(side/2);
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
		float[] figure = new float[6];
		for (double i = a - da; i > 0; i -= da) {
			figure [0] = (float)i;
			figure [5] = (float)(a - i);
			double c = bottom + 1;
			double m = c - Math.Cos (figure [0]) - Math.Cos (figure [5]);
			double p = Math.Sin (figure [0]) - Math.Sin (figure [5]);
			double s = m/(Math.Cos(Math.Atan(p/m)));
			double g = Math.Acos ((s - 1) / 2);
            if (Math.Abs(g) > (Math.PI / 2))
            {
                Debug.LogError("Angle value is higher than possible maximum for this algorithm.");
                angles.intermAngles.Clear();
                return false;
            }
            figure [1] = -(float)(figure [0] - g + Math.Atan (p / m));
            if (Math.Abs(figure[1]) > (Math.PI / 2))
            {
                Debug.LogError("Angle value is higher than possible maximum for this algorithm.");
                angles.intermAngles.Clear();
                return false;
            }
            figure [2] = -(float)g;
			figure [3] = -(float)g;
			figure [4] = (float)(g + Math.Atan (p / m) - figure [5]);
			for (int j = 0; j < 6; j++) {
				figure [j] *= (float)(180 / Math.PI);
			}
			angles.intermAngles.Add (figure.Clone () as float[]);
		}
		angles.bottom = (float)(a * 180 / Math.PI);
		angles.wave = -(float)(waveAngle * 180 / Math.PI);
		angles.vertex = -(float)(vertexAngle * 180 / Math.PI);
		return true;
	}

	public void Wave (bool isReversed)
	{
		waveGen.Sequences.Clear ();
		float[] figure = new float[(numberOfModulesInwave + 2) * configurationOfRobot];
		for (int i = 0; i<figure.Length; i++) {
			figure[i] = 0;
		}
		if (angles.intermAngles.Count != 0) {
			for (int i = 0; i < angles.intermAngles.Count; i++) {       //Почему i = 1?
				for (int j = 0; j < 6; j++) {
                    figure [j * configurationOfRobot] = angles.intermAngles [i] [j] * sideOnGroundVertical;
				}
				waveGen.Sequences.Add (figure.Clone () as float[]);
			}
		}

		figure [0 * configurationOfRobot] = 0;
		figure [1 * configurationOfRobot] = angles.bottom * sideOnGroundVertical;
		figure [2 * configurationOfRobot] = angles.wave * sideOnGroundVertical;
		figure [3 * configurationOfRobot] = angles.vertex * sideOnGroundVertical;
		figure [4 * configurationOfRobot] = angles.wave * sideOnGroundVertical;
		figure [5 * configurationOfRobot] = angles.bottom * sideOnGroundVertical;
		waveGen.Sequences.Add (figure.Clone () as float[]);
        if(isReversed)
        {
            waveGen.StartMo(waveGen.modules.Count + 2, (numberOfModulesInwave - 2) * 2, isReversed, 2);
        }
        else
        {
            waveGen.StartMo(-3, waveGen.modules.Count + 2, isReversed, 2);
        }
        waveGen.busy = true;
	}

	public bool DriversAreReady () {
		foreach (Driver drv in waveGen.modules) {
			if (drv.busy)
				return false;
		}
		return true;
	}

    public void Stop () {
        waveGen.Stop();
        waveDist = 0;
        waveGen.TotalWaves = 0;
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
        busy = waveGen.busy;
    }

    private void Start()
    {
        waveGen = GetComponent<WaveGenerator>();
        if (waveGen == null)
        {
            waveGen = gameObject.AddComponent<WaveGenerator>();
        }
        firstModuleOnGround = 1;
        angles.intermAngles = new List<float[]>();
    }
    // Update is called once per frame
    void Update () {
        if (waveGen.TotalWaves >= waveDist)
            waveGen.Stop();
        busy = waveGen.busy;
    }
}