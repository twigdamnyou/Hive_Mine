using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    public enum GameEvent
    {
        None,
        TICDocked,
        MinerDocked,
        OreChanged,
        StatChanged,
        BackpackOreChanged,
        MinerUndocked,
        TICUndocked,
        EquipmentCreated,
        WaveStarted,
        WaveEnded,
        PlayerLeftAtmosphere,
        PlayerEnteredAtmosphere,

    }

    private static Dictionary<GameEvent, List<EventListener>> eventDictionary = new Dictionary<GameEvent, List<EventListener>>();
    
    //private static EventManager eventManager;

    //public static EventManager Instance
    //{
    //    get
    //    {
    //        if (eventManager == null)
    //        {
    //            eventManager = FindObjectOfType<EventManager>();
    //        }
    //        if (eventManager == null)
    //        {
    //            Debug.LogError("There Must be an event manager script on a game object in this scene");
    //        }
    //        else
    //        {
    //            eventManager.Initialize();
    //            return eventManager;
    //        }

    //        return eventManager;
    //    }
    //}

    //public void Initialize()
    //{
    //    if (eventDictionary == null)
    //        eventDictionary = new Dictionary<GameEvent, List<EventListener>>();
    //}

    public static void AddListener(GameEvent type, Action<EventData> listener)
    {
        if (type == GameEvent.None)
            return;

        List<EventListener> currentListeners;

        if (eventDictionary.TryGetValue(type, out currentListeners))
        {
            EventListener entry = new EventListener(listener, listener.Target.GetType().Name);

            currentListeners.Add(entry);
        }
        else
        {
            eventDictionary[type] = new List<EventListener>() { new EventListener(listener, listener.Target.GetType().Name) };
        }

    }

    //public static void RemoveListener(GameEvent type, Action<EventData> listener)
    //{
    //    //if (eventManager == null)
    //    //    return;

    //    if (type == GameEvent.None)
    //        return;

    //    List<EventListener> currentListeners;

    //    if (eventDictionary.TryGetValue(type, out currentListeners))
    //    {
    //        for (int i = eventDictionary[type].Count - 1; i >= 0; i++)
    //        {
    //            if (eventDictionary[type][i].callBack.Target == listener.Target)
    //            {
    //                eventDictionary[type].RemoveAt(i);
    //            }
    //        }
    //    }
    //}

    //public static void RemoveListener(GameEvent type, Action<EventData> listener)
    //{
    //    if (eventDictionary.ContainsKey(type))
    //    {
    //        int count = eventDictionary[type].Count;
    //        for (int i = 0; i < count; i++)
    //        {
    //            if (eventDictionary[type][i].callback.Target == listener.Target)
    //            {
    //                eventDictionary[type].RemoveAt(i);
    //                i--;
    //            }
    //        }
    //    }
    //}

    public static void RemoveMyListeners(object currentListener)
    {
        foreach (KeyValuePair<GameEvent, List<EventListener>> listeners in eventDictionary)
        {
            for (int i = 0; i < listeners.Value.Count; i++)
            {
                if (listeners.Value[i].callback.Target == currentListener)
                {
                    listeners.Value.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public static void SendEvent(GameEvent type, EventData eventData)
    {
        if (type == GameEvent.None)
            return;

        List<EventListener> currentListeners;

        if (eventDictionary.TryGetValue(type, out currentListeners))
        {
            int count = currentListeners.Count;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    currentListeners[i].callback.Invoke(eventData);
                }
                catch
                {
                    Debug.LogError("An Event of Type: " + type + ". A listener for: " + currentListeners[i].listenerClassName + " has thrown an exception");
                }
            }
        }
    }








    public class EventListener
    {
        public readonly Action<EventData> callback;
        public readonly string listenerClassName;

        public EventListener(Action<EventData> callBack, string listenerClassName)
        {
            this.callback = callBack;
            this.listenerClassName = listenerClassName;
        }
    }
}
