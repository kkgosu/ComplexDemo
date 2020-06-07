using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ModularRobot : MonoBehaviour {

	GameObject robot;
	public Quaternion rotation = Quaternion.Euler(Vector3.zero);
	public Dictionary<int, Module> modules = new Dictionary<int, Module>();
	private string robotType;
	public GaitControlTable ControlTable; 
	public int referModuleId=-1;
	protected int referDistanceModuleId = -1;
	public float x=0, dx=0;
	public float y=0,  dy=0;
	public float z=0, dz=0;
	private int firstUpdate=2;

	public float[] angles;

	public Vector3 position
    {
        get
        {
            return GetPosition();
        }
    }

    private Vector3 GetPosition() {
        if (modules.ContainsKey(referModuleId))
            return modules[referModuleId].position;
        else if (modules.Values.Count > 0)
        {
            Vector3 pos = Vector3.zero;
            int count = 0;
            foreach (Module module in modules.Values)
            {
                pos += module.position;
                count++;
            }
            if (count > 0)
                pos /= count;
            return pos;
        }        
        else
            Debug.LogError(string.Format("Can't locate robot {0}. There is neither referenced module nor any modules in structure.", name));
        return Vector3.zero;
    }

    private Vector3 GetPositionFromXML(string xml)
	{
		XmlDocument doc = new XmlDocument();
		doc.LoadXml (xml.ToLower()); 
		XmlElement root = doc.DocumentElement; // get the root (robot) element
		string[] pos = root.GetAttribute ("position").Split (new char[] { ',' });
		Vector3 position = Vector3.zero;
		if (pos.Length == 3) // проверка количества компонент вектора
			position = new Vector3 (float.Parse (pos [0].Replace(".", ",")), float.Parse (pos [1].Replace(".", ",")), float.Parse (pos [2].Replace(".", ",")));
		return position;
	}

	public void LoadXML(string xml)
	{
		LoadXML (xml, GetPositionFromXML (xml));
	}

	public void Load(string xmlPath)
	{
		string xml = File.ReadAllText (xmlPath);
		LoadXML (xml);
	}

	public void Load(string xmlPath, Vector3 position)
	{
		string xml = File.ReadAllText (xmlPath);
		LoadXML (xml, position);
	}


	// класс информации о модуле
	private class moduleInfo
	{
		public string moduleType;
		public float [] q;
		public moduleInfo(string mt, float [] q)
		{
			this.moduleType = mt;
			this.q = q;
		}
	}

	// класс информации о соединении
	private class connectionInfo
	{
		public int to;
		public string surfaceTo;
		public string surfaceFrom;
		public float tilt;

		public connectionInfo(int to, string surfaceTo,string surfaceFrom, float tilt)
		{
			this.to = to;
			this.surfaceTo = surfaceTo;
			this.tilt = tilt;
			this.surfaceFrom = surfaceFrom;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	/*void FixedUpdate () {
		if (firstUpdate > 0) {
			// подождать несколько кадров для установления начальной позиции
			dx = modules [referDistanceModuleId].x;
			dy = modules [referDistanceModuleId].y;
			dz = modules [referDistanceModuleId].z;
			firstUpdate--;
		} 
		x = modules [referDistanceModuleId].x - dx;
		y = modules [referDistanceModuleId].y - dy;
		z = modules [referDistanceModuleId].z - dz;
		
		/*
		if (Input.GetKeyDown (KeyCode.D)) {
			Debug.Log (string.Format ("X: {0} Y: {1} Z: {2}", x.ToString (), y.ToString (), z.ToString ()));
		}==/


	}*/


	public void LoadXML(string xml, Vector3 position = default(Vector3))
	{
		Dictionary <int, moduleInfo> mInfo = new Dictionary<int, moduleInfo>();
		Dictionary<int, Queue<connectionInfo>> cInfo = new Dictionary<int, Queue<connectionInfo>>();
		XmlDocument doc = new XmlDocument();
		doc.LoadXml (xml.ToLower()); 
		XmlElement root = doc.DocumentElement; // get the root (robot) element

		// Attributes <robot>
		gameObject.transform.name = string.Format("Modular Robot");
		if (root.HasAttribute ("name"))
			gameObject.transform.name += " " + root.GetAttribute ("name");
		else 
			Debug.LogWarning ("XML doesn't contains \"name\" tag for <robot> tag");
		if (root.HasAttribute ("refid"))
			referModuleId = int.Parse (root.GetAttribute ("refid"));
		else
			Debug.Log ("XML doesn't contains \"refId\" atrribute in <robot> tag ");

		string[] rot = root.GetAttribute ("rotation").Split (new char[] { ',' });
		if (rot.Length == 3)
			rotation = Quaternion.Euler (new Vector3 (float.Parse (rot [0]), float.Parse (rot [1]), float.Parse (rot [2])));

		string moduleType = root.GetAttribute ("moduletype");
		string robotType = root.GetAttribute ("robottype");

		// Reference module search
		XmlNode modulesXML = root.SelectSingleNode ("modules");
		modules = new Dictionary<int, Module> (modulesXML.ChildNodes.Count);
		XmlElement refM;
		if (referModuleId == -1) {
			refM = (XmlElement)modulesXML.FirstChild;	
			referModuleId = int.Parse (refM.GetAttribute ("id"));
		}

		if (root.HasAttribute ("refdistid"))
			referDistanceModuleId = int.Parse (root.GetAttribute ("refdistid"));
		else
			referDistanceModuleId = referModuleId;

		foreach (XmlElement child in modulesXML.ChildNodes) {
			string mt = child.HasAttribute ("type") ? child.GetAttribute ("type") : root.GetAttribute ("moduletype");
			int mid = int.Parse(child.GetAttribute ("id"));
			mInfo.Add(mid,new moduleInfo(mt,new float[Modules.GetModuleByName(mt).numberOfDrivers]));
			for (int i = 1; i <= mInfo [mid].q.Length; i++)
				mInfo [mid].q [i-1] = child.HasAttribute(("q" + i)) ? float.Parse(child.GetAttribute ("q" + i)) : 0f;
		}

		// Устанавливаем начальный модуль
		modules.Add (referModuleId,Modules.Create (Modules.GetModuleByName(mInfo[referModuleId].moduleType), position, rotation, mInfo[referModuleId].q, parent: transform, ID:referModuleId));
		mInfo.Remove(referModuleId);

		XmlNode connectionsXML = root.SelectSingleNode ("connections");

		foreach (XmlElement child in connectionsXML.ChildNodes) {
			if (!(child.HasAttribute ("from") && child.HasAttribute ("to") && child.HasAttribute ("surfacefrom") && child.HasAttribute ("surfaceto"))) {
				modules [referModuleId].Destroy ();
				Debug.LogError ("XML has not attribute \"from\", \"to\", \"surfaceFrom\" or \"surfaceTo\" in <connection> tag");
				break;
			}
			int from = int.Parse (child.GetAttribute ("from"));
			int to = int.Parse (child.GetAttribute ("to"));
			string surfaceFrom = child.GetAttribute ("surfacefrom");
			string surfaceTo = child.GetAttribute ("surfaceto");
			float tilt = child.HasAttribute ("tilt") ? float.Parse (child.GetAttribute ("tilt")) : 0f;
			if (!cInfo.ContainsKey (int.Parse (child.GetAttribute ("from"))))
				cInfo.Add (from, new Queue<connectionInfo> ());
			cInfo[from].Enqueue(new connectionInfo(to,surfaceTo,surfaceFrom,tilt));
		}

		int currentId = referModuleId;
		Queue<int> ids = new Queue<int> ();
		while (cInfo.Count !=0) {
			if (cInfo.ContainsKey (currentId)) {
				while (cInfo [currentId].Count > 0) {
					int newId = cInfo [currentId].Peek ().to;
					connectionInfo connection = cInfo [currentId].Dequeue ();
					if (mInfo.ContainsKey (newId)) {
						// нужна проверка не занята ли уже площадка с этим именем, если занята заканчиваем это все с ошибкой необходим метод Surface.IsConnected()
						modules.Add (newId, 
							modules [currentId].GetSurfaceByName (connection.surfaceFrom).Add (
								Modules.GetModuleByName (mInfo [newId].moduleType),
								Modules.GetModuleByName (mInfo [newId].moduleType).GetSurfaceByName (connection.surfaceTo),
								mInfo [newId].q,
								connection.tilt,
								parent: transform,
								ID: newId));

						mInfo.Remove (newId);

						if (!ids.Contains (newId))
							ids.Enqueue (newId);

					} else {
						// если уже такой модуль есть то просто соединяем площадки (lops)
						if (modules.ContainsKey (newId))
							modules [currentId].GetSurfaceByName (connection.surfaceFrom).Connect (modules [newId].GetSurfaceByName (connection.surfaceTo));
						else {
							Debug.Log ("XML doesn't contains <module> with id =" + newId.ToString ());
							return;
						}
					}
					if (cInfo [currentId].Count == 0) {
						cInfo.Remove (currentId);
						break;
					}
				}
			}

			if (ids.Count > 0)
				currentId = ids.Dequeue ();
			else
				break;
		}

		Debug.Log (string.Format("{0} created. Reference module id: {1}",transform.name, referModuleId.ToString ()));
	}


}
