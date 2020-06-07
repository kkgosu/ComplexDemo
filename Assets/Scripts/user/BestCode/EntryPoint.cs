using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    enum State
    {
        WHEEL, WALKER, SNAKE
    }

    enum Direction
    {
        FORWARD, BACKWARD, LEFT, RIGHT, ROTATE_RIGHT, ROTATE_LEFT
    }

    ModularRobot MR;
    private WheelMovement wheelMovement;
    private WaveController_5 waveController_5;
    private SnakeFold fold;
    private WalkerMovement walkerMovement;
    private Transformations transformations;
    private CreateXML createXML;
    private CreateCFG createCFG;
    private RRT_main1 rRT;
    private float[] array;

    ModularRobot wheel;
    public Vector3 position;

    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        var Rob = new GameObject();
        print(Rob.transform.localScale.x);
        MR = Rob.AddComponent<ModularRobot>();
        MR.gameObject.AddComponent<Movement>();
        MR.gameObject.AddComponent<GaitControlTable>();

        createXML = MR.gameObject.AddComponent<CreateXML>();
        createCFG = MR.gameObject.AddComponent<CreateCFG>();
        wheelMovement = MR.gameObject.AddComponent<WheelMovement>();
        walkerMovement = MR.gameObject.AddComponent<WalkerMovement>();
        waveController_5 = MR.gameObject.AddComponent<WaveController_5>();
        fold = MR.gameObject.AddComponent<SnakeFold>();

        transformations = MR.gameObject.AddComponent<Transformations>();
        rRT = MR.gameObject.AddComponent<RRT_main1>();

        int numOfModules = 21;

        array = createCFG.CreateSnake(numOfModules);
        string path = createXML
            //.CreateHeader("test123", new Vector3(-14, 5, -16), Quaternion.Euler(0, -90, -90)) //first point
            .CreateHeader("test123", new Vector3(-8, 5, -13), Quaternion.Euler(0, -68, -90)) //second point
            //.CreateHeader("test123", new Vector3(-2.4f, 2, 0), Quaternion.Euler(0, -68, -90)) //third point
            //.CreateHeader("test123", new Vector3(0, 0, 0), Quaternion.Euler(0, -90, -90))
            .AddModules(numOfModules, createXML.CreateModules(array))
            .AddConnections(createXML.CreateSimpleConnections(numOfModules, false))
            .Create("Znake2");

        MR.angles = array;
        MR.Load(path);

        wheel = gameObject.AddComponent<ModularRobot>();
        wheel.Load(Application.dataPath + "/Resources/Configurations/Turning Snake13.xml");
        wheel.gameObject.AddComponent<LineRobAdmin>();


        currentState = State.SNAKE;
    }

    private void HandleMovement(Func<ModularRobot, IEnumerator> walkerMove, Func<ModularRobot, IEnumerator> wheelMove, Direction direction)
    {
        print(currentState);
        switch (currentState)
        {
            case State.SNAKE:
                {
                    switch (direction)
                    {
                        case Direction.FORWARD:
                            {
                                StopAllMovement();
                                StartCoroutine(transformations.MakeSnake());
                                waveController_5.Go(10, 1.5, 1.5, true);
                                break;
                            }
                        case Direction.BACKWARD:
                            {
                                StopAllMovement();
                                StartCoroutine(transformations.MakeSnake());
                                waveController_5.Go(10, 1.5, 1.5, false);
                                break;
                            }
                        case Direction.ROTATE_RIGHT:
                            {
                                StopAllMovement();
                                fold.Rotate(90);
                                break;
                            }
                        case Direction.ROTATE_LEFT:
                            {
                                StopAllMovement();
                                fold.Rotate(-90);
                                break;
                            }
                    }

                    break;
                }
            case State.WALKER:
                {
                    StopAllMovement();
                    StartCoroutine(walkerMove(MR));
                    break;
                }
            case State.WHEEL:
                {
                    StopAllMovement();
                    StartCoroutine(wheelMove(MR));
                    break;
                }
        }
    }

    private void StopAllMovement()
    {
        waveController_5.Stop();
        walkerMovement.isMoving = false;
        wheelMovement.isMoving = false;
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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            wheelMovement.isMoving = false;
            walkerMovement.isMoving = false;

            waveController_5.Stop();
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            HandleMovement(walkerMovement.RotateToTheLeft, wheelMovement.MoveForward, Direction.ROTATE_LEFT);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            HandleMovement(walkerMovement.RotateToTheRight, wheelMovement.MoveForward, Direction.ROTATE_RIGHT);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            HandleMovement(walkerMovement.MoveForward, wheelMovement.MoveForward, Direction.FORWARD);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            HandleMovement(walkerMovement.MoveLeft, wheelMovement.MoveLeft, Direction.LEFT);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            HandleMovement(walkerMovement.MoveBackward, wheelMovement.MoveBackward, Direction.BACKWARD);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            HandleMovement(walkerMovement.MoveRight, wheelMovement.MoveRight, Direction.RIGHT);
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            StartCoroutine(transformations.SnakeToWalker());
            currentState = State.WALKER;
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            StartCoroutine(transformations.Execute(transformations.WalkerToSnake, transformations.MakeSnake));
            currentState = State.SNAKE;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            StartCoroutine(transformations.SnakeToWheel());
            currentState = State.WHEEL;
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            StartCoroutine(transformations.Execute(transformations.WheelToSnake, transformations.MakeSnake));
            currentState = State.SNAKE;
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            StartCoroutine(transformations.MakeSnake());
            currentState = State.SNAKE;
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            StartCoroutine(transformations.TightWheel());
            currentState = State.WHEEL;
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            /*            rRT.init(new int[] { 1, 2, 3, 4, 5 });
                        Vector3 points = MR.modules[13].position;
                        rRT.finalPoint = new Vector3(points.x + 0.04f, points.y + 0.04f, points.z + 0.04f);
                        rRT.finalNapr = new Vector3(points.x + 0.1f, points.y + 0.04f, points.z + 0.04f);
                        print("entry rrt x: " + rRT.finalPoint.x);
                        print("entry rrt y: " + rRT.finalPoint.y);
                        print("entry rrt z: " + rRT.finalPoint.z);*/
            rRT.init();
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            rRT.button_test2();
        }
    }
}
