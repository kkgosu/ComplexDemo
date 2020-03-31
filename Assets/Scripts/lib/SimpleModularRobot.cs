using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleModularRobot : ModularRobot {
    /*
	public enum RobotTypes{
		Linear = 1,
		Wheel,
		Legged
	}

	public void Create(int numberOfModules, string modulesTypeName, bool alternation = false, Vector3 position = default(Vector3)){
		if (numberOfModules > 2) {
			modules =  new Dictionary<int, Module>();
			referModuleId = 1;
			modules.Add (referModuleId, Modules.Create (Modules.GetModuleByName (modulesTypeName), position, Quaternion.Euler (new Vector3(0,0,90)), parent: transform, ID: referModuleId));
			for(int i=2; i<=numberOfModules; i++)
			{
				float tilt = 0;
				if (alternation) {
					tilt = 90;
				}
				modules.Add (i,
					modules [i-1].surfaces["top"].Add (
						Modules.GetModuleByName (modulesTypeName),
						Modules.GetModuleByName (modulesTypeName).surfaces["bottom"],
						parent: transform,
						tilt: tilt,
						ID: i));
			}
			referDistanceModuleId = (numberOfModules+1)/2;
		}
	}

	public void Reconfugurate(RobotTypes type){
		switch (type) {
		case RobotTypes.Linear:
			break;
		case RobotTypes.Wheel:
			float angle = -1 * (modules.Count - 2) * 180 / modules.Count + 90; // формула суммы углов правильного многоугольника
			Debug.Log (angle.ToString ());
			foreach (Module m in modules.Values) {
				m.drivers["q1"].speed = 20;
				m.drivers["q1"].Rotate (angle);
			}
			//StartCoroutine (Wait(100));

			break;

		case RobotTypes.Legged:
			break;
		}
	}
	public void Connect(int id, int jd)
	{
		modules [id].surfaces["bottom"].Connect (modules [jd].surfaces["top"]);
	}

	IEnumerator Wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	*/
}
