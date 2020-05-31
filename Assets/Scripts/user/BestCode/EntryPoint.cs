using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    ModularRobot MR;
    GaitControlTable gctWheel;
    private WheelMovement wheelMovement;
    private WaveController_5 waveController_5;
    private WalkerMovement walkerMovement;
    private SnakeMovementNew snakeMovementNew;
    private SnakeMovement snakeMovement;
    private Transformations transformations;
    private CreateXML createXML;
    private CreateCFG createCFG;
    private float[] array;
    // Start is called before the first frame update
    void Start()
    {
        var Rob = new GameObject();
        print(Rob.transform.localScale.x);
        MR = Rob.AddComponent<ModularRobot>();
        MR.gameObject.AddComponent<Movement>();

        createXML = MR.gameObject.AddComponent<CreateXML>();
        createCFG = MR.gameObject.AddComponent<CreateCFG>();
        wheelMovement = MR.gameObject.AddComponent<WheelMovement>();
        walkerMovement = MR.gameObject.AddComponent<WalkerMovement>();
        snakeMovementNew = MR.gameObject.AddComponent<SnakeMovementNew>();
        gctWheel = MR.gameObject.AddComponent<GaitControlTable>();
        waveController_5 = MR.gameObject.AddComponent<WaveController_5>();
        transformations = MR.gameObject.AddComponent<Transformations>();

        int numOfModules = 21;

        array = createCFG.CreateSnake(numOfModules);
        string path = createXML
            .CreateHeader("test123", new Vector3(-14, 5, -16), Quaternion.Euler(0, -90, -90))
            .AddModules(numOfModules, createXML.CreateModules(array))
            .AddConnections(createXML.CreateSimpleConnections(numOfModules, false))
            .Create("Znake2");

        MR.angles = array;
        MR.Load(path);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.StepOver1());
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.StepOver2());
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.StepOver3());
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            walkerMovement.isMoving = false;
            StartCoroutine(walkerMovement.StepOver4());
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(transformations.MakeSnake());
            waveController_5.Go(10, 1.5, 1.5, true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            wheelMovement.isMoving = false;
            walkerMovement.isMoving = false;

            waveController_5.Stop();
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
            snakeMovementNew.isMoving = false;
            StartCoroutine(snakeMovementNew.MoveBackward(MR));
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            StartCoroutine(transformations.Execute(
                //transformations.MakeSnake
                transformations.SnakeToWalker
                //transformations.WalkerToSnake,
                //transformations.SnakeToWheel
                //transformations.WheelToSnake,
                //transformations.SnakeToWalker
                /*transformations.WalkerToSnake*/));
        }
    }
}
