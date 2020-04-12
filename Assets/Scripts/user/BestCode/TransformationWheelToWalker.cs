using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationWheelToWalker : MonoBehaviour
{
    ModularRobot MR; // Колесо
    GaitControlTable gctWheel;

    // Start is called before the first frame update
    void Start()
    {
        var Rob = new GameObject();
        MR = Rob.AddComponent<ModularRobot>();
        

        CreateXML createXML = MR.gameObject.AddComponent<CreateXML>();
        CreateCFG createCFG = MR.gameObject.AddComponent<CreateCFG>();

        float[] array = createCFG.CreateRoundedWheel(21);

        List<string> conenctions = new List<string>();
        conenctions.Add(createXML.CreateConnectionString(0, 1, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(1, 2, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(2, 3, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(3, 4, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(4, 5, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(5, 6, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(6, 7, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(7, 8, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(8, 9, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(9, 10, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(10, 11, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(11, 12, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(12, 13, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(13, 14, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(14, 15, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(15, 16, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(16, 17, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(17, 18, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(18, 19, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(19, 20, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));
        conenctions.Add(createXML.CreateConnectionString(20, 0, CreateXML.Sides.TOP, CreateXML.Sides.BOTTOM));

        string path = createXML
            .CreateHeader("test123", new Vector3(0,1,0), Quaternion.Euler(0,0,-90))
            .AddModules(21, createXML.CreateModules(array))
            .AddConnections(conenctions)
            .Create("Znake2");

        MR.Load(path, Vector3.right * 1.095f);
        MR.gameObject.AddComponent<COM_Controller>();

        /*        Transformations transformations = MR.gameObject.AddComponent<Transformations>();
                StartCoroutine(transformations.Execute(
                    MR, gctWheel, 
                    transformations.MoveWheelBack,
                    transformations.WheelToSnake,
                    transformations.SnakeToWalker));*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WalkerToWheel()
    {
        StartCoroutine(TransformWalkerToWheel());
    }

    IEnumerator TransformWalkerToWheel()
    {

        gctWheel.ReadFromFile(MR, "SnakeToWalker_5_rev.txt");
        gctWheel.BeginTillEnd();
        while (gctWheel.inProgress || !gctWheel.isReady)
        {
            print("REVERSE PLAY: STW 4");
            print("Smikanie noG_G");
            yield return new WaitForEndOfFrame();
        }

        gctWheel.ReadFromFile(MR, "SnakeToWalker_4_rev.txt");
        gctWheel.BeginTillEnd();
        while (gctWheel.inProgress || !gctWheel.isReady)
        {
            print("REVERSE PLAY: STW 3");
            print("Smikanie noG_G");
            yield return new WaitForEndOfFrame();
        }

         gctWheel.ReadFromFile(MR, "SnakeToWalker_2.txt");
         gctWheel.BeginTillEnd();
         while (gctWheel.inProgress || !gctWheel.isReady)
         {
             print("REVERSE PLAY: STW 2");
             yield return new WaitForEndOfFrame();
         }

         yield return new WaitForSeconds(1f);
         MR.modules[412].surfaces["top"].Disconnect();
         MR.modules[413].surfaces["bottom"].Disconnect();
         MR.modules[417].surfaces["top"].Connect(MR.modules[418].surfaces["bottom"]);
         MR.modules[47].surfaces["top"].Connect(MR.modules[48].surfaces["bottom"]);

         gctWheel.ReadFromFile(MR, "SnakeToWalker_1_rev.txt");
         gctWheel.BeginTillEnd();
         while (gctWheel.inProgress || !gctWheel.isReady)
         {
             yield return new WaitForEndOfFrame();
         }

        gctWheel.ReadFromFile(MR, "SnakeToWalker_1_2_rev.txt");
        gctWheel.BeginTillEnd();
        while (gctWheel.inProgress || !gctWheel.isReady)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
