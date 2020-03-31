using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class DriverEventManager : MonoBehaviour
{

    public class DriverEvent : UnityEvent<int, float, Driver> { };

    private Dictionary<string, DriverEvent> eventDictionary;

    private static DriverEventManager driverEventManager;

    // Синглтон:
    public static DriverEventManager instance
    {
        get
        {
            if (!driverEventManager)
            {
                driverEventManager = FindObjectOfType(typeof(DriverEventManager)) as DriverEventManager;

                if (!driverEventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    driverEventManager.Init();
                }
            }

            return driverEventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, DriverEvent>();
        }
    }


    public static void StartListening(string eventName, UnityAction<int, float, Driver> listener)
    {
        DriverEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new DriverEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<int, float, Driver> listener)
    {
        if (driverEventManager == null) return;
        DriverEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, int recieverID, float angle, Driver drv)
    {
        DriverEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(recieverID, angle, drv);
        }
    }
}