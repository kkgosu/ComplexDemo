using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Robot : MonoBehaviour
{

}

public class RobotInsectoid : Robot {

    public Module[,] modules = new Module[5, 4];

	public Vector3 fromPosition, position;

	public int id = 0;

	public DCT dct;

	public float totalDistance = 0;

	public void Create (Vector3 pos) {
        modules [1, 0] = Modules.Create (Modules.M3R2, /*new Vector3 (0, -1.3025f, 0) + */pos, Quaternion.Euler(Vector3.zero), new float[] {0}, parent: transform);
        modules [1, 1] = modules [1, 0].surfaces["top"].Add (Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] {45}, parent: transform);
		modules [1, 2] = modules [1, 1].surfaces["top"].Add (Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] {45}, parent: transform);
		modules [1, 3] = modules [1, 2].surfaces["top"].Add (Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] {0}, tilt: -90, parent: transform); 
		modules [0, 0] = modules [1, 3].surfaces["top"].Add (Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] {0}, tilt: -90, parent: transform);
		modules [2, 3] = modules [0, 0].surfaces["right"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {0}, tilt: -90, parent: transform);
		modules [2, 2] = modules [2, 3].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {45}, tilt: -90, parent: transform);
		modules [2, 1] = modules [2, 2].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {45}, parent: transform);
		modules [2, 0] = modules [2, 1].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {0}, parent: transform);
		modules [3, 3] = modules [0, 0].surfaces["top"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {0}, tilt: 90, parent: transform);
		modules [3, 2] = modules [3, 3].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {45}, tilt: 90, parent: transform);
		modules [3, 1] = modules [3, 2].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {45}, parent: transform);
		modules [3, 0] = modules [3, 1].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {0}, parent: transform);
		modules [4, 3] = modules [0, 0].surfaces["left"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {0}, tilt: 90, parent: transform);
		modules [4, 2] = modules [4, 3].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {45}, tilt: 90, parent: transform);
		modules [4, 1] = modules [4, 2].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {45}, parent: transform);
		modules [4, 0] = modules [4, 1].surfaces["bottom"].Add (Modules.M3R2, Modules.M3R2.surfaces["top"], new float[] {0}, parent: transform);

		dct.SetHeader (new Driver[] {
			modules [1, 3].drivers["q1"], modules [1, 2].drivers["q1"], modules [1, 1].drivers["q1"],
			modules [2, 3].drivers["q1"], modules [2, 2].drivers["q1"], modules [2, 1].drivers["q1"],
			modules [3, 3].drivers["q1"], modules [3, 2].drivers["q1"], modules [3, 1].drivers["q1"],
			modules [4, 3].drivers["q1"], modules [4, 2].drivers["q1"], modules [4, 1].drivers["q1"]
		});

		fromPosition = modules [0, 0].surfaces["bottom"].anchor.transform.position;
		position = modules [0, 0].surfaces["bottom"].anchor.transform.position;
	}

	void Awake () {
		dct = gameObject.AddComponent <DCT> ();
	}

	void FixedUpdate () {
		if (dct.inProgress) {
			totalDistance += Vector3.Distance (position, modules [0, 0].surfaces["bottom"].anchor.transform.position);
			position = modules [0, 0].surfaces["bottom"].anchor.transform.position;
		}
	}
}