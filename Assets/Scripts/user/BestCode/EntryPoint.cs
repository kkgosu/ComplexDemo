﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    ModularRobot MR;
    GaitControlTable gctWheel;
    private WheelMovement wheelMovement;
    private WalkerMovement walkerMovement;
    private SnakeMovementNew snakeMovementNew;
    private SnakeMovement snakeMovement;
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
        walkerMovement = MR.gameObject.AddComponent<WalkerMovement>();
        snakeMovementNew = MR.gameObject.AddComponent<SnakeMovementNew>();
        gctWheel = MR.gameObject.AddComponent<GaitControlTable>();
        MR.gameObject.AddComponent<COM_Controller>();

/*         snakeMovement = MR.gameObject.AddComponent<SnakeMovement>();
         */
        int numOfModules = 21;

        array = createCFG.CreateWalker(numOfModules);
        string path = createXML
            .CreateHeader("test123", new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 90))
            .AddModules(numOfModules, createXML.CreateModules(array))
            .AddConnections(createXML.CreateConnectionsForWalker(numOfModules))
            .Create("Znake2");
        MR.angles = array;
        MR.Load(path);


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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            wheelMovement.isMoving = false;
            walkerMovement.isMoving = false;
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            wheelMovement.isMoving = false;
            StartCoroutine(wheelMovement.MoveForward(MR));
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            wheelMovement.isMoving = false;
            StartCoroutine(wheelMovement.MoveBackward(MR));
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.MoveForward(MR));
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.MoveBackward(MR));
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.MoveRight(MR));
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.MoveLeft(MR));
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.RotateToTheLeft(MR));
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.RotateToTheRight(MR));
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
/*            gctWheel = MR.gameObject.AddComponent<GaitControlTable>();
            gctWheel.ReadFromFile(MR, "Test_Snake_GCT.txt");
            gctWheel.Begin();*/
            snakeMovementNew.isMoving = false;
            StartCoroutine(snakeMovementNew.MoveBackward(MR));
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            Transformations transformations = MR.gameObject.AddComponent<Transformations>();
            StartCoroutine(transformations.Execute(
                MR, gctWheel,
                transformations.WheelToSnake,
                transformations.SnakeToWalker));
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
