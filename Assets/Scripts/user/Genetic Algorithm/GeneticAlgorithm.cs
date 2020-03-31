using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticLibrary;


// Rename to GeneticController!

public class GeneticAlgorithm : MonoBehaviour {

	public static GeneticAlgorithm instance;

	/// <summary>
	/// Создадим синглтон данного класса.
	/// </summary>
	void Awake()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);    

		DontDestroyOnLoad(gameObject);
	}
		
	Utilities.CSVWriter data;
	
	CrossoverSimple crossover;
	MutationSimple mutation;

	int terminationGeneration;

	List<Chromosome> population;
	Chromosome bestChromosome;

	System.Random random = new System.Random();

	public void StartLearning(int populationSize, int[] chromosomeSizes,
		CrossoverSimple _crossover, MutationSimple _mutation, int _terminationGeneration) {
		data = new Utilities.CSVWriter ("ГА", "Среднее значение; Лучшее значение");

		crossover = _crossover;
		mutation = _mutation;
		terminationGeneration = _terminationGeneration;

		population = CreateFirstGeneration (populationSize, chromosomeSizes);
		//StartCoroutine (Learning ());
	}

	public List<Chromosome> CreateFirstGeneration(int populationSize, int[] chromosomeSizes) {
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

	/*IEnumerator Learning() {
		int generationCounter = 0;
		while (generationCounter < terminationGeneration) {
			UnityEngine.Debug.LogWarning (string.Format("Начинаю работу с {0} поколением.", generationCounter + 1));
			GeneticFitness.instance.inProgress = true;
			GeneticFitness.instance.StartEvaluating (population);
			//GeneticFitness.instance.TestPopulation (population);
			while (GeneticFitness.instance.inProgress)
				yield return new WaitForEndOfFrame ();
			UpdateBestChromosome();
			UpdateData ();

			population = crossover.GenerateNext (population);
			population = mutation.ChangePopulation (population);
			population [0] = CopyChromosome (bestChromosome);
			generationCounter++;
		}
		string genes = string.Format("Лучшая хромосома с функцией полезности {0}: ", bestChromosome.fitnessValue);
		foreach (bool element in bestChromosome.genes)
			genes += string.Format("{0}, ", element.ToString ());
		UnityEngine.Debug.LogWarning (genes);
		UnityEngine.Debug.LogWarning ("Начинаю воспроизведение данного алгоритма.");
		GeneticFitness.instance.StartInfiniteMotion (bestChromosome);
		yield return null;
	}*/

	void UpdateData() {
		float bestFitnessValue = 0, summedFitnessValue = 0;
		foreach (Chromosome chr in population) {
			if (chr.fitnessValue > bestFitnessValue)
				bestFitnessValue = chr.fitnessValue;
			summedFitnessValue += chr.fitnessValue;
		}
		float averageFitnessValue = summedFitnessValue / population.Count;
		data.AddLine (string.Format("{0};{1}", averageFitnessValue, bestFitnessValue));
	}

	void UpdateBestChromosome() {
		foreach (Chromosome chr in population) {
			if (bestChromosome == null)
				bestChromosome = CopyChromosome(chr);
			if (chr.fitnessValue > bestChromosome.fitnessValue)
				bestChromosome = CopyChromosome(chr);
		}
	}

	Chromosome CopyChromosome(Chromosome donor) {
		Chromosome recipient;
		bool[] recGenes = new bool[donor.genes.Length];
		donor.genes.CopyTo (recGenes, 0);
		recipient = new Chromosome (recGenes);
		recipient.fitnessValue = donor.fitnessValue;
		return recipient;
	}
}
