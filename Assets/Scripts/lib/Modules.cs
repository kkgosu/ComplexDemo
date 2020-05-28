using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Modules {
	public static Module M3R = GetModuleByName("M3R");
    public static Module M3R2 = GetModuleByName("M3R2");
    public static Module M3RCAM = GetModuleByName("M3RCAM");

    private static Module GetModuleFromPrefab (string path) {
		GameObject prefab = (GameObject) Resources.Load (path);
		Module module = prefab.GetComponent <Module> ();
		//module.transform.localScale = Vector3.one * 3f;
		if (module == null) {
			Debug.LogError (string.Format ("Selected prefab ({0}) doesn't contain Module component.", path));
			return null;
		}
		module.Prepare ();
		return module;
	}

	public static Module Create (Module moduleType, Vector3 position, 
                                 Quaternion rotation, float[] angles = null, Transform anchor = null, 
                                 Transform parent = null, int ID = -1) {
		GameObject moduleObject = MonoBehaviour.Instantiate (moduleType.gameObject, position, rotation);
		Module module = moduleObject.GetComponent <Module> ();
		module.Prepare ();
		module.isPhysical = false;
		module.id = ID;
		module.moduleObject = moduleObject;
		module.x -= position.x;
		module.y -= position.y;
		module.z -= position.z;

		int c = 0;
		if (angles != null) {
			// НЕПРАВИЛЬНЫЙ ИТТЕРАТОР:
            // Поправить вид коллекции для двигателей внутри модулей.
			foreach (Driver driver in module.GetComponents (typeof(Driver))) {
                //foreach (Driver driver in module.drivers) {
                // Добавить проверку того, существует ли angles [c].
                if (c < angles.Length)
			        driver.RotateKinematic (anchor, angles [c]);
				c++;
			}
		}

		if (parent != null)
			module.transform.parent = parent;
		module.transform.name = (ID == -1) ? "Module" : "Module " + ID.ToString();
		module.isPhysical = true;

		return module;
	}
	
	public static Module GetModuleByName (string moduleName) {
		switch (moduleName.ToUpper()) 
		{
			case "M3R":
				return GetModuleFromPrefab("Prefabs/M3R");
            case "M3R2":
                return GetModuleFromPrefab("Prefabs/M3R2");
            case "M3RCAM":
                return GetModuleFromPrefab("Prefabs/M3RCAM");
            default:
				return null;
		}
	}
}