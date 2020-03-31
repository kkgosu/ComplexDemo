using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

// ОСНОВНОЙ И ЕДИНСТВЕННЫЙ СКРИПТ, КОТОРЫЙ ДОЛЖЕН БЫТЬ РАЗМЕЩЕН НА СЦЕНЕ, ДЛЯ РАБОТЫ САМООБУЧЕНИЯ.

public class LearningController : MonoBehaviour {

	// TODO:
	// **1. Модифицировать DCT;
    // **2. Класс Robot с Component DCT;**
	// **3. Создание / перестроение роботов;
	// **4. Ген. Алгоритм;
	// **5. СКА.

	public Button buttonGA;
	public Button buttonFSM;


	[System.Serializable]
	public struct MovementSettings
	{
		public float downAngle;
		public float upAngle;
		public float horizontalAngle;
	}

	public MovementSettings movementSettings;

	public float downAngle = 45;
	public float upAngle = 20;
	public float horizontalAngle = 20;

	[System.Serializable]
	public struct GeneticAlgorithm
	{
		public int populationSize;
		public int generations;
		public float mutationChance;
		public int crossoverDiscrete;
		public int sliceTo;
		public int minChromosomeLength;
		public int maxChromosomeLength;
	}

	public GeneticAlgorithm geneticAlgorithm;
	GeneticController geneticController;

	[System.Serializable]
	public struct FSM
	{

	}

	public FSM finalStateMachine;
	FSMController fsmController;

	enum legMovements {upforward, downforward, upbackward, downbackward};

	void Start () {
		buttonGA.GetComponent<Button>().onClick.AddListener(StartGeneticLearning);
		buttonFSM.GetComponent<Button>().onClick.AddListener(StartFSMLearning);
		Debug.LogError (CalculateFitness(new Vector3 (5, 3, 5), new Vector3 (-1, 3, -1), 0, 0));
	}

/*	void PrototypeDecodeAndSend (DCT dct, bool [] chr) {
		foreach (int[] line in DecodeChromosome(chr)) {
			FormControlLine (dct, line);
		}
	}

	int[][] DecodeChromosome (bool [] chr) {
		for (int i = 0; i < chr.Length; i++) {
			chr [i % 8] --- новое действие.
			chr [i % 2] --- новая конечность.
			if (i % 2 == 0) --- вертикальное перемещение.
		}
	}
*/

	void StartGeneticLearning () {
		geneticController = gameObject.AddComponent <GeneticController> ();
		geneticController.learningController = GetComponentInChildren <LearningController> ();
		geneticController.populationSize = 5;
     	geneticController.generations = 2;
		geneticController.mutationChance = 0.35f;
  		geneticController.crossoverDiscrete = 2;
     	geneticController.sliceTo = 8;
     	geneticController.minChromosomeLength = 16;
     	geneticController.maxChromosomeLength = 40;


		buttonGA.gameObject.SetActive (false);
		buttonFSM.gameObject.SetActive (false);
	}

	void StartFSMLearning () {
		fsmController = gameObject.AddComponent <FSMController> ();
		fsmController.learningController = GetComponentInChildren <LearningController> ();
		buttonGA.gameObject.SetActive (false);
		buttonFSM.gameObject.SetActive (false);
	}

	Vector3 realMovement = new Vector3 (1, 0, 1);

	public float CalculateFitness (Vector3 fromPosition, Vector3 toPosition, float totalDistance, float timeSpent) {
		float realDistance = Vector3.Distance (Vector3.zero, Vector3.Project(toPosition - fromPosition, realMovement));
		float result = realDistance * (realDistance / totalDistance) * (1 / timeSpent);
		return result;
	}

	public void FormControlLine (DCT dct, int[] legStates) {
		int count = 0;
		List <float> downAngles = new List <float> ();
		List <float> upAngles = new List <float> ();
		List <float> horizontalAngles = new List <float> ();
		List <int> downDrivers = new List <int> ();
		List <int> upDrivers = new List <int> ();
		List <int> horizontalDrivers = new List <int> ();
		foreach (int state in legStates) {
			if (state % 2 == 0) {
				upAngles.Add (upAngle);
				upDrivers.Add (1 + count * 3);
				upAngles.Add (90 - upAngle);
				upDrivers.Add (2 + count * 3);
			} else {
				downAngles.Add (downAngle);
				downDrivers.Add (1 + count * 3);
				downAngles.Add (90 - downAngle);
				downDrivers.Add (2 + count * 3);
			}
			horizontalAngles.Add((state < 2) ? horizontalAngle : (-1) * horizontalAngle);
			horizontalDrivers.Add (count * 3);
			count++;
		}
		dct.data.AddLine (downAngles.ToArray(), downDrivers.ToArray());
		dct.data.AddLine (upAngles.ToArray(), upDrivers.ToArray());
		dct.data.AddLine (horizontalAngles.ToArray(), horizontalDrivers.ToArray());
	}
}