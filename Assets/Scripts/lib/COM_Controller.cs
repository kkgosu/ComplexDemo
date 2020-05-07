using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class COM_Controller : MonoBehaviour
{
    // Параметры нашего устройства:
    public byte deviceID = 0xFF;

    // Настройка COM-порт:
    public int portBaudRate = 9600;

    // Константы:
    enum DriverInfo : byte
    {
        AngleMin = 0x35,            // ANGLE_MIN   
        AngleDefault = 0x8C,        // ANGLE_CENTER
        AngleMax = 0xE4             // ANGLE_MAX   
    }

    enum Command : byte
    {
        AngleSet = 0x01,            // COM_ANGLE_SET [angle]
        AngleSubstract = 0x02,      // COM_ANGLE_MINUS [delta angle]
        AngleAdd = 0x03,            // COM_ANGLE_PLUS [delta angle]
        AngleGetCurrent = 0x04,     // COM_ANGLE_GET
        AngleResponce = 0x05,       // COM_ANGLE_IS
        AngleGetMin = 0x06,         // COM_ANGLE_MIN
        AngleGetDefault = 0x07,     // COM_ANGLE_CENTER
        AngleGetMax = 0x08,         // COM_ANGLE_MAX

        Ping = 0x80,                // COM_INFO_IS_ONLINE
        Pong = 0xDA,                // COM_INFO_ONLINE_OK

        VerisonRequest = 0x81,      // COM_INFO_VERSION -- Нужен ли нам Responce для Request?

        FlashWriteID = 0x30,        // COM_WRITE_ID [id]
        FlashWriteAngle = 0x31,     // COM_WRITE_ANGLE [angle] -- OUTDATED

        LedOff = 0x50,              // COM_LED_OFF
        LedOn = 0x51,               // COM_LED_ON
        LedToggle = 0x52,           // COM_LED_TOGGLE
        LedOnForMilliseconds = 0x53,// COM_LED_ONOFF [time in ms] - Turns led for X milliseconds
        LedBlink = 0x54,            // COM_LED_BLINK [time in ms] - Turns led every X milliseconds

        SpeedSet = 0x21,            // [2 bytes - time in ms]
        SpeedReset = 0x22           // Reset speed to 10 ms. (?)
    }

    enum Info : byte
    {
        StartByte = 0xAA,           // 10101010
        TermByte = 0x55,            // 01010101

        Version = 0x01,             // OPT_INFO_VERSION
        Broadcast = 0x00            // Send message to all modules
    }

    // Локальные переменные:
    SerialPort sp;
    Dropdown dd;
    Button buttonConnect, buttonRefresh;
    GameObject panel;
    Text desc;
    bool windowShowed;

    void Start()
    {
        DrawWindow();

        desc = UIHelper.AddText("Robot:");
        RectTransform descRT = desc.GetComponent<RectTransform>();
        descRT.anchorMax = new Vector2(0, 1);
        descRT.anchorMin = new Vector2(0, 1);
        descRT.pivot = new Vector2(0, 1);
        descRT.anchoredPosition = new Vector3(10, -20, 0);

        dd = UIHelper.AddDropdown(GetSerialPorts());
        RectTransform ddRT = dd.GetComponent<RectTransform>();
        ddRT.anchorMax = new Vector2(0, 1);
        ddRT.anchorMin = new Vector2(0, 1);
        ddRT.pivot = new Vector2(0, 1);
        ddRT.anchoredPosition = new Vector3(10, -50, 0);

        buttonConnect = UIHelper.AddButton(buttonText: "Connect");
        buttonConnect.onClick.AddListener(delegate () { ConnectToPort(dd.options[dd.value].text); });
        RectTransform brRT = buttonConnect.GetComponent<RectTransform>();
        brRT.anchorMax = new Vector2(0, 1);
        brRT.anchorMin = new Vector2(0, 1);
        brRT.pivot = new Vector2(0, 1);
        brRT.anchoredPosition = new Vector3(10, -90, 0);

        buttonRefresh = UIHelper.AddButton(buttonText: "Refresh");
        buttonRefresh.onClick.AddListener(delegate () { RefreshDropdownOptions(); });
        RectTransform bcRT = buttonRefresh.GetComponent<RectTransform>();
        bcRT.anchorMax = new Vector2(0, 1);
        bcRT.anchorMin = new Vector2(0, 1);
        bcRT.pivot = new Vector2(0, 1);
        bcRT.anchoredPosition = new Vector3(10, -130, 0);
    }

    public void DrawWindow()
    {

    }

    public void ToggleWindow()
    {

    }

    public void DestroyWindow()
    {
        if (sp != null && sp.IsOpen)
            sp.Close();
        Destroy(desc.gameObject);
        Destroy(dd.gameObject);
        Destroy(buttonConnect.gameObject);
        Destroy(buttonRefresh.gameObject);
    }

    void ConnectToPort(string portName)
    {
        sp = new SerialPort
        {
            PortName = portName,
            BaudRate = portBaudRate,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
            Handshake = Handshake.None
        };

        try
        {
            sp.Open();
            Debug.Log(string.Format("Serial port opened on {0}.", portName));

            // Чтение:
            Thread readThread = new Thread(new ThreadStart(ReceiveData));
            readThread.Start();

        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Couldn't open serial port on {0}: {1}.", portName, e.Message));
        }
    }

    void OnDisable()
    {
        if (sp != null && sp.IsOpen)
            sp.Close();
    }

    private List<string> GetSerialPorts()
    {
        string[] ports;
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.LinuxPlayer:
                ports = Directory.GetFiles("/dev/", "cu.*");
                break;
            default:
                ports = SerialPort.GetPortNames();
                break;
        }
        return new List<string>(ports);
    }

    private void RefreshDropdownOptions()
    {
        dd.ClearOptions();
        dd.AddOptions(GetSerialPorts());
        dd.RefreshShownValue();
    }

    private void ReceiveData()
    {   /*
        byte[] package = new byte[7];
        while (sp.IsOpen)
        {
            if (sp.ReadByte() == (byte)Info.StartByte)
            {
                sp.Read(package, 0, 7);
                if (package[6] == (byte)Info.TermByte)
                    PrintMessage(package, package[2] == (byte)Command.AngleSet, "Message received");
            }
            sp.DiscardOutBuffer();
            Thread.Sleep(10);
        }
        */
        while (sp.IsOpen) {
            print(sp.ReadLine());
            Thread.Sleep(10);
        }
    }

    public void SetAngle(int receiverID, float angle, Driver drv)
    {
        SetAngle(receiverID, angle, drv.qMin, drv.qMax);
    }

    public void SetAngle(int receiverID, float angle, float angleMin, float angleMax)
    {
        float angleRange = angleMax - angleMin;
        float realAngleRange = DriverInfo.AngleMax - DriverInfo.AngleMin;

        int realAngle = (byte)DriverInfo.AngleMin + (int)(((angle - angleMin) / angleRange) * realAngleRange);

        //LEDToggle(receiverID);

        byte[] package = new byte[6];

        package[0] = (byte)receiverID;
        package[1] = (byte)deviceID;
        package[2] = (byte)Command.AngleSet;
        package[3] = (byte)realAngle;
        package[4] = (byte)0x00;
        package[5] = (byte)0x00;

        PrintMessage(package, containsAngle: true, prefix: "Message sent");
        SendPackage(package);
    }

    public void LEDToggle(int receiverID)
    {
        byte[] package = new byte[6];

        package[0] = (byte)receiverID;
        package[1] = (byte)deviceID;
        package[2] = (byte)Command.LedToggle;
        package[3] = (byte)0x00;
        package[4] = (byte)0x00;
        package[5] = (byte)0x00;

        PrintMessage(package);
        SendPackage(package);
    }

    private void SendPackage(byte[] message)
    {
        byte[] package = new byte[8];
        package[0] = (byte)Info.StartByte;
        int count = 1;                                      // Индекс начала информационного сообщения.
        Array.ForEach(message, b => package[count++] = b);
        package[7] = (byte)Info.TermByte;

        if (sp != null && sp.IsOpen)
        {
            sp.Write(package, 0, 8);
            Thread.Sleep(2);                                // Если возникнут проблемы с потерей байтов, поднять величину задержки.
            sp.DiscardOutBuffer();
        }
        else
            Debug.LogError("Can't send package. Serial port on is not opened.");
    }

    private void PrintMessage(byte[] message, bool containsAngle = false,
                               string prefix = "Message printed")
    {
        switch (message[2])
        {
            // Driver:
            case (byte)Command.AngleResponce:
            case (byte)Command.AngleAdd:
            case (byte)Command.AngleSubstract:
            case (byte)Command.AngleSet:
               /* print(string.Format((message[5] == 0x00) ? "{8}: TO: {0}, FROM: {1},\n" +
                                    "COMMAND: {2} ({3}), PARAM 1: {4}, PARAM 2: {5}" +
                                    " (angle ~ {6}), PARAM 3: {7} (time)." :
                                    "{8}: TO: {0}, FROM: {1},\nCOMMAND: {2} ({3})," +
                                    " PARAM 1: {4}, PARAM 2: {5} (angle ~ {6}).",
                                    (message[0] == (byte)Info.Broadcast)
                                    ? String.Format("{0} (to all)", message[0])
                                    : message[0].ToString(),    // to
                                    message[1],                 // from
                                    message[2],                 // command
                                    ((Command)message[2]).ToString(),
                                    message[3],                 // angle_high
                                    message[4],                 // angle_low
                                    DecodeAngle(message[3], message[4], 180, -90),
                                    message[5],                 // time
                                    prefix
                                    ));*/
                break;
            default:
                print(string.Format("{7}: TO: {0}, FROM: {1},\nCOMMAND: {2} ({3}), " +
                                    "PARAM 1: {4}, PARAM 2: {5}, PARAM 3: {6}.",
                                    (message[0] == (byte)Info.Broadcast)
                                    ? String.Format("{0} (to all)", message[0])
                                    : message[0].ToString(),    // to
                                    message[1],                 // from
                                    message[2],                 // command
                                    ((Command)message[2]).ToString(),
                                    message[3],                 // param 1
                                    message[4],                 // param 2
                                    message[5],                 // param 3
                                    prefix
                                    ));
                break;
        }
    }

    private float DecodeAngle(byte highAngle, byte lowAngle = 0x00, float range = 180, float qMin = 0)
    {
        float angle;
        float realAngleRange = DriverInfo.AngleMax - DriverInfo.AngleMin;
        angle = qMin + ((highAngle - (byte)DriverInfo.AngleMin) / realAngleRange) * range;
        return angle;
    }

#region Connection

#endregion Connection

#region Misc

#endregion Misc

#region UI

#endregion UI
}