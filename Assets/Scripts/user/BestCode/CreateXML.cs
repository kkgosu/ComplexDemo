using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

public class CreateXML : MonoBehaviour
{
    public enum Sides
    {
        TOP, BOTTOM, RIGHT, LEFT
    }

    private Dictionary<Sides, string> sides = new Dictionary<Sides, string>();

    private string header = "";
    private string modules = "";
    private string connections = "";
    private string footer = "</robot>";

    /// <summary>
    /// Создание шапки xml документа
    /// </summary>
    /// <param name="robotName">Имя робота</param>
    /// <param name="position">Начальные координаты расположения</param>
    /// <param name="rotation">Начальный кватернион поворота</param>
    /// <returns></returns>
    public CreateXML CreateHeader(string robotName, Vector3 position = default(Vector3), Quaternion rotation = default (Quaternion))
    {
        StringBuilder builer = new StringBuilder("<robot name=\"");
        builer
            .Append(robotName + "\" ")
            .Append("moduleType=\"M3R2\" ")
            .Append("position=\"" + position.x + "," + position.y + "," + position.z + "\" ")
            .Append("rotation=\"" + rotation.eulerAngles.x + "," + rotation.eulerAngles.y + "," + rotation.eulerAngles.z + "\">")
            .Append("\n");
        header = builer.ToString();

        return this;
    }

    /// <summary>
    /// Добавляет раздел modules
    /// </summary>
    /// <param name="total">Общее количество модулей</param>
    /// <returns></returns>
    public CreateXML AddModules(int total)
    {
        StringBuilder builder = new StringBuilder("\t<modules>\n");
        for(int i = 0; i < total; i ++)
        {
            builder.AppendLine("\t\t<module id=\"" + i + "\" " + "q1=\"0\"></module>");
        }
        builder.AppendLine("\t</modules>");
        modules = builder.ToString();

        return this;
    }

    /// <summary>
    /// Добавляет раздел modules
    /// </summary>
    /// <param name="total">Общее количество модулей</param>
    /// <param name="modules">Лист ТОЛЬКО тех модулей, у которых необходимо поменять q1</param>
    /// <returns></returns>
    public CreateXML AddModules(int total, Dictionary<int, float> modules)
    {
        StringBuilder builder = new StringBuilder("\t<modules>\n");
        for (int i = 0; i < total; i++)
        {
            if (modules.ContainsKey(i))
            {
                builder.AppendLine("\t\t<module id=\"" + i + "\" " + "q1=\"" + modules[i] + "\"/>");
            } else
            {
                builder.AppendLine("\t\t<module id=\"" + i + "\" " + "q1=\"0\"/>");
            }
        }
        builder.AppendLine("\t</modules>");
        this.modules = builder.ToString();

        return this;
    }

    /// <summary>
    /// Добавлает раздел connections
    /// </summary>
    /// <param name="connections">Список соединений</param>
    /// <returns></returns>
    public CreateXML AddConnections(List<string> connections)
    {
        StringBuilder builder = new StringBuilder("\t<connections>\n");
        foreach (string connection in connections)
        {
            builder.AppendLine("\t\t" + connection);
        }
        builder.AppendLine("\t</connections>");
        this.connections = builder.ToString();
        return this;
    }

    public Dictionary<int, float> CreateModules(float[] array)
    {
        Dictionary<int, float> modules = new Dictionary<int, float>();
        for(int i = 0; i < array.Length; i++)
        {
            modules.Add(i, array[i]);
        }
        return modules;
    }

    public string Create(string name)
    {
        string path = Application.dataPath + "/Resources/Configurations/" + name + ".xml";
        print(path);
        File.WriteAllText(path, header + modules + connections + footer);
        return path;
    }

    /// <summary>
    /// Создает строку для соединений
    /// </summary>
    /// <param name="from">Какой модуля</param>
    /// <param name="to">К какому модулю</param>
    /// <param name="sFrom">Какой стороной</param>
    /// <param name="sTo">К какой стороне</param>
    /// <returns></returns>
    public string CreateConnectionString(int from, int to, Sides sFrom, Sides sTo, int tilt = 0)
    {
        //<connection from="41" to="42" surfaceFrom="top" surfaceTo="bottom">
        //</ connection >
        if (sides.Count == 0)
        {
            sides.Add(Sides.TOP, "top");
            sides.Add(Sides.BOTTOM, "bottom");
            sides.Add(Sides.RIGHT, "right");
            sides.Add(Sides.LEFT, "left");
        }
        if (tilt == 0)
        {
            return "<connection from=\"" + from + "\" to=\"" + to + "\" surfaceFrom=\"" + sides[sFrom] + "\" surfaceTo=\"" + sides[sTo] + "\"/>";
        }
        return "<connection from=\"" + from + "\" to=\"" + to + "\" surfaceFrom=\"" + sides[sFrom] + "\" surfaceTo=\"" + sides[sTo] + "\"" + " tilt=\"" + tilt + "\"/>";
    }

    /// <summary>
    /// Создает соединения для ходячего робота
    /// </summary>
    /// <param name="total">Кол-во модулей</param>
    /// <returns></returns>
    public List<string> CreateConnectionsForWalker(int total)
    {
        List<string> conenctions = new List<string>();

        //creating central connections
        if (total > 4)
        {
            conenctions.Add(CreateConnectionString(0, 1, Sides.TOP, Sides.TOP, 90));
            conenctions.Add(CreateConnectionString(0, 2, Sides.RIGHT, Sides.TOP, 90));
            conenctions.Add(CreateConnectionString(0, 3, Sides.BOTTOM, Sides.TOP, 90));
            conenctions.Add(CreateConnectionString(0, 4, Sides.LEFT, Sides.TOP, 90));

            //add leg modules
            for (int i = 1; i < total - 4; i++)
            {
                if (i >= 1 && i <= 4)
                {
                    conenctions.Add(CreateConnectionString(i, i + 4, Sides.BOTTOM, Sides.TOP, -90));
                } else
                {
                    conenctions.Add(CreateConnectionString(i, i + 4, Sides.BOTTOM, Sides.TOP));
                }
            }
        }

        return conenctions;
    }

    public List<string> CreateSimpleConnections(int total)
    {
        List<string> conenctions = new List<string>();
        for (int i = 0; i < total - 1; i++)
        {
            conenctions.Add(CreateConnectionString(i, i + 1, Sides.TOP, Sides.BOTTOM));
        }

        conenctions.Add(CreateConnectionString(total - 1, 0, Sides.TOP, Sides.BOTTOM));
        return conenctions;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
