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
                builder.AppendLine("\t\t<module id=\"" + i + "\" " + "q1=\"" + modules[i] + "\"></module>");
            } else
            {
                builder.AppendLine("\t\t<module id=\"" + i + "\" " + "q1=\"0\"></module>");
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
    public string CreateConnectionString(int from, int to, Sides sFrom, Sides sTo)
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
        return "<connection from=\"" + from + "\" to=\"" + to + "\" surfaceFrom=\"" + sides[sFrom] + "\" surfaceTo=\"" + sides[sTo] + "\"></connection>";
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
