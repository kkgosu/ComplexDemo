using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMController : MonoBehaviour {

	public LearningController learningController;

	public int robotsNumber = 4;

	private Translation[,] states;

	Utilities.TextWriter [] data;

	System.Random random = new System.Random();

	List <RobotInsectoid> robots = new List <RobotInsectoid> ();

	private float bestFitness = 0.01f, averageFitness = 0.01f, summedFitness = 0; // Все, что ниже среднего, теряет вероятность. Все, что выше - получает в проценте от bestFitness.

	private int itterations = 0;

	public class Translation {
		public double chance;
		public float realDistance, totalDistance;
		public Vector3 fromPosition, toPosition;
		public Vector2 position;

		public Translation (double ch, int x, int y) {
			chance = ch;
			position = new Vector2 (x, y);
		}
	}

	void Start () {
		double initialChance = 1d / 256d;
		Debug.LogError ("Chance is " + initialChance);
		states = new Translation[256, 256];
		for (int j = 0; j < 256; j++) {
			for (int k = 0; k < 256; k++) {
				states [j, k] = new Translation (initialChance, j, k);
			}
		}

		data = new Utilities.TextWriter[robotsNumber];

		BuildRobots ();

		/* for (int i = 0; i < robotsNumber; i++) {
			StartCoroutine (RobotWorker (robots[i], random.Next (0, 256), random.Next (0, 256)));
		} */
	}

	int[] DecodeStateByID (int id) {
		int[] legState = new int[4];
		legState [0] = (int) id / 64;
		legState [1] = (int) (id % 64) / 16;
		legState [2] = (int) (id % 16) / 4;
		legState [3] = (int) id % 4;
		return legState;
	}

	void RecalculateChances (float fitnessValue, Translation t, bool horizontal) {
		double chanceDelta = 0;
		if (fitnessValue < averageFitness)
			chanceDelta = t.chance * (averageFitness / (averageFitness - fitnessValue)) * (-1);
		else
			chanceDelta = t.chance * (fitnessValue / bestFitness);
		t.chance += chanceDelta;
		chanceDelta /= 255;
		if (horizontal) {
			for (int i = 0; i < 256; i++) {
				if (i == t.position.x)
					continue;
				states [i, (int) t.position.y].chance -= chanceDelta;
			}
		} else {
			for (int i = 0; i < 256; i++) {
				if (i == t.position.y)
					continue;
				states [(int) t.position.x, i].chance -= chanceDelta;
			}
		}
		if (fitnessValue > bestFitness)
			bestFitness = fitnessValue;
		summedFitness += fitnessValue;
		averageFitness = summedFitness / itterations;
	}

	Translation GetNextTranslation (int id, bool horizontal) {
		double randomValue = random.NextDouble ();
		Translation t;
		for (int i = 0; i < 256; i++) {
			t = states [horizontal ? id : i, horizontal ? i : id];
			if (randomValue <= t.chance)
				return t;
			else
				randomValue -= t.chance;
		}
		return null;
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
		robotParent.transform.name = string.Format("(FSM) Robot {0}", id);
		RobotInsectoid robot = robotParent.AddComponent <RobotInsectoid> ();
		robot.Create (new Vector3 ((-10 + id * 5), 0, (-10 + id * 5)));
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

	int GetTailLength (List<Translation> tail, int index) {
		return tail.Count - index;
	}

	float GetFitnessForTail (List<Translation> tail, int index) {
		Vector3 fromPosition = tail [index].fromPosition;
		Vector3 toPosition = tail [tail.Count - 1].toPosition;
		float totalDist = 0;
		for (int i = index; i < tail.Count; i++) {
			totalDist += tail [i].totalDistance;
		}
		return learningController.CalculateFitness (fromPosition, toPosition, totalDist, GetTailLength (tail, index));
	}
	
	IEnumerator RobotWorker (RobotInsectoid robot, int stateX, int stateY) {
		List<Translation> tail = new List <Translation> ();
		bool horizontal = false, reported = false;
		int repeatIndex = 0;
		float fitnessValue = 0;
		Translation previousTranslation, currentTranslation;
		data [robot.id] = new Utilities.TextWriter ("СКА " + robot.id);
		currentTranslation = states [stateX, stateY];
		while (true) {
			itterations++;
			reported = false;
			previousTranslation = currentTranslation;
			currentTranslation = GetNextTranslation ((horizontal ? (int) currentTranslation.position.x : (int) currentTranslation.position.y), horizontal);
			learningController.FormControlLine (robot.dct, DecodeStateByID(horizontal ? (int) currentTranslation.position.y : (int) currentTranslation.position.x));
			currentTranslation.fromPosition = robot.position;
			if (!robot.dct.inProgress)
				robot.dct.Begin ();
			yield return new WaitForEndOfFrame ();
			while (!robot.dct.DriversAreReady())
				yield return new WaitForEndOfFrame ();
			currentTranslation.toPosition = robot.position;
			if (currentTranslation.position.x == previousTranslation.position.y || currentTranslation.position.y == previousTranslation.position.x)
				fitnessValue = 0;
			else
				fitnessValue = learningController.CalculateFitness(currentTranslation.fromPosition, currentTranslation.toPosition,
				Vector3.Distance(currentTranslation.fromPosition, currentTranslation.toPosition), 1);
			RecalculateChances (fitnessValue, currentTranslation, horizontal);
			if (tail.Contains (currentTranslation)) {
				reported = true;
				repeatIndex = tail.LastIndexOf (currentTranslation);
			}
			tail.Add (currentTranslation);
			horizontal = !horizontal;
			data [robot.id].AddLine (string.Format ("State: {0}:{1}, Previous state: {2}:{3}",
				currentTranslation.position.x, currentTranslation.position.y,
				previousTranslation.position.x, previousTranslation.position.y));
			if (reported)
				data [robot.id].AddLine (string.Format ("!!! REPEAT !!! Tail Length is {0}, real distance is {1:F4}," +
					"total distance is {2:F4}, total fitness is {3:F4} !!! On itteration {4} !!!",
					GetTailLength(tail, repeatIndex), currentTranslation.realDistance,
					currentTranslation.totalDistance, GetFitnessForTail(tail, repeatIndex), itterations));
		}
	}
}
