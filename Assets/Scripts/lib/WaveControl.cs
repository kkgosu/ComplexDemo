using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveControl : MonoBehaviour
{
	public int TotalWaves = 0;
	public List<float[]> Sequences = new List<float[]>();
	public List<Driver> modules = new List<Driver> ();

	int FirstModule = 1;
	int LastModule;
	List<int> MainModuls = new List<int> ();
	int StepOffset = 1;
	int step = 0;
	bool waveConstantly = false;
	bool waveInProgress = false;
	bool IsReversed = false;
	bool emergencyStop = false;
	public bool ready;
	int figureLength;

	public bool DriversAreReady () {
		foreach (Driver drv in modules) {
			if (drv.busy)
				return false;
		}
		return true;
	}

	public void AddModulesFromRobot (ModularRobot robot){
		foreach (Module m in robot.modules.Values) {
			modules.Add (m.drivers["q1"]);
		}
	}

	void Begin(){
		if (MainModuls.Count != 0 && waveInProgress) {
			if (DriversAreReady ()) {
				if ((MainModuls [0] >= FirstModule + figureLength) && (IsReversed == false) && (waveConstantly == true)) {
					MainModuls.Insert (0, FirstModule);
					TotalWaves++;
				}
				if ((MainModuls [0] <= FirstModule - figureLength) && (IsReversed == true) && (waveConstantly == true)) {
					MainModuls.Insert (0, FirstModule);
					TotalWaves++;
				}
				if (((MainModuls [MainModuls.Count - 1] > LastModule) && (!IsReversed)) || (MainModuls [MainModuls.Count - 1] < LastModule && (IsReversed))) {
					for (int i = 0; i < Sequences [step].Length; i++) {
						if ((MainModuls [MainModuls.Count - 1] + i * (IsReversed ? -1 : 1) >= 0) && (MainModuls [MainModuls.Count - 1] + i * (IsReversed ? -1 : 1) < modules.Count)) {
							modules [MainModuls [MainModuls.Count - 1] + i * (IsReversed ? -1 : 1)].Set (0);
						}
					}
					MainModuls.RemoveAt (MainModuls.Count - 1);
				}
				for (int ID = 0; ID < MainModuls.Count; ID++) {
					for (int i = 0; i < Sequences [step].Length; i++) {
						if ((MainModuls [ID] + i * (IsReversed ? -1 : 1) >= 0) && (MainModuls [ID] + i * (IsReversed ? -1 : 1) < modules.Count)) {
							modules [MainModuls [ID] + i * (IsReversed ? -1 : 1)].Set (Sequences [step] [i]);
						}
					}
				}
				step++;
				if (step == Sequences.Count) {
					step = 0;
					for (int ID = 0; ID < MainModuls.Count; ID++) {
						MainModuls [ID] += StepOffset * (IsReversed ? -1 : 1);
					}
				}
			}
		} else {
			waveInProgress = false;
		}
	}

	public void StartMo (bool isReversed){
		if (isReversed) {
			LastModule = 0;
			FirstModule = modules.Count;
		} else {
			LastModule = modules.Count;
			FirstModule = 0;
		}
		StepOffset = 1;
		IsReversed = isReversed;
		MainModuls.Insert (0, FirstModule);
		step = 0;
		figureLength = Sequences [0].Length;
		waveInProgress = true;
		waveConstantly = true;
	}
	public void StartMo (bool isReversed, int stepOffSet){
		if (isReversed) {
			LastModule = 0;
			FirstModule = modules.Count;
		} else {
			LastModule = modules.Count;
			FirstModule = 0;
		}
		StepOffset = stepOffSet;
		IsReversed = isReversed;
		MainModuls.Insert (0, FirstModule);
		step = 0;
		figureLength = Sequences [0].Length;
		waveInProgress = true;
		waveConstantly = true;
	}
	public void StartMo (bool isReversed, int stepOffSet, int firstModule){
		if (isReversed) {
			LastModule = 0;
		} else {
			LastModule = modules.Count;
		}
		FirstModule = firstModule;
		StepOffset = stepOffSet;
		IsReversed = isReversed;
		MainModuls.Insert (0, FirstModule);
		step = 0;
		figureLength = Sequences [0].Length;
		waveInProgress = true;
		waveConstantly = true;
	}
	public void StartMo (bool isReversed, int stepOffSet, int firstModule, int lastModule){
		LastModule = lastModule;
		FirstModule = firstModule;
		StepOffset = stepOffSet;
		IsReversed = isReversed;
		MainModuls.Insert (0, FirstModule);
		step = 0;
		figureLength = Sequences [0].Length;
		waveInProgress = true;
		waveConstantly = true;
	}

	public void Stop(){
		waveConstantly = false;
	}

	public void EmergencyStop(){
		waveInProgress = false;
		waveConstantly = false;
		emergencyStop = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Begin ();
		if (emergencyStop) {
			if (DriversAreReady ()) {
				for (int i = 0; i < modules.Count; i++) {
					//modules [i].Set (0);
				}
				emergencyStop = false;
			}
		}
		if (waveInProgress) {
			ready = false;
		} else {
			ready = true;
		}
	}
}