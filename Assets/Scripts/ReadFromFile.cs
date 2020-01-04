using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadFromFile : MonoBehaviour
{

    private string folderPath = @"Resourses/MapGenerator";
    public Dictionary<string, GameObject> prefabMap;
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
        while (!reader.EndOfStream)
        {
            zOffset = 0;
            string[] items = reader.ReadLine().Split(',');
            foreach (string prefab in items)
            {
                print(prefab);
                Vector3 randPosition = new Vector3(xOffset + scale / 2, 0, zOffset + scale / 2) +
                    transform.position;
                GameObject clone = Instantiate(prefabMap[prefab], randPosition, Quaternion.identity);
               
                int r = Random.Range(0, 2);
                print(r);
                if (r == 1)
                {
                    clone.transform.localScale = new Vector3(2, 2, 2);
                }
                zOffset += scale;
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

    private void ReadFile(string path)
    {
        

    }
         
    // Start is called before the first frame update
    void Start()
    {
        print("Start called");
        setPrefabMap();
        ParseFile("test.txt");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
