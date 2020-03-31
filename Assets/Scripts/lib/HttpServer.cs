using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class HttpServer : MonoBehaviour {
	private static string startUpPath;
	public int port = 80;
	private string handler;
	private HttpListenerContext handlerContext;
	//private volatile bool isHandlerRequest = false;
	Queue <HttpListenerContext> QRequest = new Queue<HttpListenerContext>();
	public GameObject handlerGO;
	HttpListener httpListener;

	public void HttpListenerCallback(IAsyncResult result)
	{
		
		HttpListener httpListener = (HttpListener)result.AsyncState;
		HttpListenerContext context = httpListener.EndGetContext (result);
		HttpListenerResponse response = context.Response;
		response.Headers.Add("Server", "ModRobSim");
		string filename = context.Request.RawUrl.ToLower();

		// temp
		if (filename.StartsWith ("/modrobsim/handler")) {
			Debug.Log ("Recieve HTTP Request");
			handler = filename.Remove (filename.IndexOf ('?'));
			handler = handler.Remove (0, 10);
			QRequest.Enqueue (context);
			return;
		} //temp


		string file = "";
		if (filename == "/" || filename == "index.html") {
			file = startUpPath + "/index.html";
			context.Response.Headers.Set (HttpResponseHeader.ContentType, "text/html");
		} else if (filename.StartsWith("/modrobsim/handler")) {
			handler = filename.Remove (filename.IndexOf ('?'));
			handler = handler.Remove(0, 10);
			handlerContext = context;
			QRequest.Enqueue (handlerContext);
			return;
		} else if (filename == "/modrobsim/"){
			file = startUpPath + "/modrobsim.html";
			context.Response.Headers.Set (HttpResponseHeader.ContentType, "text/html");
		} else if (filename.EndsWith (".gif") || filename.EndsWith (".png") || filename.EndsWith (".jpg")) {
			file = startUpPath + "/img/" + filename;
			context.Response.Headers.Set (HttpResponseHeader.ContentType, "image/" + filename.Remove (0, filename.Length - 3));
		} else if (filename.EndsWith (".css")) {
			file = startUpPath + "/css/" + filename;
			context.Response.Headers.Set (HttpResponseHeader.ContentType, "text/css");
		} else {
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			SendResponse (response, "MRS: Incorrect");
			return;
		}

		if (!File.Exists (file)) {
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			SendResponse (response, "MRS: File not found\"");
		}
		else {
			string responseString = File.ReadAllText(file);
			SendResponse (response,responseString);
		}
	}

	private void SendResponse(HttpListenerResponse response, string responseString)
	{
		byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
		response.ContentLength64 = buffer.Length;
		response.OutputStream.Write(buffer,0,buffer.Length);
		response.OutputStream.Close();
	}


	IEnumerator ServerLoop()
	{
		startUpPath = Application.dataPath + "/www";
		httpListener = new HttpListener();
		httpListener.Prefixes.Add (string.Format("http://*:{0}/", port.ToString()));
		httpListener.Start ();
		while (true) {
			IAsyncResult result = httpListener.BeginGetContext (new System.AsyncCallback (HttpListenerCallback), httpListener);
			yield return new WaitForFixedUpdate ();
		}

	}
	// Use this for initialization
	void Start () {
		StartCoroutine (ServerLoop());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		

		if (QRequest.Count!=0) {
			Debug.Log ("CallHTTP Request");
			handlerGO.SendMessage ("Callback",QRequest.Dequeue());
		}
	}
}
