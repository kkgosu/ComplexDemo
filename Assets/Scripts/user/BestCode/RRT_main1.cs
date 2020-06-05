using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.UI;
using System.Threading;

public class RRT_main1 : MonoBehaviour
{
    public GameObject SphereFinalPoint;
    public GameObject SphereFinalNapr;
    public GameObject SphereBarrier;
    public FixedJoint joint12;
    public GameObject CubeTatget;
    public GameObject CubeTatgetHard;
    public GameObject CubeLine;
    float LenghtModule = 0.27669f;
    Vector3 finalPoint = new Vector3(0.9f, 0.26f, -0.75f); //конечная точка(задаём её)
    Vector3 finalNapr = new Vector3(0.6f, 0.26f, -0.75f);//направляющая точка(задаём её)
    Vector3 SecondFinalPoint = new Vector3(0, 0, 0);// расчитывается в findNaprPoint
    Vector3 barrierPoint = new Vector3(0.4f, 0.9f, -0.1f);//центр препятствия
    static public int numbMod=8;//кол-во мод которые будет исп


    public List<Module> modules = new List<Module>();
    public List<localRot> localRotations = new List<localRot>();

    public List<NodeTree> Tree = new List<NodeTree>();
    public List<NodeTree> BestNodeList = new List<NodeTree>();
    public List<connectoinNode> ConForTree = new List<connectoinNode>();
    public class NodeTree//узлы дерева
    {
        public float[] NodeAngels = new float[(numbMod-1)*2];
        public float fit;
        public float fit2;
        public int id;
        public NodeTree(float[] arr, float f, float f2, int i)
        {
            NodeAngels = (float[])arr.Clone();
            fit = f;
            fit2 = f2;
            id = i;
        }

    }
    public class connectoinNode//соединения узлов
    {
        public int id1;
        public int id2;
        public float costMot;

        public connectoinNode(int id_1, int id_2, float ad)
        {
            id1 = id_1;
            id2 = id_2;
            costMot = ad;
        }

    }


    public GameObject CubeTest;
    public class localRot
    {
        public Quaternion rot;
        public int st;
        public localRot(Quaternion q, int a)
        {
            rot = q;
            st = a;
        }
    }

    public void button_test2()//Основная
    {

            //Destroy(GameObject.Find("CubeTest(Clone)"));
        Tree.Clear();
        BestNodeList.Clear();
        ConForTree.Clear();
        growTree();

        //finalPoint = new Vector3(-0.8f, 0.21f, 0);
        //finalNapr = new Vector3(-0.8f, 0.21f, -0.2f);
        //SecondFinalPoint = findNaprPoint(finalPoint, finalNapr);

        //FixedJoint joint1 = gameObject.AddComponent<FixedJoint>();
        //Joint fixedJoint1 = GameObject.Find("CubeTarg").gameObject.AddComponent<FixedJoint>();
      // fixedJoint1.connectedBody = GameObject.Find("sdsd").gameObject.AddComponent<FixedJoint>();

    }

    public void button_test()//Для того чтобы шаманить
    {
        finalPoint = new Vector3(1.0f, 0.167f, -0.25f);
        finalNapr = new Vector3(1.0f, 0.167f, -0.5888f);
        SecondFinalPoint = findNaprPoint(finalPoint, finalNapr);
        SphereFinalPoint.transform.position = finalPoint;
        //CubeLine.transform.position = finalPoint;
        SphereFinalNapr.transform.position = finalNapr;
        Tree.Clear();
        BestNodeList.Clear();
        ConForTree.Clear();
        growTree();
    }
    void testPosCubes(Vector3[] pos)//тест для пзк
    {
        for (int i = 0; i < pos.Length; i++)
        {
            Instantiate(CubeTest, pos[i], Quaternion.identity);
        }
    }


    public Vector3[] quatGen2(float[] angl) // РАСЧЁТ ПЗК
    {
        Vector3[] arr = new Vector3[modules.Count];
        Vector3 rrr = new Vector3(0, 0.27669f, 0);
        Vector3 rrr2 = new Vector3(0, 0, 0);
        arr[0] = rrr2;
        Quaternion QuatQ1 = Quaternion.Euler(0, 0, 0);
        Quaternion QuatQ2 = Quaternion.Euler(0, 0, 0);
        Quaternion buff = Quaternion.Euler(0, 0, 0);
        for (int i = 1; i < modules.Count; i++)
        {
           
            rrr = new Vector3(0, 0.27669f, 0);
            /*if (i == modules.Count - 1)
            {
                rrr = new Vector3(0, 0.27669f+0.14f, 0);
            }*/
            QuatQ1 = Quaternion.Euler(0, 0,angl[(i * 2) - 2]);
            if (i != 1)
            {
                QuatQ2 = Quaternion.Euler(0, -angl[(i * 2) - 3] , 0);
                buff = buff * QuatQ2 * localRotations[i - 1].rot * QuatQ1;
            }
            if (i == 1)
            {
                buff = buff * localRotations[i - 1].rot * QuatQ1;
                rrr = buff * rrr;
                //Vector3 qwa = new Vector3(180, 0, 0);
                //Quaternion qwa2 = Quaternion.Euler(qwa);
                rrr2 = rrr;
            }
            else
            {
                rrr = buff * rrr;
                rrr = rrr + rrr2;
                rrr2 = rrr;
            }
            arr[i] = rrr;
            //Debug.Log ("Q  "+buff);
        }
        //Debug.Log("Q  " + buff);
        //Cube2.transform.position = buff * Cube2.transform.position;
        return arr;
    }

    localRot makeRotQuat(Module mod1, float tiltQuat)//Задаём локальные повороты. Для того чтобы расчитывать ПЗК для УСЛОВНО произвольной конфигурации.
    {
        int a1 = 1;
        Quaternion aaa = new Quaternion();
        string conTag = "";
        if (mod1.surfaces["top"].connectedSurface != null)
            conTag = "top";
        if (mod1.surfaces["bottom"].connectedSurface != null)
            conTag = "bottom";
        if (mod1.surfaces["left"].connectedSurface != null)
            conTag = "left";
        if (mod1.surfaces["right"].connectedSurface != null)
            conTag = "right";

        switch(conTag)
        {
            case "top": aaa = Quaternion.Euler(0, 0, 0); aaa *= Quaternion.Euler(0, -tiltQuat, 0); a1 = -1;
                break;
            case "bottom": aaa = Quaternion.Euler(0, 0, 0); aaa *= Quaternion.Euler(0, -tiltQuat, 0);
                break;
            case "left": aaa = Quaternion.Euler(0, -tiltQuat, 0); aaa *= Quaternion.Euler(-90, 0, 0); 
                break;
            case "right": aaa = Quaternion.Euler(0, -tiltQuat, 0); aaa *= Quaternion.Euler(90, 0, 0); 
                break;
            default: aaa = Quaternion.Euler(0, 0, 0); break; 

        }
        Debug.Log(aaa);
        localRot aaa1 = new localRot(aaa, a1);
        return aaa1;
    }
    public void init(int[] modulesId = null)
    {
        Vector3 vec_start_rotation = new Vector3(0, 0, 0);
        float tiltQuat = 0;
        Joint fixedJoint = GameObject.Find("Module 0/Base").gameObject.AddComponent<FixedJoint>();
        localRot a = new localRot(Quaternion.Euler(vec_start_rotation), 1);
        localRotations.Add(a);//список локальных ориентаций
        if (modulesId != null)
        {
            ModularRobot MR = GetComponent<ModularRobot>();
            for (int i = 0; i < modulesId.Length; i++)
            {
                modules.Add(MR.modules[i]);
            }
        } else
        {
            modules.Add(Modules.Create(Modules.M3R2, new Vector3(0, 0, 0), Quaternion.Euler(vec_start_rotation), new float[] { 0 }, parent: transform, ID: 0));
            tiltQuat = 90;
            modules.Add(modules[0].surfaces["top"].Add(Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] { 0 }, tilt: tiltQuat, parent: transform, ID: 1));
            localRotations.Add(makeRotQuat(modules[1], tiltQuat)); //дополняем список после создания нового модуля
            tiltQuat = 90;
            modules.Add(modules[1].surfaces["top"].Add(Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] { 0 }, tilt: tiltQuat, parent: transform, ID: 2));
            localRotations.Add(makeRotQuat(modules[2], tiltQuat));
            tiltQuat = 90;
            modules.Add(modules[2].surfaces["top"].Add(Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] { 0 }, tilt: tiltQuat, parent: transform, ID: 3));
            localRotations.Add(makeRotQuat(modules[3], tiltQuat));
            tiltQuat = 90;
            modules.Add(modules[3].surfaces["top"].Add(Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] { 0 }, tilt: tiltQuat, parent: transform, ID: 4));
            localRotations.Add(makeRotQuat(modules[4], tiltQuat));
            tiltQuat = 90;
            modules.Add(modules[4].surfaces["top"].Add(Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] { 0 }, tilt: tiltQuat, parent: transform, ID: 5));
            localRotations.Add(makeRotQuat(modules[5], tiltQuat));
            tiltQuat = 90;
            modules.Add(modules[5].surfaces["top"].Add(Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] { 0 }, tilt: tiltQuat, parent: transform, ID: 6));
            localRotations.Add(makeRotQuat(modules[6], tiltQuat));
            tiltQuat = 0;
            modules.Add(modules[6].surfaces["top"].Add(Modules.M3R2, Modules.M3R2.surfaces["bottom"], new float[] { 0 }, tilt: tiltQuat, parent: transform, ID: 7));
            localRotations.Add(makeRotQuat(modules[7], tiltQuat));
        }

        Debug.Log("!1!1 " + Vector3.Distance(SecondFinalPoint, CubeTatgetHard.transform.position));

        SecondFinalPoint = findNaprPoint(finalPoint, finalNapr);//расчёт нужного положения пердпоследнего модуля
        for (int i = 0; i < modules.Count; i++)//отключаю силу притяжения, мб так делать не надо
        {
            GameObject.Find("Module " + i + "/Base").gameObject.GetComponent<Rigidbody>().useGravity = false;
            GameObject.Find("Module " + i + "/Turning Segment/Static Segment").gameObject.GetComponent<Rigidbody>().useGravity = false;
            GameObject.Find("Module " + i + "/Turning Segment/Rotation Segment").gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        SphereFinalPoint.transform.position = finalPoint;
        SphereFinalNapr.transform.position = finalNapr;
        SphereBarrier.transform.position = barrierPoint;
    }

    // Start is called before the first frame update
    void Start()
    {

        init();
        //CubeLine.transform.position = finalPoint;



        //Vector3 qw = new Vector3(1.0f, 0.167f, -0.25f);
        //Vector3 qw2 = new Vector3(1.0f, 0.167f, -0.45f);
        //SecondFinalPoint = findNaprPoint(finalPoint, finalNapr);
        //SphereFinalPoint.transform.position = qw;
        //CubeLine.transform.position = qw2;
        //CubeLine.transform.position.x = CubeLine.transform.position.x - 0.1f;

    }


    Vector3 findNaprPoint(Vector3 fin,Vector3 finNap)//Находим точку для задания ориентации. Включается только 1 раз.
    {
         Vector3 SecFinPoint = new Vector3(0,0,0);
         Vector3 BuffVec = new Vector3(finNap.x - fin.x, finNap.y - fin.y, finNap.z - fin.z);
         BuffVec.Normalize();
         SecFinPoint.x = (BuffVec.x * LenghtModule) + fin.x;
         SecFinPoint.y = (BuffVec.y * LenghtModule) + fin.y;
         SecFinPoint.z = (BuffVec.z * LenghtModule) + fin.z;
        return SecFinPoint;
    }
    float fittnesFin(float[] popAng)//Фитнесс функция для конца робота. Расчёт расстояния между последним модулем и целевой точкой
    {
        Vector3 lastMod = new Vector3(0, 0, 0);
        lastMod = quatGen2(popAng)[modules.Count-1];
        lastMod.x = (float)Math.Pow((lastMod.x - finalPoint.x), 2);
        lastMod.y = (float)Math.Pow((lastMod.y - finalPoint.y), 2);
        lastMod.z = (float)Math.Pow((lastMod.z - finalPoint.z), 2);
        float result = (float)Math.Sqrt(lastMod.x + lastMod.y + lastMod.z);
        //Debug.Log("F " + result);
        return result;
    }
    float fittnesSecondFin(float[] popAng)//Фитнесс функция для точки ориентации. Расчёт расстояния между предпоследним модулем и точкой ориентации найденной в findNaprPoint
    {
        Vector3 lastMod = new Vector3(0, 0, 0);
        lastMod = quatGen2(popAng)[modules.Count - 2];
        lastMod.x = (float)Math.Pow((lastMod.x - SecondFinalPoint.x), 2);
        lastMod.y = (float)Math.Pow((lastMod.y - SecondFinalPoint.y), 2);
        lastMod.z = (float)Math.Pow((lastMod.z - SecondFinalPoint.z), 2);
        float result = (float)Math.Sqrt(lastMod.x + lastMod.y + lastMod.z);
        //Debug.Log("F " + result);
        return result;
    }
    void growTree()//основная функция
    {
        if (Tree.Count > 3)
        {
            Tree.Clear();
            ConForTree.Clear();
            BestNodeList.Clear();

        }

        System.Random rand = new System.Random();
        bool tagGrowing = true;
        int shetchik = 0;
        float mainFit = 0;
        float[] arrAng = new float[(modules.Count - 1) * 2];//Создаём первый узел, согласно текущему положению робота.!!До метки ЫЫЫ
        for (int i = 0; i < modules.Count-1; i++)
        {
            arrAng[i*2] = modules[i].drivers["q1"].qValue;
            arrAng[(i*2)+1] = modules[i].drivers["q2"].qValue;
        }
        NodeTree firstNode = new NodeTree(arrAng, fittnesFin(arrAng), fittnesSecondFin(arrAng), 0);
        Tree.Add(firstNode);// !!ЫЫЫЫ
        BestNodeList.Add(firstNode);
       
        while (tagGrowing == true)
        {
            shetchik++;
            //Вот тут надо выбрать узел,от которого происходит рост
            int randBestNode1 = 0;
            randBestNode1 = rand.Next(0,BestNodeList.Count);
            float[] angls = new float[BestNodeList[randBestNode1].NodeAngels.Length];
            for (int mk = 0; mk < BestNodeList[randBestNode1].NodeAngels.Length; mk++)
            {
                angls[mk] = BestNodeList[randBestNode1].NodeAngels[mk];
            }
            //Вот мы выбрали
            //Теперь формируем новый узел( я знаю это  уёбиищно))))
            int qwqwqwqw = rand.Next(1, 4);
            for (int asas = 0; asas < qwqwqwqw; asas++)
            {

                int modNum = rand.Next(modules.Count - 1);
                int modQNum = rand.Next(2);
                int modQ = rand.Next(10);
                int znak = rand.Next(2);

                if (znak == 0)
                {
                    znak = -1;
                }
                else
                {
                    znak = 1;
                }

                angls[(modNum * 2) + modQNum] += (modQ * znak);
                if (angls[(modNum * 2) + modQNum] > 90)
                    angls[(modNum * 2) + modQNum] = 90;

                if (angls[(modNum * 2) + modQNum] < -90)
                    angls[(modNum * 2) + modQNum] = -90;
            }//Закончили формировать новые углы
            // Сейчас будет проверка на приближение нового набора углов(по концу и предыдущему модулю),а так же проверка на возможность движения
            if (((BestNodeList[randBestNode1].fit >= fittnesFin(angls)) || (BestNodeList[randBestNode1].fit2 >= fittnesSecondFin(angls))) && (CheckMotion(BestNodeList[randBestNode1].NodeAngels,angls,7)))
            {
                //((BestNodeList[randBestNode1].fit >= fittnesFin(angls)) || (BestNodeList[randBestNode1].fit2 >= fittnesSecondFin(angls)))

                //Если прошли проверку, то создаём новый узел и включаем его в список
                NodeTree NewNode = new NodeTree(angls, fittnesFin(angls), fittnesSecondFin(angls), Tree.Count);
                float q45 = raschCost((BestNodeList[randBestNode1].NodeAngels), angls);
                connectoinNode NewCon = new connectoinNode(BestNodeList[randBestNode1].id, NewNode.id, q45);//добавить стоимость
                Tree.Add(NewNode);
                ConForTree.Add(NewCon);
                //BestNodeList.OrderBy(NodeTree => NodeTree.fit).ThenBy(NodeTree => NodeTree.fit2).ToList();

                for (int h = 0; h < BestNodeList.Count; h++)//Обновляем список лучщих узлов. Тут их 9,можно увеличить. 
                {
                    //Debug.Log("Q3"); if ((NewNode.fit < BestNodeList[h].fit) && (NewNode.fit2 < BestNodeList[h].fit2))
                    if ((NewNode.fit + NewNode.fit2) * 0.5 < (BestNodeList[h].fit + BestNodeList[h].fit2) * 0.5)
                    {
                        BestNodeList.Add(NewNode);
                        
                    }
                    if (BestNodeList.Count > 9)
                    {
                        BestNodeList.Remove(BestNodeList[h]);
                        
                    }
                    h = 11;

                }

                if ((NewNode.fit < 0.033f) && (NewNode.fit2 < 0.033)) // Проверяем новую конфигурацию на соответствие ответу. Данная версия плохо рабоатет с точночтью ниже 0.033
                {
                    tagGrowing = false;
                    Debug.Log("!!!Fit1 " + NewNode.fit);
                    Debug.Log("!!!Fit2 " + NewNode.fit2);
                    Debug.Log("Numb_pop " + shetchik);
                    Debug.Log("Kol_Nodes " + Tree.Count);
                    //motionRob (ab2.NodeAngels);
                    Debug.Log("KolCON " + ConForTree.Count);
                    //Если новая конф соответствует ответу, то формируем матрицу и ищем кратчайший путь
                    float[,] matr_conn = new float[Tree.Count, Tree.Count];
                    matr_conn = makeMatr();
                    int[] seq = alg_deykstra(matr_conn, Tree.Count - 1);
                    int SeqCount = 0;
                    for (int g = 0; g < seq.Length; g++)
                    {
                        if (seq[g] != 0)
                        {
                            SeqCount++;
                        }
                    }
                    int[] seq2 = new int[SeqCount];
                    for (int y = 0; y < seq2.Length; y++)
                    {
                        seq2[y] = seq[y];
                    }
                    Debug.Log(string.Join(" ", seq2.Select(x => x.ToString()).ToArray()));
                    //int[] seqOPT = optimizatoin(seq2);
                    //Debug.Log(string.Join("! ", seqOPT.Select(x => x.ToString()).ToArray()));
                    //seq.Reverse();
                    //mot1(NewNode.NodeAngels);
                    /*for (int l = 0; l < seq.Length; l++)
                    {
                        mot1(Tree[seq[l]].NodeAngels);
                    }*/

                    var gct = gameObject.AddComponent<GaitControlTable>(); //Двигаемся по полученной последовательности конфигураций
                    var drvs = new List<Driver>();
                    foreach (Module m in modules)
                    {
                        drvs.Add(m.drivers["q1"]);
                        drvs.Add(m.drivers["q2"]);
                    }
                    gct.SetHeader(drvs.ToArray());
                    foreach (int index in seq2.Reverse())
                        gct.AddLine(string.Join(",", Tree[index].NodeAngels.Select(f => f.ToString("0.00")).ToArray()));
                    gct.Begin();

                    //mot1(NewNode.NodeAngels);

                }


            }





            if (shetchik > 13000) // Выход если не нашли за Н итераций  
            {
                tagGrowing = false;
                Debug.Log("NE NASHEL!!");
                Debug.Log("Numb_pop " + shetchik);
                Debug.Log("Numb_NOD " + Tree.Count);
            }

        }//whileFIN


    }
    public int[] optimizatoin(int[] arr)//Эта хуйня не рабоатет. Будет рабоать в финальной версии.
    {
        List<int> newArr = new List<int>();
        arr = Enumerable.Range(1, arr.Length).Reverse().ToArray();
        newArr.Add(arr[0]);
        for (int i = 0; i < arr.Length; i++)
        {
            for (int J = i+1; J < arr.Length-1; J++)
            {
                if ((CheckMotion(Tree[arr[i]].NodeAngels, Tree[arr[J+1]].NodeAngels, 8)==false)&&(CheckMotion(Tree[arr[i]].NodeAngels, Tree[arr[J]].NodeAngels, 8)==true))
                {
                    newArr.Add(arr[J]);
                    i = J;
                    J = arr.Length + 10;
                    Debug.Log("SS "+newArr[newArr.Count]);
                }

            }


        }
        int[] a = new int[newArr.Count];
        for (int g = 0; g < newArr.Count; g++)
        {
            a[g] = newArr[g];
        }
        return a;
    }

    public float[,] makeMatr() //Создание матрицы согласно всем узлам и соединениям дерева
    {
        float[,] arr = new float[Tree.Count, Tree.Count];
        for (int i = 0; i < ConForTree.Count; i++)
        {
            arr[ConForTree[i].id1, ConForTree[i].id2] = ConForTree[i].costMot;
            arr[ConForTree[i].id2, ConForTree[i].id1] = ConForTree[i].costMot;
        }

        return arr;
    }
    public float raschCost(float[] a, float[] b)//Рассчёт стоимости пути между конфигурациями по углу
    {
        float result = 0f;

        for (int i = 0; i < a.Length; i++)
        {
            float r = b[i] - a[i];
            //Debug.Log ("!r " + r);
            if (r < 0)
                r = r * (-1);
            result += r;

        }
        //Debug.Log (result);
        if (result == 0f)
            result = 2f;

        return result;
    }

    public float raschCost2(float[] a, float[] b)//Рассчёт стоимости пути между конфигурациями по расстоянию (не рабоатет и не используется)
    {
        float result = 0f;
        Vector3[] arr1 = new Vector3[numbMod];
        Vector3[] arr2 = new Vector3[numbMod];
        arr1 = quatGen2(a);
        arr2 = quatGen2(b);
        //arr1[numbMod].x
        Vector3 lastMod = new Vector3(0, 0, 0);
        //lastMod = quatGen2(popAng)[modules.Count - 1];
        lastMod.x = (float)Math.Pow((arr1[numbMod-1].x - arr2[numbMod-1].x), 2);
        lastMod.y = (float)Math.Pow((arr1[numbMod-1].y - arr2[numbMod-1].y), 2);
        lastMod.z = (float)Math.Pow((arr1[numbMod-1].z - arr2[numbMod-1].z), 2);
         result = (float)Math.Sqrt(lastMod.x + lastMod.y + lastMod.z);
        return result;
    }

    bool CheckMotion(float[] currentConf, float[] futureConf, int diskr) //Проверка на возможность движения из currentConf в futureCon. Работвает только для 1 препятствия barrierPoint
    {
        bool q = true;
        Vector3[] currPos = new Vector3[modules.Count];
        Vector3[] futPos = new Vector3[modules.Count];
        float[] intermediateConf = new float[(modules.Count - 1) * 2];
        float[] currentConfCOPY = new float[(modules.Count - 1) * 2];
        //currPos = quatGen2(currentConf);
        futPos = quatGen2(futureConf);
        currentConfCOPY = (float[])currentConf.Clone();
        for (int i = 0; i < currentConf.Length; i++)
        {
            intermediateConf[i] = (futureConf[i] - currentConfCOPY[i]) * (1f / diskr);
        }

        for (int i = 0; i < diskr; i++)
        {
            for (int k = 0; k < currentConfCOPY.Length; k++)
            {
                currentConfCOPY[k] += intermediateConf[k];
            }

            currPos = quatGen2(currentConfCOPY);
            for (int w = 1; w < modules.Count; w++)
            {
                barrierPoint = CubeTatgetHard.gameObject.GetComponent<Collider>().ClosestPoint(currPos[w]); //Ближайшая точка на коллайдере препятствия. Если менять препятствие то делать это тут.
                Vector3 lastMod = new Vector3(0, 0, 0);
                lastMod.x = (float)Math.Pow((currPos[w].x - barrierPoint.x), 2);
                lastMod.y = (float)Math.Pow((currPos[w].y - barrierPoint.y), 2);
                lastMod.z = (float)Math.Pow((currPos[w].z - barrierPoint.z), 2);
                float rassogl = (float)Math.Sqrt(lastMod.x + lastMod.y + lastMod.z);
                rassogl = rassogl - (0.165f);//Вот тут можно сденлать нормально. В данной версии модуль апроксимируется шаром. Размер примерно подогнан,но будет задевать с углов.
                if (rassogl < 0)
                {
                    q = false;
                    return q;
                }

            }

        }

        return q;
    }
    public int[] alg_deykstra(float[,] a, int end) // Спизженый хз откуда алгоритм дейкстры
    {
        int[] d = new int[Tree.Count];
        int[] v = new int[Tree.Count];
        float temp;
        int minindex, min;

        for (int i = 0; i < Tree.Count; i++)
        {
            d[i] = 10000;
            v[i] = 1;
        }
        d[0] = 0;

        do
        {
            minindex = 10000;
            min = 10000;
            for (int i = 0; i < Tree.Count; i++)
            { // Если вершину ещё не обошли и вес меньше min
                if ((v[i] == 1) && (d[i] < min))
                { // Переприсваиваем значения
                    min = d[i];
                    minindex = i;
                }
            }
            // Добавляем найденный минимальный вес
            // к текущему весу вершины
            // и сравниваем с текущим минимальным весом вершины
            if (minindex != 10000)
            {
                for (int i = 0; i < Tree.Count; i++)
                {
                    if (a[minindex, i] > 0)
                    {
                        temp = min + a[minindex, i];
                        if (temp < d[i])
                        {
                            d[i] = (int)temp;
                        }
                    }
                }
                v[minindex] = 0;
            }
        } while (minindex < 10000);
        // Вывод кратчайших расстояний до вершин


        int[] ver = new int[Tree.Count]; // массив посещенных вершин
        //int end = 4; // индекс конечной вершины = 5 - 1
        ver[0] = end + 1; // начальный элемент - конечная вершина
        int k = 0; // индекс предыдущей вершины
        float weight = d[end]; // вес конечной вершины

        while (end > 0) // пока не дошли до начальной вершины
        {
            for (int i = 0; i < Tree.Count; i++) // просматриваем все вершины
                if (a[end, i] != 0)   // если связь есть
                {
                    float temp1 = weight - a[end, i]; // определяем вес пути из предыдущей вершины
                    if (temp1 == d[i]) // если вес совпал с рассчитанным
                    {                 // значит из этой вершины и был переход
                        weight = temp1; // сохраняем новый вес
                        end = i;       // сохраняем предыдущую вершину
                        ver[k] = i + 1; // и записываем ее в массив
                        k++;
                    }
                }
        }

        Debug.Log("AAA!!! ");
        Debug.Log(string.Join(" ", ver.Select(x => x.ToString()).ToArray()));
        return ver;
    }
    void mot1(float[] arr)//не нужна вроде бы

    {
        for (int i = 0; i < modules.Count - 1; i++)
        {
            modules[i].drivers["q1"].Set(arr[i * 2]);
            modules[i].drivers["q2"].Set(arr[(i * 2) + 1]);

        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
