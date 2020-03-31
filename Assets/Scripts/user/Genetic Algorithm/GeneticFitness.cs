using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticLibrary;

public class GeneticFitness : MonoBehaviour {
	/*
	public static GeneticFitness instance;

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

	System.Random random = new System.Random();

	public bool inProgress = false, busy = false;


	private GameObject refferencedModule;

	// Управление конечностями:

	bool[] controlBusy = {false, false, false, false};
	public float delta = 15f;
	public float time = 1.0f;

	IEnumerator MoveLegHorizontalPositive (int leg) {
		controlBusy [leg - 1] = true;
		float elapsedTime = 0;
		//motionControl.MoveLegHorizontal (leg, delta, time);
		while(elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		controlBusy [leg - 1] = false;
	}

	IEnumerator MoveLegHorizontalNegative (int leg) {
		controlBusy [leg - 1] = true;
		float elapsedTime = 0;
		//motionControl.MoveLegHorizontal (leg, delta * (-1), time);
		while(elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		controlBusy [leg - 1] = false;
	}

	IEnumerator MoveLegHorizontalNeutral (int leg) {
		controlBusy [leg - 1] = true;
		float elapsedTime = 0;
		//motionControl.MoveLegHorizontal (leg, 0, time);
		while(elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		controlBusy [leg - 1] = false;
	}

	IEnumerator MoveLegVertical (int leg) {
		controlBusy [leg - 1] = true;
		float elapsedTime = 0;
		//motionControl.MoveLegVertical (leg, time);
		while(elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		controlBusy [leg - 1] = false;
	}

	// Управление конечностями закончено.

	public void StartEvaluating(List<Chromosome> population) {
		motionControl = GetComponent<MotionControl> ();
		builder = GetComponent<Builder> ();

		StartCoroutine(EvaluatePopulation(population));
	}

	IEnumerator EvaluatePopulation(List<Chromosome> population) {
		foreach (Chromosome chr in population) {
			busy = true;
			StartCoroutine (EvaluateChromosome (chr));
			while (busy)
				yield return new WaitForEndOfFrame ();
		}
		inProgress = false;
		yield return null;
	}

	IEnumerator EvaluateChromosome(Chromosome chromosome) {
		builder.StartRebuildInsectRobot ();
		while (builder.insectIsBusy)
			yield return new WaitForEndOfFrame ();
		yield return new WaitForSeconds (0.25f);
		string genes = "Тестирую хромосому: ";
		int counter = 0;
		foreach (bool element in chromosome.genes) {
			if (counter % 12 == 0 && counter != 0)
				genes += " ";
			genes += string.Format ("{0}", element ? "1" : "0");
			counter++;
		}
		UnityEngine.Debug.Log (genes);
		refferencedModule = GameObject.Find ("Module 0/Base").gameObject;
		Vector3 startPosition = refferencedModule.transform.position;
		// Парсим хромосому по генам:
		int position = 0;
		while (position < chromosome.genes.Length) {
			for (int leg = 0; leg < 4; leg++) {
				if ((chromosome.genes [position + leg * 3] && !motionControl.legVerticalPosition [leg]) || (!chromosome.genes [position + leg * 3] && motionControl.legVerticalPosition [leg]))
					StartCoroutine (MoveLegVertical (leg + 1));
			}
			yield return new WaitForSeconds (time + 0.15f);
			for (int leg = 0; leg < 4; leg++) {
				if (!chromosome.genes [position + leg * 3 + 1]) {
					if (!chromosome.genes [position + leg * 3 + 2])
						StartCoroutine (MoveLegHorizontalNeutral (leg + 1));
					else
						StartCoroutine (MoveLegHorizontalPositive (leg + 1));
				} else if (chromosome.genes [position + leg * 3 + 1]) {
					if (!chromosome.genes [position + leg * 3 + 2])
						StartCoroutine (MoveLegHorizontalNegative (leg + 1));
				}
			}
			yield return new WaitForSeconds (time + 0.15f);
			position += 12;
		}
		yield return new WaitForSeconds (0.25f);
		Vector3 currentPosition = refferencedModule.transform.position;
		chromosome.fitnessValue = CalculateDistance(startPosition, currentPosition) / (int) (chromosome.genes.Length / 12);
		busy = false;
		yield return null;
	}

	public float CalculateDistance(Vector3 startPosition, Vector3 currentPosition) {
		float result;
		result = Mathf.Sqrt (Mathf.Pow ((currentPosition.x - startPosition.x), 2) + Mathf.Pow ((currentPosition.z - startPosition.z), 2));
		Debug.Log (string.Format ("Дистанция: {0}", result));
		return result;
	}

	// Не относится к обучению:

	public void StartInfiniteMotion(Chromosome chr) {
		StartCoroutine (InifiniteMotion (chr));
	}

	IEnumerator InifiniteMotion (Chromosome chr) {
		builder.StartRebuildInsectRobot ();
		while (builder.insectIsBusy)
			yield return new WaitForEndOfFrame ();
		while (true) {
			int position = 0;
			while (position < chr.genes.Length) {
				for (int leg = 0; leg < 4; leg++) {
					if ((chr.genes [position + leg * 3] && !motionControl.legVerticalPosition [leg]) || (!chr.genes [position + leg * 3] && motionControl.legVerticalPosition [leg]))
						StartCoroutine (MoveLegVertical (leg + 1));
				}
				yield return new WaitForSeconds (time + 0.1f);
				for (int leg = 0; leg < 4; leg++) {
					if (!chr.genes [position + leg * 3 + 1]) {
						if (!chr.genes [position + leg * 3 + 2])
							StartCoroutine (MoveLegHorizontalNeutral (leg + 1));
						else
							StartCoroutine (MoveLegHorizontalPositive (leg + 1));
					} else if (chr.genes [position + leg * 3 + 1]) {
						if (!chr.genes [position + leg * 3 + 2])
							StartCoroutine (MoveLegHorizontalNegative (leg + 1));
					}
				}
				yield return new WaitForSeconds (time + 0.1f);
				position += 12;
			}
		}
	}*/
}
