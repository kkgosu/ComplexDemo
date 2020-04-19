using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public IEnumerator Forward(ModularRobot modularRobot, float[] angles)
    {
        GaitControlTable controlTable = modularRobot.gameObject.AddComponent<GaitControlTable>();
        controlTable.ReadFromFile(modularRobot, CreateGCT(NextStep(angles), 3));
        yield return StartCoroutine(ForwardC(controlTable));
        controlTable.ReadFromFile(modularRobot, CreateGCT(NextStep(angles), 3));
        yield return StartCoroutine(ForwardC(controlTable));
        controlTable.ReadFromFile(modularRobot, CreateGCT(NextStep(angles), 3));
        yield return StartCoroutine(ForwardC(controlTable));
        controlTable.ReadFromFile(modularRobot, CreateGCT(NextStep(angles), 3));
        yield return StartCoroutine(ForwardC(controlTable));
    }

    private IEnumerator ForwardC(GaitControlTable controlTable)
    {
        yield return WaitUntilMoveEnds(controlTable);
    }

    public float[] NextStep(float[] angles)
    {
        float last = angles[angles.Length - 1];
        for (int i = angles.Length - 2; i >= 0; i--)
        {
            angles[i + 1] = angles[i];
        }
        angles[0] = last;
        return angles;
    }

    private string CreateGCT(float[] angles, int time)
    {
        StringBuilder builder = new StringBuilder("header = \"");
        for (int i = 0; i < angles.Length; i++)
        {
            builder.Append(i);
            if (i != angles.Length - 1)
            {
                builder.Append(",");
            }
            else
            {
                builder.Append("\"\n");
            }
        }
        string header = builder.ToString();
        builder.Clear();

        for (int i = 0; i < angles.Length; i++)
        {
            string angle = angles[i].ToString().Replace(",", ".");
            builder.Append(angle).Append("(" + time + ")");
            if (i != angles.Length - 1)
            {
                builder.Append(",");
            }
            else
            {
                builder.Append("\n");
            }
        }

        string values = builder.ToString();
        string path = Application.dataPath + "/Resources/Gait Control Tables/" + "teztz" + ".gct";
        File.WriteAllText(path, header + values);
        return path;
    }
    private IEnumerator WaitUntilMoveEnds(GaitControlTable controlTable)
    {
        controlTable.BeginTillEnd();
        while (controlTable.inProgress || !controlTable.isReady)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
