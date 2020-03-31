using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Utilities : MonoBehaviour {

	/// <summary>
	/// Класс для записи CSV файлов. Один объект - один файл.
	/// </summary>
	public class CSVWriter {
		StreamWriter sw;
		string _filePath;
		public string filePath {
			get {
				return _filePath;
			}
		}
			
		public CSVWriter (string name, string columns) {
			_filePath = GetName(name);
			sw = new StreamWriter (_filePath, true, System.Text.Encoding.GetEncoding(65001));
			sw.WriteLine (columns);
			sw.Close ();
		}

		// Путь:
		public CSVWriter (string name, string columns, string path) {
			_filePath = GetName(name);
			sw = new StreamWriter (_filePath, true, System.Text.Encoding.GetEncoding(65001));
			sw.WriteLine (columns);
			sw.Close ();
		}

		public void AddLine (string data) {
			sw = new StreamWriter (_filePath, true, System.Text.Encoding.GetEncoding(65001));
			sw.WriteLine (data);
			sw.Close ();
		}

		private string GetName (string name) {
			string fileName = "", optional = "";
			int counter = 0;
			while (true) {
				fileName = string.Format ("{0}/Data/Experiments/{1}{2}.csv", 
					Application.dataPath, 
					name,
					optional);
				if (File.Exists (fileName)) {
					counter++;
					optional = " (" + counter + ")";
				} else
					break;
			}
			return fileName;
		}
	}

	/// <summary>
	/// Класс для записи текстовых файлов.
	/// </summary>
	public class TextWriter {
		StreamWriter sw;
		string filePath;

		public TextWriter (string name) {
			filePath = GetName(name);
		}

		public void AddLine (string line) {
			sw = new StreamWriter (filePath, true, System.Text.Encoding.GetEncoding(65001));
			sw.WriteLine (line);
			sw.Close ();
		}

		private string GetName (string name) {
			string fileName = "", optional = "";
			int counter = 0;
			while (true) {
				fileName = string.Format ("{0}/Data/Experiments/{1}{2}.txt", 
					Application.dataPath, 
					name,
					optional);
				if (File.Exists (fileName)) {
					Debug.Log ("Trying to cast " + fileName);
					counter++;
					optional = " (" + counter + ")";
				} else
					break;
			}
			return fileName;
		}
	}

	/// <summary>
	/// Класс захвата скриншотов. Один объект - одна серия скриншотов.
	/// </summary>
	public class ScreenShotCapture {
		Camera camera;

		public int resolutionWidth = 2400;
		public int resolutionHeight = 900;

		public string path {
			get {
				return path;
			}
		}

		public ScreenShotCapture (string name) {

		}

		public ScreenShotCapture (string name, Camera camera) {

		}

		public ScreenShotCapture (string name, int resolutionWidth, int resolutionHeight) {

		}

		public ScreenShotCapture (string name, int resolutionWidth, int resolutionHeight, Camera camera) {

		}

		public ScreenShotCapture (string name, string path) {

		}

		public ScreenShotCapture (string name, string path, Camera camera) {

		}

		public ScreenShotCapture (string name, string path, int resolutionWidth, int resolutionHeight) {

		}

		public ScreenShotCapture (string name, string path, int resolutionWidth, int resolutionHeight, Camera camera) {

		}

		public void MakeScreenShot () {

		}

		public void MakeSeries (int amount, float interval) {

		}

		public void MakeDelayed (float interval) {

		}

		private string GetName (string name) {
			string fileName = "", optional = "";
			int counter = 0;
			// Create Directory + Trim.
			while (true) {
				fileName = string.Format ("{0}/Data/Screenshots/{1}{2}.csv", 
					Application.dataPath, 
					name,
					optional);
				if (File.Exists (fileName)) {
					Debug.Log ("Try to cast " + fileName);
					counter++;
					optional = " (" + counter + ")";
				} else
					break;
			}
			return fileName;
		}
	}


	Camera simulationCamera, learningCamera;

	public int resWidth = 2400; 
	public int resHeight = 900;
	public int series = 0;

	[HideInInspector]
	public bool takeHiResShot = false;

	private int screenCounter = 0;

	public string ScreenShotName(int width, int height) {
		return string.Format("{0}/Data/Screenshots/screen-{1}-{2}.png", 
			Application.dataPath, 
			series,
			screenCounter);
	}

	public void MakeScreenshot() {
		takeHiResShot = true;
		if (takeHiResShot && screenCounter <= 40) {
			RenderTexture rt = new RenderTexture (resWidth, resHeight, 24);
			simulationCamera.targetTexture = rt;
			Texture2D screenShot = new Texture2D (resWidth, resHeight, TextureFormat.RGB24, false);
			simulationCamera.Render ();
			RenderTexture.active = rt;
			screenShot.ReadPixels (new Rect (0, 0, resWidth, resHeight), 0, 0);
			simulationCamera.targetTexture = null;
			RenderTexture.active = null;
			Destroy (rt);
			byte[] bytes = screenShot.EncodeToPNG ();
			string filename = ScreenShotName (resWidth, resHeight);
			System.IO.File.WriteAllBytes (filename, bytes);
			Debug.Log (string.Format ("Took screenshot to: {0}", filename));
			screenCounter++;
			takeHiResShot = false;
		}
	}
		
	void Start () {
		simulationCamera = GameObject.Find ("Simulation Camera").GetComponent<Camera>();
		learningCamera = GameObject.Find ("Learning Camera").GetComponent<Camera>();
	}

	void Update () {
		
	}
}
