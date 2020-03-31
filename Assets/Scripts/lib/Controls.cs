using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Driver control table.
/// </summary>
public class DriverControlTable : MonoBehaviour {

	public List <Driver> header;

	public int step = 0;

	public bool inProgress = false;

	public class DCT_Data {
		public List<DriverState> values;

		public override string ToString () {
			return null;
		}

		public string[] ToStringArray () {
			return null;
		}

		public void AddLine (float[] line, int[] drivers = null) {
			values.Add (new DriverState (line, drivers));
		}

		public void AddLineAtIndex (int index, float[] line, int[] drivers = null) {
			values.Insert (index, new DriverState(line, drivers));
		}

		public float[] GetLine (int index) {
			return values [index].angles;
		}

		public void RemoveLine (int index) {
			values.RemoveAt (index);
		}

		public void Clear () {
			values.Clear ();
		}

		public DCT_Data () {
			values = new List<DriverState>();
		}
	}

	public class DriverState {
		public float[] angles;
		public int[] drvs;

		public DriverState (float [] a, int [] d) {
			angles = a;
			drvs = d;
		}
	}

	public DCT_Data data;

	public void SetHeader (Driver[] drvs) {
		header = new List <Driver> (drvs);
		data = new DCT_Data ();
	}

	/// <summary>
	/// Begin this instance. Пока не придет команда остановиться.
	/// </summary>
	public void Begin () {
		if (DriversAreReady()) {
			if (!inProgress) {
				inProgress = true;
				StartCoroutine (WorkerWithWaiting ());
			} else
				Debug.LogError ("Cannot start DCT while it's active.");
		} else
			Debug.LogError ("Cannot start DCT. Drivers are not ready.");
	}

	/// <summary>
	/// Воспроизвести один раз, пока не закончится таблица.
	/// </summary>
	public void BeginTillEnd () {
		if (DriversAreReady()) {
			if (!inProgress) {
				inProgress = true;
				StartCoroutine (Worker ());
			} else
				Debug.LogError ("Cannot start DCT while it's active.");
		} else
			Debug.LogError ("Cannot start DCT. Drivers are not ready.");
	}

	/// <summary>
	/// Непрерывное воспроизведение определенного количества раз. Если параметр не указан, пока DCT не будет остановлен.
	/// </summary>
	/// <param name="times">Количество повторений.</param>
	public void BeginLoop (float times = Mathf.Infinity) {
		if (DriversAreReady()) {
			if (!inProgress) {
				inProgress = true;
				StartCoroutine (WorkerLoop (times));
			} else
				Debug.LogError ("Cannot start DCT while it's active.");
		} else
			Debug.LogError ("Cannot start DCT. Drivers are not ready.");
	}

	IEnumerator WorkerLoop (float times) {
		int count = 0;
		while (count < times) {
			inProgress = true;
			StartCoroutine (Worker ());
			do {
				yield return new WaitForEndOfFrame ();
			} while (inProgress);
			count++;
		}
	}

	IEnumerator WorkerWithWaiting () {
		int count;
		while (true) {
			while (inProgress) {
				if (step < data.values.Count && DriversAreReady ()) {
					count = 0;
					if (data.values[step].drvs == null) {
						foreach (Driver drv in header) {
							drv.Set (data.values [step].angles [count]);
							count++;
						}
					} else {
						foreach (int drvIndex in data.values[step].drvs) {
							header[drvIndex].Set (data.values [step].angles [count]);
							count++;
						}
					}
					step++;
				} else
					yield return new WaitForEndOfFrame ();
			}
			step = 0;
		}
	}

	IEnumerator Worker () {
		int count;
		while (step < data.values.Count && inProgress) {
			count = 0;
			if (data.values [step].drvs == null) {
				foreach (Driver drv in header) {
					drv.Set (data.values [step].angles [count]);
					count++;
				}
			} else {
				foreach (int drvIndex in data.values[step].drvs) {
					header[drvIndex].Set (data.values[step].angles[count]);
					count++;
				}
			}
			do {
				yield return new WaitForEndOfFrame ();
			} while (!DriversAreReady());
			step++;
		}
		step = 0;
		inProgress = false;
	}

	public bool DriversAreReady () {
		foreach (Driver drv in header) {
			if (drv.busy)
				return false;
		}
		return true;
	}

	public void Pause () {
		inProgress = false;
	}

	public void ForcePause () {
		StopAllCoroutines ();
		inProgress = false;
	}

	public void Stop () {
		inProgress = false;
		step = 0;
	}

	public void ForceStop () {
		StopAllCoroutines ();
		inProgress = false;
		step = 0;
	}

	public void Reset () {
		ForceStop ();
		header.Clear ();
		data.Clear ();
	}

	public void Destroy () {
		Destroy (this);
	}
}

public class DCT : DriverControlTable {

}