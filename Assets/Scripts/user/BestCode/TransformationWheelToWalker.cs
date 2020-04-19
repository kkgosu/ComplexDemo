﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TransformationWheelToWalker : MonoBehaviour
{
    ModularRobot MR;
    GaitControlTable gctWheel;
    private WheelMovement wheelMovement;
    private CreateXML createXML;
    private CreateCFG createCFG;
    private float[] array;
    // Start is called before the first frame update
    void Start()
    {
        var Rob = new GameObject();
        MR = Rob.AddComponent<ModularRobot>();
        
        createXML = MR.gameObject.AddComponent<CreateXML>();
        createCFG = MR.gameObject.AddComponent<CreateCFG>();
        wheelMovement = MR.gameObject.AddComponent<WheelMovement>();

        array = createCFG.CreateRoundedWheel(21);
        string path = createXML
            .CreateHeader("test123", new Vector3(0,0,0), Quaternion.Euler(0,0,-90))
            .AddModules(21, createXML.CreateModules(array))
            .AddConnections(createXML.CreateSimpleConnections(21))
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
        if (Input.GetKeyUp(KeyCode.W))
        {
            wheelMovement.isMoving = false;
            StartCoroutine(wheelMovement.MoveForward(MR, array));
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            wheelMovement.isMoving = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            wheelMovement.isMoving = false;
            StartCoroutine(wheelMovement.MoveBackward(MR, array));
        }
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
