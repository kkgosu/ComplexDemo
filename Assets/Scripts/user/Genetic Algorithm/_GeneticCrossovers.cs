using System;
using System.Collections;
using System.Collections.Generic;

namespace GeneticLibrary {
	public class CrossoverBase {
		public CrossoverBase() {

		}
	}

	// Многоточечное скрещивание:
	public class CrossoverSimple : CrossoverBase {
		int discrete;
		int sliceTo;

		Random random = new Random();

		public CrossoverSimple(int _discrete, int _sliceTo) {
			discrete = _discrete;
			sliceTo = _sliceTo;
		}

		public List<Chromosome> GenerateNext(List<Chromosome> population) {
			List<Chromosome> newPopulation = new List<Chromosome> ();
			float fitnessSum = 0;
			Chromosome parent1, parent2;
			// Применим элитный способ отбора:
			List<Chromosome> elitePopulation = new List<Chromosome>();
			elitePopulation = EliteSelection (population);
			// Подсчитаем сумму всех значений фитнесс-функций:
			foreach (Chromosome chr in elitePopulation) {
				fitnessSum += chr.fitnessValue;
			}
			for (int c = 0; c < population.Count; c++) {
				parent1 = SelectParent (elitePopulation, fitnessSum);
				parent2 = SelectParent (elitePopulation, fitnessSum);

				newPopulation.Add (GetChild(parent1, parent2));
			}
			return newPopulation;
		}

		List<Chromosome> EliteSelection (List<Chromosome> population) {
			List<Chromosome> elitePopulation = new List<Chromosome> ();
			float fitnessSum = 0;
			foreach (Chromosome chr in population)
				fitnessSum += chr.fitnessValue;
			float fitnessAverage = fitnessSum / population.Count;
			foreach (Chromosome chr in population) {
				if (chr.fitnessValue >= fitnessAverage)
					elitePopulation.Add (chr);
			}
			return elitePopulation;	
		}

		public Chromosome SelectParent (List<Chromosome> population, float fitnessSum) {
			float rValue = (float) random.NextDouble() * fitnessSum;
			foreach (Chromosome chr in population) {
				if (chr.fitnessValue != 0 && rValue <= chr.fitnessValue)
					return chr;
				else
					rValue -= chr.fitnessValue;
			}
			return null;
		}

		public Chromosome GetChild(Chromosome parent1, Chromosome parent2) {
			int childLength;
			bool[] childGenes;
			int p1Length = parent1.genes.Length;
			int p2Length = parent2.genes.Length;
			if (p1Length != p2Length) {
				int maxLength, minLength;
				minLength = p1Length < p2Length ? p1Length : p2Length;
				maxLength = p1Length > p2Length ? p1Length : p2Length;
				childLength = random.Next (minLength, maxLength + sliceTo);
				childLength = childLength - (childLength % sliceTo);
			}
			else
				childLength = p1Length - (p1Length % sliceTo);

			childGenes = new bool[childLength];
			int counter = 0;
			bool[] parentGenes;
			while (counter + discrete <= childLength) {
				if (p1Length >= (counter + discrete) && p2Length >= (counter + discrete)) {
					if (random.Next (0, 2) == 0) {
						parentGenes = parent1.genes;
					} else {
						parentGenes = parent2.genes;
					}
				} else {
					if (p1Length >= (counter + discrete))
						parentGenes = parent1.genes;
					else
						parentGenes = parent2.genes;
				}
				for (int c = 0; c < discrete; c++) {
					childGenes [counter + c] = parentGenes [counter + c];
				}
				counter += discrete;
			}

			Chromosome child = new Chromosome (childGenes);
			return child;
		}
	}
}