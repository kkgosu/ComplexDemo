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
    private float[] array;

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

        int numOfModules = 21;

        array = createCFG.CreateSnake(numOfModules);
        string path = createXML
            .CreateHeader("test123", new Vector3(-14, 5, -16), Quaternion.Euler(0, -90, -90))
            .AddModules(numOfModules, createXML.CreateModules(array))
            .AddConnections(createXML.CreateSimpleConnections(numOfModules, false))
            .Create("Znake2");

        MR.angles = array;
        MR.Load(path);

        currentState = State.SNAKE;
    }

    private void HandleMovement(IEnumerator walkerMove, IEnumerator wheelMove, Direction direction)
    {
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
                    StartCoroutine(walkerMove);
                    break;
                }
            case State.WHEEL:
                {
                    StopAllMovement();
                    StartCoroutine(wheelMove);
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
            HandleMovement(walkerMovement.RotateToTheLeft(MR), wheelMovement.MoveForward(MR), Direction.ROTATE_LEFT);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            HandleMovement(walkerMovement.RotateToTheRight(MR), wheelMovement.MoveForward(MR), Direction.ROTATE_RIGHT);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            HandleMovement(walkerMovement.MoveForward(MR), wheelMovement.MoveForward(MR), Direction.FORWARD);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            HandleMovement(walkerMovement.MoveLeft(MR), wheelMovement.MoveLeft(MR), Direction.LEFT);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            HandleMovement(walkerMovement.MoveBackward(MR), wheelMovement.MoveBackward(MR), Direction.BACKWARD);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            HandleMovement(walkerMovement.MoveRight(MR), wheelMovement.MoveRight(MR), Direction.RIGHT);
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
    }
}
