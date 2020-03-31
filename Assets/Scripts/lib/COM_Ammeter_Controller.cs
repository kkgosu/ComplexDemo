using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class COM_Ammeter_Controller : MonoBehaviour {

    // Настройка COM-порт:
    public int portBaudRate = 9600;

    SerialPort sp;
    Dropdown dd;
    Button bConnect, bCancel, bHide;
    Text desc;

    public float current = 0;

    void Start()
    {
        desc = UIHelper.AddText("Ammeter:");
        RectTransform descRT = desc.GetComponent<RectTransform>();
        descRT.anchorMax = new Vector2(0.5f, 1);
        descRT.anchorMin = new Vector2(0.5f, 1);
        descRT.pivot = new Vector2(0.5f, 1);
        descRT.anchoredPosition = new Vector3(0, -20, 0);

        dd = UIHelper.AddDropdown(GetSerialPorts());
        RectTransform ddRT = dd.GetComponent<RectTransform>();
        ddRT.anchorMax = new Vector2(0.5f, 1);
        ddRT.anchorMin = new Vector2(0.5f, 1);
        ddRT.pivot = new Vector2(0.5f, 1);
        ddRT.anchoredPosition = new Vector3(0, -50, 0);

        Button buttonConnect = UIHelper.AddButton(buttonText: "Connect");
        buttonConnect.onClick.AddListener(delegate () { ConnectToPort(dd.options[dd.value].text); });
        RectTransform brRT = buttonConnect.GetComponent<RectTransform>();
        brRT.anchorMax = new Vector2(0.5f, 1);
        brRT.anchorMin = new Vector2(0.5f, 1);
        brRT.pivot = new Vector2(0.5f, 1);
        brRT.anchoredPosition = new Vector3(0, -90, 0);

        Button buttonCancel = UIHelper.AddButton(buttonText: "Refresh");
        buttonCancel.onClick.AddListener(delegate () { RefreshDropdownOptions(); });
        RectTransform bcRT = buttonCancel.GetComponent<RectTransform>();
        bcRT.anchorMax = new Vector2(0.5f, 1);
        bcRT.anchorMin = new Vector2(0.5f, 1);
        bcRT.pivot = new Vector2(0.5f, 1);
        bcRT.anchoredPosition = new Vector3(0, -130, 0);
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

    string text = "null";
    byte rcvdByte = 0x00;

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
        int counter = 0;
        while (sp.IsOpen)
        {
            if (sp.BytesToRead > 0)
            {
                text = sp.ReadLine();
                current = float.Parse(text);
                counter++;
            }
        }
    }


}
