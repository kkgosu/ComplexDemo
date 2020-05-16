using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovementNew : Movement
{
    public override IEnumerator MoveBackward(ModularRobot modularRobot)
    {
        isMoving = true;
        GaitControlTable controlTable = modularRobot.gameObject.GetComponent<GaitControlTable>();
        if (controlTable == null)
        {
            controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        }
        while (isMoving)
        {
            controlTable.ReadFromFile(modularRobot, CreateGCTForSnake(modularRobot.angles, 3));
            yield return StartCoroutine(Move(controlTable));
        }
        isMoving = false;
    }

    public override IEnumerator MoveForward(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator MoveLeft(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator MoveRight(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator RotateToTheLeft(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator RotateToTheRight(ModularRobot modularRobot)
    {
        throw new System.NotImplementedException();
    }

    private ModularRobot MR;
    private int lenght = 21;
    private int direction = 1;
    private float speed = 1;
    private int hump = 0;
    private float alpha = 90;
    private float way = 8;
    private bool forward;
    private float lapse = 0.1f;
    private int QuanWaves = 1;
    private bool snake;
    private float X1;
    private float Y1;
    private float X2;
    private float Y2;
    private bool motion;
    private float b;
    private bool next;
    private bool turn;
    private float XX = 0;
    private float YY = 0;
    private float X = 23;
    private float Y = -10 ;
    private readonly float alpha0 = 0;
    private readonly float moduleLength = 0.2766909f;

    public IEnumerator SnakeMotion2(int ID, int len, float a)    //движение Кадочникова
    {
        if (ID >= lenght + len + 1 || ID <= 0 - (len + 2))
        {
            snake = false;
        }
        else
        {
            b = (-2 * a) / (len - 1); ;
            if (ID - direction * (len + 1) >= 0 && ID - direction * (len + 1) < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len + 1), 0, speed));        //волна
            if (ID >= 0 && ID < lenght)
                StartCoroutine(ChangeQ1(ID, a, speed));
            if (ID - direction * len >= 0 && ID - direction * len < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len), a, speed));
            for (int i = 1; i < len; i++)
            {
                if (ID - direction * i >= 0 && ID - direction * i < lenght)
                    StartCoroutine(ChangeQ1(ID - direction * i, b, speed));
            }
            yield return new WaitForSeconds(speed);
            StartCoroutine(SnakeMotion2(ID + direction, len, a));
        }
    }

    private IEnumerator ChangeQ1(int id, float angle, float speed)
    {
        GaitControlTable controlTable = MR.gameObject.GetComponent<GaitControlTable>();

        MR.angles[id] = angle;
        controlTable.ReadFromFile(MR, Movement.CreateGCT(MR.angles, (int)speed));

        yield return StartCoroutine(Move(controlTable));
    }

/*    IEnumerator SnakeMotion1(int ID, int start)    // движение по массиву
    {
        int i = start;
        while (snake)
        {
            StartCoroutine(ChangeQ1(ID, (float)((Math.Asin(Y[Y_Number(i)] - Y[Y_Number(i - direction)]) - Math.Asin(Y[Y_Number(i - direction)] - Y[Y_Number(i - 2 * direction)])) * 57.3), 2f));
            yield return new WaitForSeconds(2f);
            i += direction;
            if (i == Y.Count)
                i = 0;
            if (i == -1)
                i = Y.Count - 1;
        }
    }*/

    public IEnumerator Setup_Wave(float distance)
    {
        way = distance;
        forward = true;
        if (distance < 0)
            direction = -1;
        else
            direction = 1;
        distance /= (float)(moduleLength * 2);
        float da = 1;
        int LQuanWaves = 1;
        float dl = Math.Abs(distance / LQuanWaves);
        int len = 2;
        float a = 6;
        while (false)
        {
            a += da;
            if (a > 91)
            {
                a = 6;
                len++;
                if ((len + 1) > lenght / 4 || len > 3)
                {
                    LQuanWaves++;
                    if (LQuanWaves == 4 && motion == true)
                    {
                        a = 45;
                        LQuanWaves = 3;
                        len = 2;
                        break;
                    }
                    Debug.Log("число волн проверки = " + LQuanWaves);
                    if (LQuanWaves > 1000)
                    {
                        Debug.Log("переполнение ");
                        StopCoroutine(Setup_Wave(distance));
                        break;
                    }
                    dl = Math.Abs(distance / LQuanWaves);
                    Debug.Log("dl = " + dl);
                    len = 2;
                }
                Debug.Log("len проверки = " + len);
            }
        }
        Debug.Log("a = " + a);
        Debug.Log("len = " + len);
        b = (-2 * a) / (len - 1);        //угол в волне        

        Debug.Log("b = " + b);
        double C = 1;                //длинна основания волны
        for (int j = 0; j < len; j++)
            C += Math.Cos((a + b * j) / 57.29577951);
        Debug.Log("C = " + C);

        double b1 = 0;                //верхний угол треугольника
        b1 = Math.Acos((-Math.Pow(C, 2) + Math.Pow(len, 2) + 1) / (2 * (len)));
        //            b1 = Math.Acos ((-Math.Pow (C, 2) + Math.Pow (H+1, 2) + 1) / (2 * (H+1)));
        Debug.Log("b1 (верхний угол треугольника) = " + b1 * 57.29577951);
        b1 = b1 * 57.29577951 - 180;
        if (b1 <= -90 || b1 >= 90)
        {
            Debug.Log("Ошибка. b1 (верхний угол треугольника) недостижим");
            LQuanWaves = 0;
            next = true;
        }

        double b2;        //тупой угол треугольника
        b2 = 57.29577951 * Math.Acos((Math.Pow(len, 2) - Math.Pow(C, 2) - 1) / (-2 * C));
        //            b2 = 57.29577951*Math.Acos ((Math.Pow (H+1, 2) - Math.Pow (C, 2) - 1) / (-2 * C));
        Debug.Log("b2 (тупой угол треугольника) = " + b2);
        if (b2 <= -90 || b2 >= 90)
        {
            Debug.Log("b2 (Ошибка. тупой угол треугольника) недостижим");
            next = true;
            LQuanWaves = 0;
        }

        double b3;        //острый угол треугольника
        b3 = -b2 - b1;
        Debug.Log("b3 (острый угол треугольника) = " + b3);
        if (b3 <= -90 || b3 >= 90)
        {
            Debug.Log("Ошибка. b3 (острый угол треугольника) недостижим");
            next = true;
            LQuanWaves = 0;
        }

        double tr;            //угол в трапеции
        tr = 57.29577951 * Math.Acos((C - len + 1) / 2);
        Debug.Log("tr (угол в трапеции) = " + tr);
        while (LQuanWaves != 0)
        {
            if (next == true)
            {
                if (direction == 1)
                {
                    next = false;
                    StartCoroutine(Wave((len + 1) * 2 - 2, len, b1, b2, b3, tr, a));
                    //StartCoroutine (Wave (len, len, b1, b2, b3, tr, a));
                    LQuanWaves--;
                    QuanWaves = LQuanWaves;
                    snake = true;
                    yield return new WaitForSeconds(speed);
                }
                if (direction == -1)
                {
                    next = false;
                    StartCoroutine(Wave(lenght - ((len + 1) * 2 - 1), len, b1, b2, b3, tr, a));
                    //StartCoroutine (Wave (lenght - (len) - 1, len, b1, b2, b3, tr, a));
                    LQuanWaves--;
                    QuanWaves = LQuanWaves;
                    snake = true;
                    yield return new WaitForSeconds(speed);
                }
            }
            yield return new WaitForSeconds(speed);
        }
        while (!re())
            yield return new WaitForSeconds(speed);
        forward = false;
    }

    public IEnumerator Wave(int ID, int len, double b1, double b2, double b3, double tr, float a) // движение, теоретически без проскальзывания
    {
        StopCoroutine(Wave(ID - direction, len, b1, b2, b3, tr, a));

        if ((ID == (len) * 2 + 2 && direction == 1) || (ID == lenght - ((len + 1) * 2 + 1) && direction == -1))
            next = true;

        if (ID >= lenght + len + 1 || ID <= 0 - (len + 2))
        {
            StopCoroutine(Wave(ID, len, b1, b2, b3, tr, a));
            snake = false;
        }
        else
        {
            b = (-2 * a) / (len - 1);        //угол в волне        
            Debug.Log("ID = " + ID);

            if (ID >= 0 && ID < lenght)
                StartCoroutine(ChangeQ1(ID, (float)b3, speed));        //1-й треугольник
            for (int i = 1; i < len; i++)
            {
                if (ID - direction * i >= 0 && ID - direction * i < lenght)
                    StartCoroutine(ChangeQ1(ID - direction * i, 0, speed));
            }
            if (ID - direction * len >= 0 && ID - direction * len < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len), (float)b1, speed));
            if (ID - direction * (len + 1) >= 0 && ID - direction * (len + 1) < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len + 1), (float)b2, speed));
            yield return new WaitForSeconds(speed);

            if (ID >= 0 && ID < lenght)
                StartCoroutine(ChangeQ1(ID, (float)tr, speed));            //трапеция
            if (ID - direction >= 0 && ID - direction < lenght)
                StartCoroutine(ChangeQ1(ID - direction, -(float)tr, speed));
            for (int i = 2; i < len; i++)
            {
                if (ID - direction * i >= 0 && ID - direction * i < lenght)
                    StartCoroutine(ChangeQ1(ID - direction * i, 0, speed));
            }
            if (ID - direction * len >= 0 && ID - direction * len < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len), -(float)tr, speed));
            if (ID - direction * (len + 1) >= 0 && ID - direction * (len + 1) < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len + 1), (float)tr, speed));
            yield return new WaitForSeconds(speed);

            if (ID >= 0 && ID < lenght)
                StartCoroutine(ChangeQ1(ID, (float)b2, speed));        //2-й треугольник
            if (ID - direction >= 0 && ID - direction < lenght)
                StartCoroutine(ChangeQ1(ID - direction, (float)b1, speed));
            for (int i = 2; i < len + 1; i++)
            {
                if (ID - direction * i >= 0 && ID - direction * i < lenght)
                    StartCoroutine(ChangeQ1(ID - direction * i, 0, speed));
            }
            if (ID - direction * (len + 1) >= 0 && ID - direction * (len + 1) < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len + 1), (float)b3, speed));
            yield return new WaitForSeconds(speed);

            if (ID - direction * (len + 1) >= 0 && ID - direction * (len + 1) < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len + 1), 0, speed));        //волна
            if (ID >= 0 && ID < lenght)
                StartCoroutine(ChangeQ1(ID, a, speed));
            if (ID - direction * len >= 0 && ID - direction * len < lenght)
                StartCoroutine(ChangeQ1(ID - direction * (len), a, speed));
            for (int i = 1; i < len; i++)
            {
                if (ID - direction * i >= 0 && ID - direction * i < lenght)
                    StartCoroutine(ChangeQ1(ID - direction * i, b, speed));
            }
            yield return new WaitForSeconds(speed);

            yield return StartCoroutine(Wave(ID + direction, len, b1, b2, b3, tr, a));
        }
        StopCoroutine(Wave(ID, len, b1, b2, b3, tr, a));
    }

    IEnumerator Fold(float a)            //божественный поворот через мостик
    {
        int middle = (int)(lenght / 2);

        StartCoroutine(ChangeQ1(middle - 1, 10, speed));
        StartCoroutine(ChangeQ1(middle + 1, 10, speed));
        yield return new WaitForSeconds(speed);

        StartCoroutine(ChangeQ1(middle - 2, a, speed));
        StartCoroutine(ChangeQ1(middle + 2, -a, speed));
        StartCoroutine(ChangeQ1(middle - 4, -a / 2, speed));
        StartCoroutine(ChangeQ1(middle + 4, a / 2, speed));
        yield return new WaitForSeconds(speed);

        StartCoroutine(ChangeQ1(middle - 1, 0, speed));
        StartCoroutine(ChangeQ1(middle + 1, 0, speed));
        yield return new WaitForSeconds(speed);

        float b = a / 2;
        float x = 1 + 2 * Mathf.Cos((float)(b / 57.29577951)) + Mathf.Cos((float)((b - a) / 57.29577951));
        Debug.Log(3);
        for (float i = (float)a; Math.Abs(i) <= Math.Abs(a); i -= (float)(a / 2/*Math.Abs(a)*/))
        {

            //Debug.Log (2);
            float a1 = (float)(Mathf.Acos(x / (1 + 2 * Mathf.Cos((float)((i / 2) / 57.29577951)) + Mathf.Cos((float)((i / 2 - i) / 57.29577951)))) * 57.29577951);
            StartCoroutine(ChangeQ1(middle - 2, i, speed));//(Mathf.Abs((int)a*2))));
            StartCoroutine(ChangeQ1(middle + 2, -i, speed));//(Mathf.Abs((int)a*2))));

            StartCoroutine(ChangeQ1(middle - 4, -i / 2, speed));//(Mathf.Abs((int)a*2))));
            StartCoroutine(ChangeQ1(middle + 4, i / 2, speed));//(Mathf.Abs((int)a*2))));

            StartCoroutine(ChangeQ1(middle - 1, -a1, speed));//(Mathf.Abs((int)a*2))));
            StartCoroutine(ChangeQ1(middle + 1, -a1, speed));//(Mathf.Abs((int)a*2))));

            StartCoroutine(ChangeQ1(middle - 5, a1, speed));//(Mathf.Abs((int)a*2))));
            StartCoroutine(ChangeQ1(middle + 5, a1, speed));//(Mathf.Abs((int)a*2))));
            yield return new WaitForSeconds(speed);//(Mathf.Abs((int)a*2)));
        }

        StartCoroutine(ChangeQ1(middle - 1, 10, speed));
        StartCoroutine(ChangeQ1(middle + 1, 10, speed));
        yield return new WaitForSeconds(speed);

        StartCoroutine(ChangeQ1(middle - 2, 0, speed));
        StartCoroutine(ChangeQ1(middle + 2, 0, speed));
        StartCoroutine(ChangeQ1(middle - 4, 0, speed));
        StartCoroutine(ChangeQ1(middle + 4, 0, speed));
        yield return new WaitForSeconds(speed);

        StartCoroutine(ChangeQ1(middle - 1, 0, speed));
        StartCoroutine(ChangeQ1(middle + 1, 0, speed));
        yield return new WaitForSeconds(speed);
    }


    public IEnumerator Motion(float X1, float Y1, bool side)
    {
        motion = true;
        int middle = (int)(lenght / 2);
        float XX1 = GameObject.Find("Module " + (0)).transform.Find("Base").transform.position.x;
        float YY1 = GameObject.Find("Module " + (0)).transform.Find("Base").transform.position.z;
        float X0 = GameObject.Find("Module " + middle).transform.Find("Base").transform.position.x;
        float Y0 = GameObject.Find("Module " + middle).transform.Find("Base").transform.position.z;

        while ((side && (Mathf.Sqrt(Mathf.Pow(X1 - XX, 2) + Mathf.Pow(Y1 - YY, 2))) > 0.05) || (!side && (Mathf.Sqrt(Mathf.Pow(X1 - XX1, 2) + Mathf.Pow(Y1 - YY1, 2))) > 0.05))
        {
            float alphaf;

            way = Mathf.Sqrt(Mathf.Pow(X1 - X0, 2) + Mathf.Pow(Y1 - Y0, 2));
            alphaf = (float)(57.3 * Mathf.Acos((Y1 - Y0) / way) * Mathf.Asin((X1 - X0) / way) / Mathf.Abs(Mathf.Asin((X1 - X0) / way)));
            if (!side)
            {
                if (alphaf >= 0)
                    alphaf -= 180;
                else
                    alphaf += 180;
            }
            Debug.Log("alpfaf = " + alphaf);
            StartCoroutine(Turn(alphaf));
            yield return new WaitForSeconds(0.01f);
            while (turn)
                yield return new WaitForSeconds(speed);
            X0 = GameObject.Find("Module " + middle).transform.Find("Base").transform.position.x;
            Y0 = GameObject.Find("Module " + middle).transform.Find("Base").transform.position.z;
            if (!side)
            {
                XX1 = GameObject.Find("Module " + (0)).transform.Find("Base").transform.Find("Analysis").transform.position.x;
                YY1 = GameObject.Find("Module " + (0)).transform.Find("Base").transform.Find("Analysis").transform.position.z;
                way = Mathf.Sqrt(Mathf.Pow(X1 - XX1, 2) + Mathf.Pow(Y1 - YY1, 2));
                way *= -1;
                if (((X0 - X1) < 0 && (XX1 - X1) > 0) || ((X0 - X1) > 0 && (XX1 - X1) < 0))
                    way *= -1;
            }
            else
            {
                way = Mathf.Sqrt(Mathf.Pow(X1 - XX, 2) + Mathf.Pow(Y1 - YY, 2));
                if (((X0 - X1) < 0 && (XX - X1) > 0) || ((X0 - X1) > 0 && (XX - X1) < 0))
                    way *= -1;
            }
            StartCoroutine(Setup_Wave(way));
            while (forward)
                yield return new WaitForSeconds(speed);
            XX1 = GameObject.Find("Module " + (0)).transform.Find("Base").transform.position.x;
            YY1 = GameObject.Find("Module " + (0)).transform.Find("Base").transform.position.z;
            X0 = GameObject.Find("Module " + middle).transform.Find("Base").transform.position.x;
            Y0 = GameObject.Find("Module " + middle).transform.Find("Base").transform.position.z;
        }
        while (!re())
        {
            yield return new WaitForSeconds(speed);
        }
        motion = false;

    }

    private bool re()
    {
        return (GetComponent<GaitControlTable>().inProgress || !GetComponent<GaitControlTable>().isReady);
    }

    public IEnumerator Turn(float alphaf)
    {
        turn = true;
        alpha = alphaf - alpha0;
        if (alpha > 180)
            alpha -= 360;

        while (Mathf.Abs(alpha) > 0.5)
        {
            //Debug.Log ("1");
            if (alpha >= 50)
                StartCoroutine(Fold(50));
            else if (alpha <= -50)
                StartCoroutine(Fold(-50));
            else
                StartCoroutine(Fold(alpha + 1 * (alpha / Math.Abs(alpha))));
            yield return new WaitForSeconds(speed * 12);
            alpha = alphaf - alpha0;
        }
        turn = false;
    }
/*    IEnumerator doc()
    {
        int middle1 = (int)GameObject.Find("robotEnv").transform.GetComponent<Test>().lenght / 2;
        int middle2 = (int)GameObject.Find("robotEnv2").transform.GetComponent<Test2>().lenght / 2;

        X1 = GameObject.Find("Module " + middle1).transform.Find("Base").transform.position.x;
        Y1 = GameObject.Find("Module " + middle1).transform.Find("Base").transform.position.z;
        X2 = GameObject.Find("Moduler " + middle2).transform.Find("Base").transform.position.x;
        Y2 = GameObject.Find("Moduler " + middle2).transform.Find("Base").transform.position.z;

        float way = Mathf.Sqrt(Mathf.Pow(X1 - X2, 2) + Mathf.Pow(Y1 - Y2, 2));

        X = (X1 + X2) / 2;
        Y = (Y1 + Y2) / 2;
        double alphaf = (57.3 * Mathf.Acos((Y2 - Y1) / way) * Mathf.Asin((X2 - X1) / way) / Mathf.Abs(Mathf.Asin((X2 - X1) / way)));

        StartCoroutine(GameObject.Find("robotEnv").transform.GetComponent<Test>().Motion((float)(X + 0.22 * Math.Cos(alphaf)), (float)(Y + 0.22 * Math.Sin(alphaf)), true));
        StartCoroutine(GameObject.Find("robotEnv2").transform.GetComponent<Test2>().Motion((float)(X - 0.22 * Math.Cos(alphaf)), (float)(Y - 0.22 * Math.Sin(alphaf)), false));

        while (GameObject.Find("robotEnv").transform.GetComponent<Test>().motion || GameObject.Find("robotEnv2").transform.GetComponent<Test2>().motion)
        {
            yield return new WaitForSeconds(1f);
        }
        Debug.Log("dock");

        X1 = GameObject.Find("Module " + middle1).transform.Find("Base").transform.position.x;
        Y1 = GameObject.Find("Module " + middle1).transform.Find("Base").transform.position.z;
        X2 = GameObject.Find("Moduler " + middle2).transform.Find("Base").transform.position.x;
        Y2 = GameObject.Find("Moduler " + middle2).transform.Find("Base").transform.position.z;
        way = Mathf.Sqrt(Mathf.Pow(X1 - X2, 2) + Mathf.Pow(Y1 - Y2, 2));
        alphaf = 57.3 * Mathf.Acos((Y2 - Y1) / way) * Mathf.Asin((X2 - X1) / way) / Mathf.Abs(Mathf.Asin((X2 - X1) / way));

        StartCoroutine(GameObject.Find("robotEnv").transform.GetComponent<Test>().Turn((float)(alphaf + 0.8)));
        StartCoroutine(GameObject.Find("robotEnv2").transform.GetComponent<Test2>().Turn((float)(alphaf + 0.8)));
        while (GameObject.Find("robotEnv").transform.GetComponent<Test>().turn || GameObject.Find("robotEnv2").transform.GetComponent<Test2>().turn)
            yield return new WaitForSeconds(1f);

        double XX1 = GameObject.Find("Moduler " + (0)).transform.Find("Base").transform.Find("Analysis").transform.position.x;
        double YY1 = GameObject.Find("Moduler " + (0)).transform.Find("Base").transform.Find("Analysis").transform.position.z;
        double XX = GameObject.Find("Module " + (GameObject.Find("robotEnv").transform.GetComponent<Test>().lenght - 1)).transform.Find("Turning Segment").transform.Find("Rotation Segment").transform.Find("Top Face").transform.position.x;
        double YY = GameObject.Find("Module " + (GameObject.Find("robotEnv").transform.GetComponent<Test>().lenght - 1)).transform.Find("Turning Segment").transform.Find("Rotation Segment").transform.Find("Top Face").transform.position.z;
        way = (float)(Math.Sqrt(Math.Pow(XX1 - XX, 2) + Math.Pow(YY - YY1, 2)) - 0.15);
        StartCoroutine(GameObject.Find("robotEnv").transform.GetComponent<Test>().Dock(way));
        yield return false;
    }*/


    // Start is called before the first frame update
    void Start()
    {
        MR = GetComponent<ModularRobot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
