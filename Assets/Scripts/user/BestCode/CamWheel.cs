using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CamWheel : MonoBehaviour
{
    ModularRobot MR; // Первое Колесо
    ModularRobot MR1; // Шагающая платформа
    ModularRobot MR2; // Второе Колесо
    GaitControlTable gct;
    GaitControlTable gct1;
    GaitControlTable gct2;
    GameObject Rob;
    GameObject Rob1;
    GameObject Rob2;
    GaitControlTable gct01, gct02, gct03;


    // Use this for initialization
    void Start()
    {
        MR = GameObject.Find("Modular Robot test123").GetComponent<ModularRobot>();
        //  TestWheelRobot();
        //OneWheelRobot();
        CreateCamera();
        //TwoWheelRobot();
        //CameraTwo();

    }
    void TestWheelRobot()
    {
        var Rob = new GameObject();
        MR = Rob.AddComponent<ModularRobot>();
        MR.Load(Application.dataPath + "/Resources/Configurations/2legs_1_cam.xml");

    }
    void OneWheelRobot()
    {


        var Rob = new GameObject();
        MR = Rob.AddComponent<ModularRobot>();
        MR.Load(Application.dataPath + "/Resources/Configurations/wheel22.xml", Vector3.right * 1.095f);

        // MR.Load(Application.dataPath + "/Resources/Configurations/Wheel14.xml", Vector3.right);


        //MR.ControlTable.Load(Application.dataPath + "/Resources/Gait Control Tables/Wheel12.gct", MR.modules);
        // GameObject.Find("Module 8" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
        //GameObject.Find("Module 9" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
        //GameObject.Find("Module 10" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
        //GameObject.Find("Module 11" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
        //GameObject.Find("Module 12" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;



    }
    void TwoWheelRobot()
    {

        var Rob = new GameObject();
        MR = Rob.AddComponent<ModularRobot>();
        MR.Load(Application.dataPath + "/Resources/Configurations/wheel14_1.xml", Vector3.forward);
        var Rob2 = new GameObject();
        MR2 = Rob2.AddComponent<ModularRobot>();
        MR2.Load(Application.dataPath + "/Resources/Configurations/wheel14_2.xml", Vector3.forward * 1.279f);
        GameObject.Find("Module 6" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
        GameObject.Find("Module 36" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
    }
    void CreateCamera()
    {
        var Rob1 = new GameObject();
        MR1 = Rob1.AddComponent<ModularRobot>();
        MR1.Load(Application.dataPath + "/Resources/Configurations/2legs_1.xml");

        MR1.modules[25].surfaces["bottom"].Connect(MR.modules[2].surfaces["left"]);
        MR1.modules[20].surfaces["bottom"].Connect(MR.modules[2].surfaces["right"]);
    }

    private int lastModule = 0;

    IEnumerator MoveLegs()
    {
        int moduleToConnect = GetNextModule();
        print("GetNextModule: " + moduleToConnect);
        MR1.modules[25].surfaces["bottom"].Disconnect();
        MR1.modules[22].drivers["q2"].Set(45);
        MR1.modules[25].drivers["q2"].Set(45);
        yield return WaitWhileDriversAreBusy();
        MR1.modules[25].surfaces["bottom"].Connect(MR.modules[moduleToConnect].surfaces["left"]);

        MR1.modules[22].drivers["q2"].Set(0);
        MR1.modules[25].drivers["q2"].Set(0);
        MR1.modules[20].surfaces["bottom"].Disconnect();
        MR1.modules[27].drivers["q2"].Set(45);
        MR1.modules[20].drivers["q2"].Set(45);
        yield return WaitWhileDriversAreBusy();


    }

    private int GetNextModule()
    {
        lastModule += 2;
        if (lastModule >= MR.modules.Count)
        {
            lastModule -= MR.modules.Count;
        }
        return lastModule;
    }

    private IEnumerator WaitWhileDriversAreBusy()
    {
        while (IfAnyDriverIsBusy())
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private bool IfAnyDriverIsBusy()
    {
        bool flag = false;
        foreach (Module module in MR.modules.Values)
        {
            foreach (Driver driver in module.drivers.Values)
            {
                if (driver.busy)
                {
                    flag = true;
                }
            }
        }
        return flag;
    }

    void CameraTwo()
    {
        var Rob1 = new GameObject();
        MR1 = Rob1.AddComponent<ModularRobot>();
        MR1.Load(Application.dataPath + "/Resources/Configurations/2legs_1_cam.xml");
    }

    void InitLegsSnake()
    {
        gct01 = gameObject.AddComponent<GaitControlTable>();
        gct02 = gameObject.AddComponent<GaitControlTable>();
        gct01.ReadFromFile(MR1, "SLegs.txt");
        gct02.ReadFromFile(MR1, "SLegs_0.txt");
        StartCoroutine(Demo2());
    }
    IEnumerator Demo2()
    {

        float delay = 1f;
        float zaderzka = 2f;

        gct01.BeginTillEnd();
        yield return new WaitForSeconds(delay);
        yield return new WaitForEndOfFrame();
        while (gct01.inProgress && !gct01.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(delay);


    }

    void Demo1Init()
    {
        gct01 = gameObject.AddComponent<GaitControlTable>();
        gct02 = gameObject.AddComponent<GaitControlTable>();
        gct03 = gameObject.AddComponent<GaitControlTable>();
        gct01.ReadFromFile(MR1, "Legs_1_GCT.txt");
        gct02.ReadFromFile(MR1, "Legs_2_GCT.txt");
        gct03.ReadFromFile(MR1, "Legs_3_GCT.txt");

        StartCoroutine(Demo1());
    }
    IEnumerator Demo1()
    {
        float delay = 1f;
        float zaderzka = 2f;

        MR1.modules[20].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 1");
        gct01.BeginTillEnd();
        yield return new WaitForSeconds(5.3f);
        yield return new WaitForEndOfFrame();
        while (gct01.inProgress && !gct01.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(delay);
        MR1.modules[20].surfaces["bottom"].Connect(MR.modules[410].surfaces["right"]);
        yield return new WaitForEndOfFrame();
        MR1.modules[25].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 2");
        gct02.BeginTillEnd();
        yield return new WaitForSeconds(5.3f);
        yield return new WaitForEndOfFrame();
        while (gct02.inProgress && !gct02.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(zaderzka);
        MR1.modules[25].surfaces["bottom"].Connect(MR.modules[410].surfaces["left"]);
        yield return new WaitForEndOfFrame();
        MR1.modules[20].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 3");
        gct01.BeginTillEnd();
        yield return new WaitForSeconds(6f);
        yield return new WaitForEndOfFrame();
        while (gct01.inProgress && !gct01.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(delay);
        MR1.modules[20].surfaces["bottom"].Connect(MR.modules[48].surfaces["right"]);
        yield return new WaitForEndOfFrame();
        MR1.modules[25].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 4");
        gct02.BeginTillEnd();
        yield return new WaitForSeconds(6f);
        yield return new WaitForEndOfFrame();
        while (gct02.inProgress && !gct02.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(zaderzka);
        MR1.modules[25].surfaces["bottom"].Connect(MR.modules[48].surfaces["left"]);
        yield return new WaitForEndOfFrame();

        print("Step 5");

        yield return new WaitForSeconds(delay);
        MR1.modules[20].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 6");
        gct01.BeginTillEnd();
        yield return new WaitForSeconds(6.7f);
        yield return new WaitForEndOfFrame();
        while (gct01.inProgress && !gct01.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(delay);
        MR1.modules[20].surfaces["bottom"].Connect(MR.modules[46].surfaces["right"]);
        yield return new WaitForEndOfFrame();
        MR1.modules[25].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 7");
        gct02.BeginTillEnd();
        yield return new WaitForSeconds(6.7f);
        yield return new WaitForEndOfFrame();
        while (gct02.inProgress && !gct02.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(zaderzka);
        MR1.modules[25].surfaces["bottom"].Connect(MR.modules[46].surfaces["left"]);
        yield return new WaitForEndOfFrame();
        MR1.modules[20].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 8");
        gct01.BeginTillEnd();
        yield return new WaitForSeconds(7.4f);
        yield return new WaitForEndOfFrame();
        while (gct01.inProgress && !gct01.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(delay);
        MR1.modules[20].surfaces["bottom"].Connect(MR.modules[44].surfaces["right"]);
        yield return new WaitForEndOfFrame();
        MR1.modules[25].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 9");
        gct02.BeginTillEnd();
        yield return new WaitForSeconds(7.4f);
        yield return new WaitForEndOfFrame();
        while (gct02.inProgress && !gct02.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(zaderzka);
        MR1.modules[25].surfaces["bottom"].Connect(MR.modules[44].surfaces["left"]);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        print("Step 10");
        yield return new WaitForSeconds(1.5f);
        MR1.modules[20].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();
        gct01.BeginTillEnd();
        yield return new WaitForSeconds(8.1f);
        yield return new WaitForEndOfFrame();
        while (gct01.inProgress && !gct01.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1.5f);
        MR1.modules[20].surfaces["bottom"].Connect(MR.modules[42].surfaces["right"]);
        yield return new WaitForEndOfFrame();
        MR1.modules[25].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

        print("Step 11");
        gct02.BeginTillEnd();
        yield return new WaitForSeconds(8.1f);
        yield return new WaitForEndOfFrame();
        while (gct02.inProgress && !gct02.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(5f);
        MR1.modules[25].surfaces["bottom"].Connect(MR.modules[42].surfaces["left"]);
        yield return new WaitForEndOfFrame();
        // MR1.modules[20].surfaces["bottom"].Disconnect();
        yield return new WaitForEndOfFrame();

    }

    void LegsUp()
    {
        gct01 = gameObject.AddComponent<GaitControlTable>();
        gct02 = gameObject.AddComponent<GaitControlTable>();
        gct03 = gameObject.AddComponent<GaitControlTable>();
        gct01.ReadFromFile(MR1, "Slegs_0.txt");
        gct02.ReadFromFile(MR1, "Slegs_1.txt");
        gct03.ReadFromFile(MR1, "Slegs_2.txt");
        StartCoroutine(Demo());

    }
    void WheelUP()
    {
        StartCoroutine(DemoUP());
    }
    IEnumerator Demo()
    {
        gct01.Begin(3);
        while (gct01.inProgress && !gct01.isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(2f);
        print("Count: " + MR.modules.Count);
        MR1.modules[20].surfaces["bottom"].Connect(MR.modules[20].surfaces["left"]);
        MR1.modules[25].surfaces["bottom"].Connect(MR.modules[20].surfaces["right"]);

        foreach(Module module in GameObject.Find("Modular Robot 2legs").GetComponent<ModularRobot>().modules.Values)
        {
            foreach (Rigidbody rb in MR1.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddForce(Vector3.up * 0.1f);
            }
        }
    }
    IEnumerator DemoUP()
    {

        yield return new WaitForEndOfFrame();
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            InitLegsSnake();
        }
        if (Input.GetKeyUp(KeyCode.Y))
        {
            LegsUp();
        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            WheelUP();
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            StartCoroutine(MoveLegs());
/*            var gct = gameObject.AddComponent<GaitControlTable>();
            gct.ReadFromFile(MR, "WheelUp.txt");
            gct.BeginTillEnd();*/
            //MR.modules[1].surfaces["bottom"].Connect(MR.modules[10].surfaces["top"]);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            //MR.modules[1].surfaces["bottom"].Disconnect();
            var gct = gameObject.AddComponent<GaitControlTable>();
            gct.ReadFromFile(MR, "DemoWheelDown.txt");
            gct.BeginTillEnd();
        }
/*        if (Input.GetKeyUp(KeyCode.C))
        {

            var gct = gameObject.AddComponent<GaitControlTable>();
            gct.ReadFromFile(MR, "Test1.txt");
            gct.BeginTillEnd();
        }
        if (Input.GetKeyUp(KeyCode.V))
        {

            var gct = gameObject.AddComponent<GaitControlTable>();
            gct.ReadFromFile(MR, "Test2.txt");
            gct.BeginTillEnd();
        }


        if (Input.GetKeyUp(KeyCode.Space))
        {
            Demo1Init();

        }
        if (Input.GetKeyUp(KeyCode.KeypadEnter))
        {

            //demo2();

        }

        if (Input.GetKeyUp(KeyCode.Keypad1))
        {

            var gct = gameObject.AddComponent<GaitControlTable>();

            MR1.modules[20].surfaces["bottom"].Disconnect();

            gct.ReadFromFile(MR, "Test_wheel22_1_GCT.txt");
            var gct1 = gameObject.AddComponent<GaitControlTable>();
            gct1.ReadFromFile(MR1, "Legs_1_GCT.txt");
            gct1.BeginTillEnd();
            gct.BeginTillEnd();




        }
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            var gct = gameObject.AddComponent<GaitControlTable>();
            var gct1 = gameObject.AddComponent<GaitControlTable>();
            MR1.modules[20].surfaces["bottom"].Connect(MR.modules[410].surfaces["right"]);
            MR1.modules[25].surfaces["bottom"].Disconnect();

            gct.ReadFromFile(MR, "Test_wheel22_2_GCT.txt");

            gct1.ReadFromFile(MR1, "Legs_2_GCT.txt");
            gct1.BeginTillEnd();
            gct.BeginTillEnd();
        }
        if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            var gct = gameObject.AddComponent<GaitControlTable>();
            var gct1 = gameObject.AddComponent<GaitControlTable>();
            MR1.modules[25].surfaces["bottom"].Connect(MR.modules[410].surfaces["left"]);

            gct.ReadFromFile(MR, "Test_wheel22_3_GCT.txt");
            // gct1.ReadFromFile(MR1, "Legs_3_GCT.txt");
            //gct1.BeginTillEnd();
            gct.BeginTillEnd();
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {

            var gct = gameObject.AddComponent<GaitControlTable>();
            var gct1 = gameObject.AddComponent<GaitControlTable>();
            //MR1.modules[25].surfaces["bottom"].Connect(MR.modules[410].surfaces["left"]);

            gct.ReadFromFile(MR, "Test_wheel22_4_GCT.txt");
            // gct1.ReadFromFile(MR1, "Legs_3_GCT.txt");
            // gct1.BeginTillEnd();
            gct.BeginTillEnd();

        }
        if (Input.GetKeyUp(KeyCode.Keypad5))
        {
            var gct = gameObject.AddComponent<GaitControlTable>();
            var gct1 = gameObject.AddComponent<GaitControlTable>();

            MR1.modules[20].surfaces["bottom"].Disconnect();
            gct.ReadFromFile(MR, "Test_wheel22_5_GCT.txt");
            gct1.ReadFromFile(MR1, "Legs_3_GCT.txt");
            gct1.BeginTillEnd();
            gct.BeginTillEnd();
        }
        if (Input.GetKeyUp(KeyCode.Keypad6))
        {
            var gct = gameObject.AddComponent<GaitControlTable>();
            var gct1 = gameObject.AddComponent<GaitControlTable>();
            MR1.modules[20].surfaces["bottom"].Connect(MR.modules[48].surfaces["right"]);
            MR1.modules[25].surfaces["bottom"].Disconnect();
            gct.ReadFromFile(MR, "Test_wheel22_6_GCT.txt");
            gct1.ReadFromFile(MR1, "Legs_4_GCT.txt");
            gct1.BeginTillEnd();
            gct.BeginTillEnd();

        }
        if (Input.GetKeyUp(KeyCode.Keypad7))
        {
            var gct = gameObject.AddComponent<GaitControlTable>();
            var gct1 = gameObject.AddComponent<GaitControlTable>();
            MR1.modules[25].surfaces["bottom"].Connect(MR.modules[48].surfaces["left"]);
            MR1.modules[20].surfaces["bottom"].Disconnect();
            gct.ReadFromFile(MR, "Test_wheel22_7_GCT.txt");
            gct1.ReadFromFile(MR1, "Legs_5_GCT.txt");
            gct1.BeginTillEnd();
            gct.BeginTillEnd();

        }
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {



            var gct = gameObject.AddComponent<GaitControlTable>();
            var gct1 = gameObject.AddComponent<GaitControlTable>();
            MR1.modules[20].surfaces["bottom"].Connect(MR.modules[46].surfaces["right"]);
            MR1.modules[25].surfaces["bottom"].Disconnect();
            gct.ReadFromFile(MR, "Test_wheel22_8_GCT.txt");

            gct1.ReadFromFile(MR1, "Legs_6_GCT.txt");
            gct1.BeginTillEnd();
            gct.BeginTillEnd();

        }

        if (Input.GetKeyUp(KeyCode.Keypad9))
        {
            Destroy(GameObject.Find("Module 20" + "Base").GetComponent<FixedJoint>());
            GameObject.Find("Module 6" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
            GameObject.Find("Module 36" + "Base").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
            var gct = gameObject.AddComponent<GaitControlTable>();
            var gct1 = gameObject.AddComponent<GaitControlTable>();
            var gct2 = gameObject.AddComponent<GaitControlTable>();
            gct.ReadFromFile(MR, "Test_wheel_1_GCT.txt");
            gct2.ReadFromFile(MR2, "Test_wheel2_1_GCT.txt");
            gct1.ReadFromFile(MR1, "Legs_1_GCT.txt");
            gct1.BeginTillEnd();
            gct2.BeginTillEnd();
            gct.BeginTillEnd();


        }*/
    }
}