// Rename to GeneticController.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticLibrary;

public class GeneticController : MonoBehaviour {

	public LearningController learningController;

	public int populationSize = 20;
	public int generations = 15;
	public float mutationChance = 0.35f;
	public int crossoverDiscrete = 8;
	public int sliceTo = 8;
	public int minChromosomeLength = 16;
	public int maxChromosomeLength = 48;
	public int robotsNumber = 5;

	private int[] chromosomeSizes;

	Utilities.CSVWriter data;

	List <Chromosome> population;
	List <RobotInsectoid> robots = new List <RobotInsectoid> ();
	CrossoverSimple crossover;
	MutationSimple mutation;

	Chromosome bestChromosome;

	private bool evaluationInProgress = false;

	System.Random random = new System.Random();

	void Start () {
		BuildRobots ();
		Debug.Log ("Начинаю обучение робота походке с помощью эволюционного алгоритма.");
		Debug.Log (String.Format("(Размер популяции: {0}, шанс мутации: {1}, кроссовер выполняется для " +
			"участков хромосомы размером {2}, завершение работы алгоритма после {3} поколений).", 
			populationSize, mutationChance, crossoverDiscrete, generations));
		chromosomeSizes = CalculatePossibleChromosomeSizes ();
		mutation = new MutationSimple (mutationChance);
		crossover = new CrossoverSimple (crossoverDiscrete, sliceTo);
		StartLearning (populationSize, chromosomeSizes);
	}

	private void BuildRobots () {
		for (int i = 0; i < robotsNumber; i++) {
			BuildRobot (i);
		}
	}

	private RobotInsectoid BuildRobot (int id = 0, bool rebuild = false) {
		if (rebuild)
			DestroyRobot (id);
		GameObject robotParent = new GameObject ();
		robotParent.transform.name = string.Format("(GA) Robot {0}", id);
		RobotInsectoid robot = robotParent.AddComponent <RobotInsectoid> ();
		robot.Create (new Vector3 (1, 0, 1) * (-10 + id * 5));
		robot.id = id;
		if (robots.Count > id)
			robots.Insert (id, robot);
		else
			robots.Add (robot);
		return robot;
	}

	private void DestroyRobot (int id) {
		if (robots.Count > id) {
			Destroy (robots[id].gameObject);
			robots.RemoveAt (id);
		} else
			Debug.LogError (string.Format("Cannot destroy robot with ID {0}.", id));
	}

	private int [] CalculatePossibleChromosomeSizes () {
		int arraySize = ((maxChromosomeLength - minChromosomeLength) / sliceTo) + 1;
		int[] sizes = new int[arraySize];
		for (int i = 0; i < arraySize; i++) {
			sizes [i] = minChromosomeLength + i * sliceTo;
		}
		return sizes;
	}

	public void StartLearning(int populationSize, int[] chromosomeSizes) {
		data = new Utilities.CSVWriter ("ГА", "Среднее значение; Лучшее значение");
		population = GenerateFirstGeneration (populationSize, chromosomeSizes);
		StartCoroutine (Learning ());
	}

	public List<Chromosome> GenerateFirstGeneration(int populationSize, int[] chromosomeSizes) {
		List<Chromosome> firstGeneration = new List<Chromosome>();
		Chromosome chromosome;
		int chromosomeSize;

		for (int c = 0; c < populationSize; c++) {
			chromosomeSize = chromosomeSizes [random.Next (0, chromosomeSizes.Length)];
			bool[] genes = new bool[chromosomeSize];
			for (int g = 0; g < genes.Length; g++) {
				genes [g] = Convert.ToBoolean(random.Next (0, 2));
			}
			chromosome = new Chromosome (genes);
			firstGeneration.Add (chromosome);
		}
		return firstGeneration;
	}

	IEnumerator Learning() {
		int generationCounter = 0;
		while (generationCounter < generations) {
			UnityEngine.Debug.LogWarning (string.Format("Начинаю работу с {0} поколением.", generationCounter + 1));
			evaluationInProgress = true;
			StartCoroutine (EvaluatePopulation());
			while (evaluationInProgress)
				yield return new WaitForEndOfFrame ();
			UpdateBestChromosome();
			UpdateData ();

			population = crossover.GenerateNext (population);
			population = mutation.ChangePopulation (population);
			population [0] = CopyChromosome (bestChromosome);
			generationCounter++;
		}
		string genes = string.Format("Лучшая хромосома с функцией полезности {0:F4}: ", bestChromosome.fitnessValue);
		foreach (bool element in bestChromosome.genes)
			genes += string.Format("{0}, ", element.ToString ());
		UnityEngine.Debug.LogWarning (genes);
		UnityEngine.Debug.LogWarning ("Начинаю воспроизведение данного алгоритма.");
		do {
			DestroyRobot (0);
		} while (robots.Count > 0);
		RobotInsectoid robot = BuildRobot ();
		foreach (int[] states in DecodeChromosome(bestChromosome.genes)){
			learningController.FormControlLine (robot.dct, states);
		}
		robot.dct.BeginLoop ();

	}

	List <int[]> DecodeChromosome (bool [] chr) {
		List <int[]> legStates = new List <int[]> ();
		int[] legState = new int[] {0, 0, 0, 0};
		int count = 0;
		foreach (bool gene in chr) {
			if (count > 0 && count % sliceTo == 0) {
				legStates.Add (legState);
				legState = new int[] {0, 0, 0, 0};
			}
			if (count % 2 == 0) {
				if (!gene)
					legState [(count % sliceTo) / 2] = 2; // Если в первом разряде единица, алгоритм 0 или 1, иначе - 2 или 3.
			} else {
				if (gene)
					legState [(count % sliceTo) / 2] += 1;
			}
			count ++;
		}
		legStates.Add (legState);
		return legStates;
	}

	private bool RobotsAreReady () {
		foreach (RobotInsectoid robot in robots) {
			if (robot.dct.inProgress)
				return false;
		}
		return true;
	}

	IEnumerator EvaluatePopulation () {
		Queue <Chromosome> queue = new Queue <Chromosome> (population);
		foreach (RobotInsectoid robot in robots) {
			StartCoroutine (RobotWorker(robot, queue));
		}
		do {
			yield return new WaitForEndOfFrame();
		} while (!RobotsAreReady());
		evaluationInProgress = false;
	}

	IEnumerator RobotWorker (RobotInsectoid robot, Queue <Chromosome> queue) {
		Chromosome chr;
		while (queue.Count > 0 && !robot.dct.inProgress) {
			chr = queue.Dequeue ();
			if (chr.isChecked)
				continue;
			chr.isChecked = true;
			foreach (int[] states in DecodeChromosome(chr.genes)){
				learningController.FormControlLine (robot.dct, states);
			}
			robot.dct.BeginTillEnd ();
			while (robot.dct.inProgress)
				yield return new WaitForEndOfFrame ();
			chr.fitnessValue = learningController.CalculateFitness(robot.fromPosition, robot.position, robot.totalDistance, (chr.genes.Length / sliceTo));
			robot = BuildRobot (robot.id, rebuild: true);
			yield return new WaitForEndOfFrame ();
		}
	}

	void UpdateData() {
		float bestFitnessValue = 0, summedFitnessValue = 0;
		foreach (Chromosome chr in population) {
			if (chr.fitnessValue > bestFitnessValue)
				bestFitnessValue = chr.fitnessValue;
			summedFitnessValue += chr.fitnessValue;
		}
		float averageFitnessValue = summedFitnessValue / population.Count;
		data.AddLine (string.Format("{0:F5};{1:F5}", averageFitnessValue, bestFitnessValue));
	}

	void UpdateBestChromosome() {
		foreach (Chromosome chr in population) {
			if (bestChromosome == null)
				bestChromosome = CopyChromosome(chr);
			if (chr.fitnessValue > bestChromosome.fitnessValue)
				bestChromosome = CopyChromosome(chr);
		}
	}

	Chromosome CopyChromosome(Chromosome donor, bool fullCopy = true) {
		Chromosome recipient;
		bool[] recGenes = new bool[donor.genes.Length];
		donor.genes.CopyTo (recGenes, 0);
		recipient = new Chromosome (recGenes, _isChecked: true);
		if (fullCopy)
			recipient.fitnessValue = donor.fitnessValue;
		return recipient;
	}
}
