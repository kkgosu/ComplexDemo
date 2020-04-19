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

        float[] array = createCFG.CreateWalker(21);

        string path = createXML
            .CreateHeader("test123", new Vector3(0,1,0), Quaternion.Euler(0,0,90))
            .AddModules(21, createXML.CreateModules(array))
            .AddConnections(createXML.CreateConnectionsForWalker(21))
            .Create("Znake2");

        MR.Load(path);
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
