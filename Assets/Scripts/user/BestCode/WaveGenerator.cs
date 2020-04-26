using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveGenerator : MonoBehaviour
{
	public int TotalWaves = 0;

    public List<Driver>  modules = new List<Driver>();
    public List<int> MainModules = new List<int>();
    public List<float[]> Sequences = new List<float[]>();

	int FirstModule = 1;
	int LastModule;
	int StepOffset = 1;
	int step = 0;
	bool waveConstantly = false;
	bool waveInProgress = false;
	bool IsReversed = false;
	bool emergencyStop = false;
	public bool ready;
	int figureLength;
    public bool busy = false;

    public bool DriversAreReady () {
		foreach (Driver drv in modules) {
			if (drv.busy)
				return false;
		}
		return true;
	}

    private void Start()
    {
        AddModulesFromRobot();
    }

    public void AddModulesFromRobot (){
        foreach (Module m in GetComponent<ModularRobot>().modules.Values) {
            modules.Add (m.drivers["q1"]);
		}
	}

	void Go(){
        if (MainModules.Count != 0 && waveInProgress) {
            if (DriversAreReady ()) {
                busy = true;
				if ((MainModules [0] >= FirstModule + figureLength) && (IsReversed == false) && (waveConstantly == true)) {
					MainModules.Insert (0, FirstModule);
					TotalWaves++;
				}
				if ((MainModules [0] <= FirstModule - figureLength) && (IsReversed == true) && (waveConstantly == true)) {
					MainModules.Insert (0, FirstModule);
					TotalWaves++;
				}
				if (((MainModules [MainModules.Count - 1] > LastModule) && (!IsReversed)) || (MainModules [MainModules.Count - 1] < LastModule && (IsReversed))) {
					for (int i = 0; i < Sequences [step].Length; i++) {
						if ((MainModules [MainModules.Count - 1] + i * (IsReversed ? -1 : 1) >= 0) && (MainModules [MainModules.Count - 1] + i * (IsReversed ? -1 : 1) < modules.Count)) {
							modules [MainModules [MainModules.Count - 1] + i * (IsReversed ? -1 : 1)].Set (0);
						}
					}
					MainModules.RemoveAt (MainModules.Count - 1);
				}
				for (int ID = 0; ID < MainModules.Count; ID++) {
					for (int i = 0; i < Sequences [step].Length; i++) {
						if ((MainModules [ID] + i * (IsReversed ? -1 : 1) >= 0) && (MainModules [ID] + i * (IsReversed ? -1 : 1) < modules.Count)) {
							modules [MainModules [ID] + i * (IsReversed ? -1 : 1)].Set (Sequences [step] [i]);
						}
					}
				}
				step++;
				if (step == Sequences.Count) {
					step = 0;
					for (int ID = 0; ID < MainModules.Count; ID++) {
						MainModules [ID] += StepOffset * (IsReversed ? -1 : 1);
					}
				}
			}
		} else {
			waveInProgress = false;
            if (DriversAreReady())
            {
                busy = false;
            }
		}
	}

	public void StartMo (bool isReversed = false, int stepOffSet = 2){
		if (isReversed) {
			LastModule = 0;
			FirstModule = modules.Count;
		} else {
			LastModule = modules.Count;
			FirstModule = 0;
		}
		StepOffset = stepOffSet;
		IsReversed = isReversed;
		MainModules.Insert (0, FirstModule);
		step = 0;
		figureLength = Sequences [0].Length;
		waveInProgress = true;
		waveConstantly = true;
        TotalWaves = 1;
    }
	public void StartMo (int firstModule, bool isReversed, int stepOffSet){
		if (isReversed) {
			LastModule = 0;
		} else {
			LastModule = modules.Count;
		}
		FirstModule = firstModule;
		StepOffset = stepOffSet;
		IsReversed = isReversed;
		MainModules.Insert (0, FirstModule);
		step = 0;
		figureLength = Sequences [0].Length;
		waveInProgress = true;
		waveConstantly = true;
        TotalWaves = 1;
    }
	public void StartMo (int firstModule, int lastModule, bool isReversed = false, int stepOffSet = 2){
		LastModule = lastModule;
		FirstModule = firstModule;
		StepOffset = stepOffSet;
		IsReversed = isReversed;
		MainModules.Insert (0, FirstModule);
		step = 0;
		figureLength = Sequences [0].Length;
		waveInProgress = true;
		waveConstantly = true;
        TotalWaves = 1;
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
		Go ();
		if (emergencyStop) {
			if (DriversAreReady ()) {
				for (int i = 0; i < modules.Count; i++) {
					modules [i].Set (0);
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