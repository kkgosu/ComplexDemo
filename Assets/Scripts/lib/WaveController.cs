using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveController : MonoBehaviour {

//	bool isNegative = false;
//	int num = isNegative ? -1 : 1;

	public List<Driver> module= new List<Driver>();
	private bool waveConstantly, waveInProgress, kadochnikovWaveInProgress, turn;
	private struct io{
		public float bottom;
		public float wave;
		public float vertex;
		public List<float[]> intermAngles;
	}
	private io angles = new io ();				//углы текущей конфигурации волны
	int step = 0;									//синхронизация волн
	double PracticableWay;						//расстояние проходимое за 1 волну при текущей конфигурации волны
	int NumberOfModulesInwave;					//величина гребня волны в модулях
	List<int> MainModuls = new List<int> ();	// содержит номера модулей от которых идут гребни волн
	private int TotalWaves = 0;
	WaveControl WaveGen;
	//ModularRobot Robot;

	public int SideOnGroundHorizontal = -1;
	public int SideOnGroundVertical = - 1;		//переменная нужна что бы определить знак угла поворота привода. 1 означает, что для поворота от земли подаётся положительный угол.
	public int FirstModuleOnGround;				//переменная показывает какой модуль первым лежит на земле нужной стороной
	public int LastModuleOnGround;				//аналогично предидущей переменной
	public int ConfigurationOfRobot = 1;		//показывает через сколько модулей идёт чередование. 1 означает что все модули на земле нужной стороной, 2 что через 1 и т.д.
	public int DirectionOfMotion = 1;			//направление движения
	public float MaxAngle = 90;
	public float MinAngle = -90;


	public void AddModulesFromRobot (ModularRobot robot){
		foreach (Module m in robot.modules.Values) {
			module.Add (m.drivers["q1"]);
		}
//		Robot = gameObject.AddComponent<ModularRobot>();
//		Robot = robot;
	}

	bool Check (double dl, double h){
		NumberOfModulesInwave = 4;
		angles.intermAngles.Clear ();
		double bottom = 4 - dl;
		double bottomAngle = Math.Atan (2 * h / bottom);

		double side = h / Math.Sin (bottomAngle);
		double waveAngle = 2*Math.Acos(side/2);

		double a = bottomAngle + waveAngle / 2;

		double vertexAngle = 2 * bottomAngle - waveAngle;
		double da = (a / (int)(a * 12 / Math.PI));
		float[] Figure = new float[6];
		for (double i = a - da; i > 0; i -= da) {
			Figure [0] = (float)i;
			Figure [5] = (float)(a - i);
			double c = bottom + 1;
			double m = c - Math.Cos (Figure [0]) - Math.Cos (Figure [5]);
			double p = Math.Sin (Figure [0]) - Math.Sin (Figure [5]);
			double s = m/(Math.Cos(Math.Atan(p/m)));
			double g = Math.Acos ((s - 1) / 2);
			Figure [1] = -(float)(Figure [0] - g + Math.Atan (p / m));
			Figure [2] = -(float)g;
			Figure [3] = -(float)g;
			Figure [4] = (float)(g + Math.Atan (p / m) - Figure [5]);
			for (int j = 0; j < 6; j++) {
				Figure [j] *= (float)(180 / Math.PI);
			}
			angles.intermAngles.Add (Figure.Clone () as float[]);

//			if (i < da) {
//				Figure [0] = 0;
//				Figure [5] = (float)a;
//				m = c - Math.Cos (Figure [0]) - Math.Cos (Figure [5]);
//				p = Math.Sin (Figure [0]) - Math.Sin (Figure [5]);
//				s = m/(Math.Cos(Math.Atan(p/m)));
//				g = Math.Acos ((s - 1) / 2);
//				Figure [1] = -(float)(Figure [0] - g + Math.Atan (p / m));
//				Figure [2] = -(float)g;
//				Figure [3] = -(float)g;
//				Figure [4] = (float)(g + Math.Atan (p / m) - Figure [5]);
//				for (int j = 0; j < 6; j++) {
//					Figure [j] *= (float)(180 / Math.PI);
//				}
//				angles.intermAngles.Add (Figure.Clone () as float[]);
//			}

		}
		angles.bottom = (float)(a * 180 / Math.PI);
		angles.wave = -(float)(waveAngle * 180 / Math.PI);
		angles.vertex = -(float)(vertexAngle * 180 / Math.PI);
		return true;
	}

	void Wave(int ExpectableNumberOfModulesInwave, float a) // движение волной, теоретически без проскальзывания
	{
		WaveGen.Sequences.Clear ();
		float b;
		b = (float)((-2 * a) / (ExpectableNumberOfModulesInwave - 1));		//угол в волне

		float C = 1;				//длинна основания волны
		for (int j = 0; j < ExpectableNumberOfModulesInwave; j++)
			C += (float)(Math.Cos ((a + b * j)*Math.PI/180));
		
		float b1 = 0;				//верхний угол треугольника
		b1 = (float)(Math.Acos ((-Math.Pow (C, 2) + Math.Pow (ExpectableNumberOfModulesInwave, 2) + 1) / (2 * (ExpectableNumberOfModulesInwave))));
		b1 = (float)(b1*180/Math.PI - 180);

		float b2;		//тупой угол треугольника
		b2 = (float)(180/Math.PI*Math.Acos ((Math.Pow (ExpectableNumberOfModulesInwave, 2) - Math.Pow (C, 2) - 1) / (-2 * C)));

		float b3;		//острый угол треугольника
		b3 = -b2 - b1;

		float tr;			//угол в трапеции
		tr = (float)(180/Math.PI*Math.Acos ((C - ExpectableNumberOfModulesInwave + 1) / 2));

		a *= SideOnGroundVertical;
		b *= SideOnGroundVertical;
		b1 *= SideOnGroundVertical;
		b2 *= SideOnGroundVertical;
		b3 *= SideOnGroundVertical;
		tr *= SideOnGroundVertical;
		PracticableWay = ExpectableNumberOfModulesInwave - C + 1;
		NumberOfModulesInwave = ExpectableNumberOfModulesInwave;

		float[] Figure = new float[(NumberOfModulesInwave + 2) * ConfigurationOfRobot];
		for (int i = 0; i<Figure.Length; i++) {
			Figure[i] = 0;
		}
		Figure [0] = b2;
		Figure [ConfigurationOfRobot] = b1;
		Figure [(NumberOfModulesInwave + 1) * ConfigurationOfRobot] = b3;
		WaveGen.Sequences.Add (Figure.Clone() as float[]);

		for (int i = 0; i<Figure.Length; i++) {
			Figure[i] = 0;
		}
		Figure [0] = tr;
		Figure [ConfigurationOfRobot] = -tr;
		Figure [NumberOfModulesInwave * ConfigurationOfRobot] = -tr;
		Figure [(NumberOfModulesInwave + 1) * ConfigurationOfRobot] = tr;
		WaveGen.Sequences.Add (Figure.Clone() as float[]);

		for (int i = 0; i<Figure.Length; i++) {
			Figure[i] = 0;
		}
		Figure [0] = b3;
		Figure [NumberOfModulesInwave * ConfigurationOfRobot] = b1;
		Figure [(NumberOfModulesInwave + 1) * ConfigurationOfRobot] = b2;
		WaveGen.Sequences.Add (Figure.Clone() as float[]);

		for (int i = 0; i<Figure.Length; i++) {
			Figure[i] = 0;
		}
		Figure [0] = 0;
		Figure [ConfigurationOfRobot] = a;
		Figure [(NumberOfModulesInwave + 1) * ConfigurationOfRobot] = a;
		for (int i = 2 * ConfigurationOfRobot; i < (NumberOfModulesInwave + 1) * ConfigurationOfRobot; i += ConfigurationOfRobot) {
			Figure [i] = b;
		}
		WaveGen.Sequences.Add (Figure.Clone() as float[]);
		WaveGen.StartMo (true);
	}

	public void Kadochnikov(int NumberOfModulesInwave, float a)
	{
		WaveGen.Sequences.Clear ();
		a *= SideOnGroundVertical;
		float b;
		b = (float)((-2 * a) / (NumberOfModulesInwave - 1));	//угол в волне
		float[] Figure = new float[(NumberOfModulesInwave + 2) * ConfigurationOfRobot];
		for (int i = 0; i<Figure.Length; i++) {
			Figure[i] = 0;
		}
		Figure [0] = 0;
		Figure [ConfigurationOfRobot] = a;
		Figure [(NumberOfModulesInwave + 1) * ConfigurationOfRobot] = a;
		for (int i = 2 * ConfigurationOfRobot; i < (NumberOfModulesInwave + 1) * ConfigurationOfRobot; i += ConfigurationOfRobot) {
			Figure [i] = b;
		}
		WaveGen.Sequences.Add (Figure.Clone() as float[]);
		WaveGen.StartMo (true);
	}
		
	void Fold(float a)			//поворот через мостик
	{
		if (DriversAreReady () && turn) {
			int crutch = 1;
			int middle = (int)(module.Count / 2);
			if (module.Count % 2 != 0) {
				//middle++;
			}
			if ((middle - FirstModuleOnGround) % ConfigurationOfRobot != 0) {
				crutch = 0;
			}
			a *= SideOnGroundHorizontal;
			switch (step) {
			case 1:
				module [middle - 1 - ConfigurationOfRobot * crutch].Set (10 * SideOnGroundVertical);
				module [middle + 1 + ConfigurationOfRobot * crutch].Set (10 * SideOnGroundVertical);
				break;
			case 2:
				module [middle - 2 - ConfigurationOfRobot * crutch].Set (a * SideOnGroundVertical);
				module [middle + 2 + ConfigurationOfRobot * crutch].Set (-a * SideOnGroundVertical);

				module [middle - 4 - ConfigurationOfRobot * crutch].Set (a / 2 * SideOnGroundVertical);
				module [middle + 4 + ConfigurationOfRobot * crutch].Set (-a / 2 * SideOnGroundVertical);
				break;
			case 3:
				module [middle - 1 - ConfigurationOfRobot * crutch].Set (0);
				module [middle + 1 + ConfigurationOfRobot * crutch].Set (0);
				break;
			case 4:
				float b = a / 2;
				float x = 1 + 2 * Mathf.Cos ((float)(b / 57.29577951)) + Mathf.Cos ((float)((b - a) / 57.29577951));
				float a1 = (float)(Mathf.Acos (x / (4)) * 57.29577951);
				module [middle - 4 - ConfigurationOfRobot * crutch].speed *= 0.5f;
				module [middle + 4 - ConfigurationOfRobot * crutch].speed *= 0.5f;
				module [middle - 1 - ConfigurationOfRobot * crutch].speed *= a1 / Math.Abs (a);
				module [middle + 1 + ConfigurationOfRobot * crutch].speed *= a1 / Math.Abs (a);
				module [middle - 5 - ConfigurationOfRobot * crutch].speed *= a1 / Math.Abs (a);
				module [middle + 5 + ConfigurationOfRobot * crutch].speed *= a1 / Math.Abs (a);

				module [middle - 2 - ConfigurationOfRobot * crutch].Set (0);
				module [middle + 2 + ConfigurationOfRobot * crutch].Set (0);

				module [middle - 4 - ConfigurationOfRobot * crutch].Set (0);
				module [middle + 4 + ConfigurationOfRobot * crutch].Set (0);

				module [middle - 1 - ConfigurationOfRobot * crutch].Set (-a1 * SideOnGroundVertical);
				module [middle + 1 + ConfigurationOfRobot * crutch].Set (-a1 * SideOnGroundVertical);

				module [middle - 5 - ConfigurationOfRobot * crutch].Set (a1 * SideOnGroundVertical);
				module [middle + 5 + ConfigurationOfRobot * crutch].Set (a1 * SideOnGroundVertical);
				break;
			case 5:
				module [middle - 2 - ConfigurationOfRobot * crutch].Set (-a* SideOnGroundVertical);
				module [middle + 2 + ConfigurationOfRobot * crutch].Set (a* SideOnGroundVertical);

				module [middle - 4 - ConfigurationOfRobot * crutch].Set (-a / 2* SideOnGroundVertical);
				module [middle + 4 + ConfigurationOfRobot * crutch].Set (a / 2* SideOnGroundVertical);

				module [middle - 1 - ConfigurationOfRobot * crutch].Set (0);
				module [middle + 1 + ConfigurationOfRobot * crutch].Set (0);

				module [middle - 5 - ConfigurationOfRobot * crutch].Set (0);
				module [middle + 5 + ConfigurationOfRobot * crutch].Set (0);
				break;
			case 6:
				module [middle - 4 - ConfigurationOfRobot * crutch].speed = module [middle - 2 - ConfigurationOfRobot * crutch].speed;
				module [middle + 4 - ConfigurationOfRobot * crutch].speed = module [middle - 2 - ConfigurationOfRobot * crutch].speed;
				module [middle - 1 - ConfigurationOfRobot * crutch].speed = module [middle - 2 - ConfigurationOfRobot * crutch].speed;
				module [middle + 1 + ConfigurationOfRobot * crutch].speed = module [middle - 2 - ConfigurationOfRobot * crutch].speed;
				module [middle - 5 - ConfigurationOfRobot * crutch].speed = module [middle - 2 - ConfigurationOfRobot * crutch].speed;
				module [middle + 5 + ConfigurationOfRobot * crutch].speed = module [middle - 2 - ConfigurationOfRobot * crutch].speed;
				module [middle - 1 - ConfigurationOfRobot * crutch].Set (10 * SideOnGroundVertical);
				module [middle + 1 + ConfigurationOfRobot * crutch].Set (10 * SideOnGroundVertical);
				break;
			case 7:
				module [middle - 2 - ConfigurationOfRobot * crutch].Set (0);
				module [middle + 2 + ConfigurationOfRobot * crutch].Set (0);

				module [middle - 4 - ConfigurationOfRobot * crutch].Set (0);
				module [middle + 4 + ConfigurationOfRobot * crutch].Set (0);
				break;
			case 8:
				module [middle - 1 - ConfigurationOfRobot * crutch].Set (0);
				module [middle + 1 + ConfigurationOfRobot * crutch].Set (0);
				turn = false;
				break;
			}
			step++;
		}
	}
	void Wave ()
	{
		WaveGen.Sequences.Clear ();
		float[] Figure = new float[(NumberOfModulesInwave + 2) * ConfigurationOfRobot];
		for (int i = 0; i<Figure.Length; i++) {
			Figure[i] = 0;
		}
		if (angles.intermAngles.Count != 0) {
			for (int i = 1; i < angles.intermAngles.Count; i++) {
				for (int j = 0; j < 6; j++) {
					Figure [j*ConfigurationOfRobot] = angles.intermAngles [i] [j] * SideOnGroundVertical;
				}
				WaveGen.Sequences.Add (Figure.Clone () as float[]);
			}
		}

		Figure [0] = 0;
		Figure [1] = angles.bottom * SideOnGroundVertical;
		Figure [2] = angles.wave * SideOnGroundVertical;
		Figure [3] = angles.vertex * SideOnGroundVertical;
		Figure [4] = angles.wave * SideOnGroundVertical;
		Figure [5] = angles.bottom * SideOnGroundVertical;
		WaveGen.Sequences.Add (Figure.Clone () as float[]);
		WaveGen.StartMo (true);
	}

	public bool DriversAreReady () {
		foreach (Driver drv in module) {
			if (drv.busy)
				return false;
		}
		return true;
	}

	// Use this for initialization
	public void Init() {
		FirstModuleOnGround = 1;
		LastModuleOnGround = module.Count;
		WaveGen = gameObject.GetComponent<WaveControl> ();
		angles.intermAngles = new List<float[]> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.W) && WaveGen.ready) {
			if(Check(1.5, 1.5)){
                Wave ();
                print("Yo!");
            }
		}
		if (Input.GetKeyDown (KeyCode.O) && WaveGen.ready) {
			Wave (4, 45);
		}

		if (Input.GetKeyDown (KeyCode.K) && WaveGen.ready) {
			Kadochnikov (4, 45);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			WaveGen.Stop();
		}

		if (Input.GetKeyDown (KeyCode.F) && WaveGen.ready) {
			step = 1;
			turn = true;
		}
		if (turn) {
			Fold (45);
		}
	}
}