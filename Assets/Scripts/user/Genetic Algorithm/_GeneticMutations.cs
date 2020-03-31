using System;
using System.Collections;
using System.Collections.Generic;

namespace GeneticLibrary {
	public class MutationBase {

	}

	public class MutationSimple : MutationBase {
		float mutationChance;

		Random random = new Random ();

		public MutationSimple(float _mutationChance) {
			mutationChance = _mutationChance;
		}

		public List<Chromosome> ChangePopulation(List<Chromosome> population) {
			List<Chromosome> newPopulation = new List<Chromosome>();
			foreach (Chromosome chr in population)
				newPopulation.Add(ChangeChromosome (chr));
			return newPopulation;
		}

		public Chromosome ChangeChromosome(Chromosome chr) {
			bool[] genes = new bool[chr.genes.Length];
			chr.genes.CopyTo (genes, 0);
			for (int c = 0; c < chr.genes.Length; c++) {
				if (random.NextDouble() < mutationChance)
					genes[c] = !genes[c];
			}
			return new Chromosome (genes);
		}
	}
}