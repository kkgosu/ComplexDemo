using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Experimenting : MonoBehaviour
{

    GameObject robotGO;
    ModularRobot robot;

    Dropdown dropdownKinematic, dropdownAlgorithm;
    Button buttonStart, buttonStop;

    List<string> robotConfigurations, wheelAlgorithms, snakeAlgorithms, snakeTurningAlgorithms, walkerAlgorithms;

    COM_Controller comController;

    float startingTime;

    static class ConfigurationNames
    {
        public static string snake6 = "Snake 6";
        public static string snake8 = "Snake 8";
        public static string snake10 = "Snake 10";
        public static string snake12 = "Snake 12";
        public static string turningSnake11 = "Turning Snake 11";
        public static string turningSnake13 = "Turning Snake 13";
        public static string wheel45 = "Wheel 12 - 4x45";
        public static string wheel60 = "Wheel 12 - 3x60";
        public static string walker = "Walking Robot 13";
    }

    static class AlgorithmNames
    {
        public static string snakeWave = "Wave Motion (Кадочников)";
        public static string snakeFig = "Figure-based Motion (Шилов)";
        public static string snakeSin = "Sin Motion";
        public static string turning15 = "Turning - 15";
        public static string turning30 = "Turning - 30";
        public static string turning45 = "Turning - 45";
        public static string turning60 = "Turning - 60";
        public static string turning75 = "Turning - 75";
        public static string turning90 = "Turning - 90";
        public static string kinematicMotion = "Kinematic Motion";
        public static string algorithm0 = "Algorithm 0";
        public static string algorithm1 = "Algorithm 1";
        public static string algorithm2 = "Algorithm 2";

    }

    void Start () {
        //comController = gameObject.AddComponent<COM_Controller>();
        robotGO = new GameObject();
        robotGO.transform.name = "Экспериментальный робот";
        robotGO.AddComponent<COM_Controller>();

        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(0, 0, 90);
        robot = Robots.CreateSnake(Modules.M3R, new Vector3(0, 1, 0), rot, 6, robotName: "Snake 6");
        robot.gameObject.AddComponent<WaveControl>();
        WaveController wc = robot.gameObject.AddComponent<WaveController>();
        wc.AddModulesFromRobot(robot);
        wc.Init();

        robotConfigurations = new List<string>
        {
            ConfigurationNames.snake6,
            ConfigurationNames.snake8,
            ConfigurationNames.snake10,
            ConfigurationNames.snake12,
            ConfigurationNames.turningSnake11,
            ConfigurationNames.turningSnake13,
            ConfigurationNames.wheel45,
            ConfigurationNames.wheel60,
            ConfigurationNames.walker
        };

        snakeAlgorithms = new List<string>
        {
            AlgorithmNames.snakeWave,
            AlgorithmNames.snakeFig,
            AlgorithmNames.snakeSin

        };

        snakeTurningAlgorithms = new List<string>
        {
            AlgorithmNames.turning15,
            AlgorithmNames.turning30,
            AlgorithmNames.turning45,
            AlgorithmNames.turning60,
            AlgorithmNames.turning75,
            AlgorithmNames.turning90
        };

        wheelAlgorithms = new List<string>
        {
            AlgorithmNames.kinematicMotion
        };

        walkerAlgorithms = new List<string>
        {
            AlgorithmNames.algorithm0,
            AlgorithmNames.algorithm1,
            AlgorithmNames.algorithm2
        };

        //dropdown.options.Clear();

        dropdownKinematic = UIHelper.AddDropdown(robotConfigurations);
        RectTransform ddkRT = dropdownKinematic.GetComponent<RectTransform>();
        ddkRT.anchorMax = new Vector2(1, 1);
        ddkRT.anchorMin = new Vector2(1, 1);
        ddkRT.pivot = new Vector2(1, 1);
        ddkRT.anchoredPosition = new Vector3(-10, -10, 0);
        dropdownKinematic.onValueChanged.AddListener(delegate {
            KinematicChanged(dropdownKinematic);
        });

        dropdownAlgorithm = UIHelper.AddDropdown(snakeAlgorithms);
        RectTransform ddaRT = dropdownAlgorithm.GetComponent<RectTransform>();
        ddaRT.anchorMax = new Vector2(1, 1);
        ddaRT.anchorMin = new Vector2(1, 1);
        ddaRT.pivot = new Vector2(1, 1);
        ddaRT.anchoredPosition = new Vector3(-10, -50, 0);

        buttonStart = UIHelper.AddButton(buttonText: "Start");
        buttonStart.onClick.AddListener(delegate () { StartPressed(); });
        RectTransform brRT = buttonStart.GetComponent<RectTransform>();
        brRT.anchorMax = new Vector2(1, 1);
        brRT.anchorMin = new Vector2(1, 1);
        brRT.pivot = new Vector2(1, 1);
        brRT.anchoredPosition = new Vector3(-10, -90, 0);

        buttonStop = UIHelper.AddButton(buttonText: "Stop");
        buttonStop.onClick.AddListener(delegate () { StopPressed(); });
        RectTransform bcRT = buttonStop.GetComponent<RectTransform>();
        bcRT.anchorMax = new Vector2(1, 1);
        bcRT.anchorMin = new Vector2(1, 1);
        bcRT.pivot = new Vector2(1, 1);
        bcRT.anchoredPosition = new Vector3(-10, -130, 0);
    }

    void KinematicChanged (Dropdown dd) {
        switch (dd.value) {
            case 0:
            case 1:
            case 2:
            case 3:
                dropdownAlgorithm.options.Clear();
                dropdownAlgorithm.AddOptions(snakeAlgorithms);
                break;
            case 4:
            case 5:
                dropdownAlgorithm.options.Clear();
                dropdownAlgorithm.AddOptions(snakeTurningAlgorithms);
                break;
            case 6:
            case 7:
                dropdownAlgorithm.options.Clear();
                dropdownAlgorithm.AddOptions(wheelAlgorithms);
                break;
            case 8:
                dropdownAlgorithm.options.Clear();
                dropdownAlgorithm.AddOptions(walkerAlgorithms);
                break;
        }
    }

    void StartPressed () {
        if (robot != null)
        {
            //robot.GetComponentInParent<COM_Controller>().DestroyWindow();
            Destroy(robot.gameObject);
        }
        string gctName = "";
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(0, 0, 90);
        switch (dropdownKinematic.value)
        {
            case 0:
                robot = Robots.CreateSnake(Modules.M3R, new Vector3(0, 1, 0), rot, 6, robotName: "Snake 6");
                gctName = "Snake_6.txt";
                break;
            case 1:
                robot = Robots.CreateSnake(Modules.M3R, new Vector3(0, 1, 0), rot, 8, robotName: "Snake 8");
                gctName = "Snake_8.txt";
                break;
            case 2:
                robot = Robots.CreateSnake(Modules.M3R, new Vector3(0, 1, 0), rot, 10, robotName: "Snake 10");
                gctName = "Snake_10.txt";
                break;
            case 3:
                robot = Robots.CreateSnake(Modules.M3R, new Vector3(0, 1, 0), rot, 12, robotName: "Snake 12");
                gctName = "Snake_12.txt";
                /*switch (dropdownAlgorithm.value)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }*/
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                robot = Robots.CreateWheel(Modules.M3R, new Vector3 (0, 1, 0), rot, 12, robotName: "Wheel 4x45");
                gctName = "Wheel_4x45.txt";
                break;
            case 7:
                robot = Robots.CreateWheel(Modules.M3R, new Vector3 (0, 1, 0), rot, 12, arcCount: 3, arcAngle: 60, robotName: "Wheel 3x60");
                gctName = "Wheel_3x60.txt";
                break;
            case 8:
                break;
            case 9:
                break;
        }

        robot.transform.parent = robotGO.transform;
        //robot.gameObject.AddComponent<COM_Controller>();
        GaitControlTable gct = robot.gameObject.AddComponent<GaitControlTable>();
        gct.ReadFromFile(robot, gctName);
        gct.isKeyboardControlled = true;
        //gct.BeginLoop();
    }

    void StopPressed () {


    }
}
