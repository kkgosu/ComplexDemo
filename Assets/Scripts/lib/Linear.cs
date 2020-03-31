using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс модульных роботов с линейной конфигурацией: гусеница/змея, колесо, манипулятор (??конечность шагающего робота??)
/// 
/// </summary>
public class Linear : ModularRobot {
    /*
	public void Create(int numberOfModules, string modulesTypeName, bool alternation = false, Vector3 position = default(Vector3)){
		if (numberOfModules > 2) {
			modules =  new Dictionary<int, Module>();
			referModuleId = 1;
			modules.Add (referModuleId, Modules.Create (Modules.GetModuleByName (modulesTypeName), position, Quaternion.Euler (new Vector3(0,0,90)), parent: transform, ID: referModuleId));
			for(int i = 2; i <= numberOfModules; i++)
			{
				float tilt = 0;
				if (alternation) {
					tilt = 90;
				}
				modules.Add (i,
                    (modules [i-1] as M3RL).surfaces["top"].Add (
					Modules.GetModuleByName (modulesTypeName),
					Modules.GetModuleByName (modulesTypeName).surfaces["bottom"],
					parent: transform,
						tilt: tilt,
						ID: i));
			}
			referDistanceModuleId = (numberOfModules+1)/2;
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	*/
}
