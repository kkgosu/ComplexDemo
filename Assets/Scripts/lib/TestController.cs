using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;

// Для Егора:

public class TestController : MonoBehaviour {

	private GameObject prefab;
	private LearningController learning;

	Module basem0;

	void Start () {
		Application.targetFrameRate = 60;

		bool[] chrb = new bool[] {
			true, true, true, true, true, true, true, true, 		// Вернет 180.
			false, false, false, false, false, false, false, false,	// Вернет 0.
			true, false, true, false, false, false, false, false,	// Вернет ~112 (160 * 0.7 - дискрету).
			false, false, false, false, false, false, true, true,	// Вернет ~2.1 (3 * 0.7).
		};

        foreach (float angle in GetAnglesFromBoolChromosome(chrb))
        {
            Debug.LogWarning(angle);
        }
	}


	/// <summary>
	/// Получить значения углов из бинарной хромосомы типа bool.
	/// </summary>
	/// <returns>Массив углов типа float.</returns>
	/// <param name="chr">Массив типа bool.</param>
	/// <param name="disc">Количество бит, описывающих один угол.
	/// Длина хромосомы должна быть кратна этому числу.</param>
	private float[] GetAnglesFromBoolChromosome (bool [] chr, int disc = 8) {
		float discrete = 180f / (Mathf.Pow (2, disc) - 1);
		int count = 0;
		List <float> angles = new List <float> ();
		string binNumber = "";
		foreach (bool gene in chr) {
			if (count > 0 && count % disc == 0) {
				angles.Add ((float) Convert.ToInt32(binNumber, 2) * discrete);
				binNumber = "";
			}
			if (gene)
				binNumber += "1";
			else
				binNumber += "0";
			count++;
		}
		angles.Add (Convert.ToInt32(binNumber, 2) * discrete);
		return angles.ToArray ();
	}

	/// <summary>
	/// Получить значения углов из бинарной хромосомы типа int. Убивал бы за такое.
	/// </summary>
	/// <returns>Массив углов типа float.</returns>
	/// <param name="chr">Массив типа int.</param>
	/// <param name="disc">Количество бит, описывающих один угол.
	/// Длина хромосомы должна быть кратна этому числу.</param>
	private float[] GetAnglesFromIntChromosome (int [] chr, int disc = 8) {
		float discrete = 180f / (Mathf.Pow (2, disc) - 1);
		int count = 0;
		List <float> angles = new List <float> ();
		string binNumber = "";
		foreach (int gene in chr) {
			if (count > 0 && count % disc == 0) {
				angles.Add ((float) Convert.ToInt32(binNumber, 2) * discrete);
				binNumber = "";
			}
			if (gene == 0)
				binNumber += "0";
			else
				binNumber += "1";
			count++;
		}
		angles.Add (Convert.ToInt32(binNumber, 2) * discrete);
		return angles.ToArray ();
	}
		
	// Update is called once per frame
	void Update () {

	}
}
