using System.Collections;
using System.Collections.Generic;

namespace GeneticLibrary {
	public class Chromosome {
		public bool[] genes;
		public float fitnessValue;
		public bool isChecked;

		public Chromosome(bool [] _genes, bool _isChecked = false) {
			genes = _genes;
			isChecked = _isChecked;
		}
	}
}