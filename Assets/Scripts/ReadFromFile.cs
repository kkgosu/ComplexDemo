using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Globalization;

public class ReadFromFile : MonoBehaviour
{

    private string folderPath = @"MapGenerator";
    public Dictionary<string, GameObject> prefabMap;
    public float[] scales = new float[100];
    public Quaternion[] rotationsX = new Quaternion[100];
    public Quaternion[] rotationsY = new Quaternion[100];
    public Quaternion[] rotationsZ = new Quaternion[100];
    public GameObject[] objects;

    public float itemXSpread = 10;
    public float itemYSpread = 0;
    public float itemZSpread = 10;

    int xOffset = 0;
    int zOffset = 0;
    int scale = 10;


    public void ParseFile(string file)
    {
        string fullPath = Path.Combine(Path.Combine(Application.dataPath, folderPath), file);
        print(fullPath);
        StreamReader reader = new StreamReader(fullPath);
        int counter = 0;
      
        while (!reader.EndOfStream)
        {
            zOffset = 0;
            string[] items = reader.ReadLine().Split(',');
            foreach (string prefab in items)
            {
                if (!prefab.Equals("0"))
                {
                    Vector3 randPosition = new Vector3(xOffset + scale / 2, 0, zOffset + scale / 2) +
                        transform.position;
                    GameObject clone = Instantiate(prefabMap[prefab], randPosition, Quaternion.identity);

                    if (prefab.Equals("6")) {
                        clone.transform.localScale = new Vector3(scales[counter] * 2, scales[counter], scales[counter]);
                    } else
                    {
                        clone.transform.localScale = new Vector3(scales[counter], scales[counter], scales[counter]);
                    }
                    
                    clone.transform.localRotation *= rotationsX[counter] * rotationsY[counter] * rotationsZ[counter];
                    clone.transform.localPosition = new Vector3(clone.transform.localPosition.x, clone.transform.localPosition.y + 2f, clone.transform.localPosition.z);
                }
                zOffset += scale;
                counter++;
            }
            xOffset += scale;
        }
        
    }

    public void setPrefabMap()
    {
        prefabMap = new Dictionary<string, GameObject>();
        for(int i = 0; i < objects.Length; i++)
        {
            prefabMap.Add(i.ToString(), objects[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        print("Start called");
        setupRotations("rotations.txt");
        setupScaling("scales.txt");
        setPrefabMap();
        ParseFile("test.txt");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setupRotations(string file)
    {
        string fullPath = Path.Combine(Path.Combine(Application.dataPath, folderPath), file);
        print(fullPath);
        StreamReader reader = new StreamReader(fullPath);
        int counter = 0;
        while (!reader.EndOfStream)
        {
            string[] items = reader.ReadLine().Split(';');
            foreach (string cell in items)
            {
                string[] angles = cell.Split(',');
                int x;
                Int32.TryParse(angles[0], out x);
                int y;
                Int32.TryParse(angles[1], out y);
                int z;
                Int32.TryParse(angles[2], out z);
                Quaternion qx = Quaternion.AngleAxis(x, Vector3.right);
                Quaternion qy = Quaternion.AngleAxis(y, Vector3.up);
                Quaternion qz = Quaternion.AngleAxis(z, Vector3.forward);
                rotationsX[counter] = qx;
                rotationsY[counter] = qy;
                rotationsZ[counter] = qz;
                counter++;
            }
        }
    }

    private void setupScaling(string file)
    {
        string fullPath = Path.Combine(Path.Combine(Application.dataPath, folderPath), file);
        StreamReader reader = new StreamReader(fullPath);
        int counter = 0;
        while (!reader.EndOfStream)
        {
            string[] items = reader.ReadLine().Split(',');
            foreach(string scale in items)
            {
                scales[counter] = float.Parse(scale, CultureInfo.InvariantCulture.NumberFormat);
                counter++;
            }
        }
    }
}
