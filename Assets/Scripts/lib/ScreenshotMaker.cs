using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenshotMaker : MonoBehaviour {

	public string seriesName = "манипулятор";
	public int resWidth = 2400; 
	public int resHeight = 900;

	public Camera myCamera;
	private string pathToFolder = "Screenshots";

	[HideInInspector]
	public bool takeHiResShot = false;

	private string GetName (string name) {
		string fileName = "", optional = "";
		int counter = 0;
		while (true) {
			fileName = Path.Combine(Application.dataPath, string.Format("{0}/{1}{2}.png",
				pathToFolder,
				name,
				optional));
			if (File.Exists (fileName)) {
				counter++;
				optional = " (" + counter + ")";
			} else
				break;
		}
		return fileName;
	}

	public void MakeScreenshot() {
		takeHiResShot = true;
		RenderTexture rt = new RenderTexture (resWidth, resHeight, 24);
		myCamera.targetTexture = rt;
		Texture2D screenShot = new Texture2D (resWidth, resHeight, TextureFormat.RGB24, false);
		myCamera.Render ();
		RenderTexture.active = rt;
		screenShot.ReadPixels (new Rect (0, 0, resWidth, resHeight), 0, 0);
		myCamera.targetTexture = null;
		RenderTexture.active = null;
		Destroy (rt);
		byte[] bytes = screenShot.EncodeToPNG ();
		string filename = GetName(seriesName);
		System.IO.File.WriteAllBytes (filename, bytes);
		Debug.LogWarning (string.Format ("(Screenshot Maker) Took screenshot to: {0}", filename));
		takeHiResShot = false;
	}
	void Start () {
		System.IO.Directory.CreateDirectory (Path.Combine(Application.dataPath, pathToFolder));
		myCamera = GetComponentInChildren <Camera> ();
	}
		
	void Update () {
		if (Input.GetKeyDown(KeyCode.S)) {
			MakeScreenshot ();
		}
	}
}
