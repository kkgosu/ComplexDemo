using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Collections.Specialized; 	
using UnityEngine;
using System.IO;

/* public class Myhandler : MonoBehaviour {
	static int parallelRobots = 32;
	Stack<GameObject> Robots = new Stack<GameObject> ();
	Vector3 [] positions = new Vector3[parallelRobots];
	bool[] positionsState = new bool[parallelRobots];

	int numberOfCalls = 0, numberOfRobots =0;

	IEnumerator Callback(HttpListenerContext handlerContext)
	{
		int i = 0;
		GameObject r;
		ModularRobot MR;
		//try{
			
			numberOfCalls++;

			for (int j = 0; j < positionsState.Length; j++)
				if (positionsState [j] == true) {
					positionsState [j] = false;
					i = j;
					break;
				}
			r = new GameObject ();
			r.transform.position= Vector3.zero;
			MR = r.AddComponent<ModularRobot> ();
			string configuration = handlerContext.Request.QueryString ["robot"].TrimStart();
			int numberOfSteps = int.Parse(handlerContext.Request.QueryString ["numberofsteps"]);
			if(configuration.StartsWith("<"))
				MR.LoadXML(configuration, positions[i]);
			else 
				MR.Load (Application.dataPath + "/Resources/Configurations/" + configuration + ".xml", positions[i]);
		
			foreach (string s in handlerContext.Request.QueryString) {
				if (s.StartsWith ("step"))
					MR.ControlTable.AddLine (handlerContext.Request.QueryString [s]);
				}
				MR.ControlTable.isKeyboardControl=false;
				MR.ControlTable.StartMotion (numberOfSteps);
				numberOfRobots++;
		/*	}
			catch {
			Debug.Log("AABB problem");
				handlerContext.Response.Headers.Add("DistanseX", "0");
				SendResponse (handlerContext.Response, "Error");
				yield break;
			} /

			//MR.ControlTable.
			//MR.ControlTable.AddLine ("-45, 45, -45, 45, -45, 45, -45, 45");


			while (MR.ControlTable.isMotion) {
					yield return new WaitForEndOfFrame ();
				}
			handlerContext.Response.StatusCode = 200;
			handlerContext.Response.Headers.Add("DistanseX", MR.x.ToString());
			handlerContext.Response.Headers.Add("DistanseY", MR.y.ToString());
			handlerContext.Response.Headers.Add("DistanseZ", MR.z.ToString());
			string resp = "X: " + MR.x.ToString ()+ "\n" +"Y: " + MR.y.ToString () + "\n"+"Z: " + MR.z.ToString ();
			Destroy (r);
			yield return new WaitForSeconds (1);
			positionsState [i] = true;
			Debug.Log ("Destroyed");
			SendResponse (handlerContext.Response, resp);
		

		 
			//handlerContext.Response.Headers.Add("isBusy", "true");
			//handlerContext.Response.StatusCode = 404;
			//SendResponse (handlerContext.Response, "isBusy");
	}

	private void SendResponse(HttpListenerResponse response, string responseString)
	{
		//response.SendChunked = false;
		byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
		response.ContentLength64 = buffer.Length;
		response.OutputStream.Write(buffer,0,buffer.Length);
		response.OutputStream.Close();
	}
	// Use this for initialization
	void Start () {
		

		for (int i = 0; i < parallelRobots; i++){
			int j = 1;
			if (i % 2 == 0)
				j = -1;
			positions [i] = new Vector3 (j * 10, 0, i * 10);
			positionsState [i] = true;
		}
		//Debug.Log (positions [10]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/