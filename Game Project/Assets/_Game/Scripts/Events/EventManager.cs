using UnityEngine;
using System.Collections;

public enum EventName
{
    NONE,
    SWORD_OBTAINED,
    GAME_OBJECT_DESTROYED,
}

public class EventManager : MonoBehaviour
{

    public delegate void EventAction();
    private class EventElement
    {
        public event EventAction element;
        public void Execute()
        {
            if (element != null)
                element();
        }
    }

    private EventElement[] elementArray = null;

    private bool initialized = false;

    void Initialize()
    {
        if (initialized)
            return;
        int numTypes = System.Enum.GetValues(typeof(EventType)).Length;
        if (elementArray == null)
        {

            elementArray = new EventElement[numTypes];
            for (int i = 0; i < numTypes; i++)
                elementArray[i] = new EventElement();
        }
        initialized = true;
    }

    void Awake()
    {
        Initialize();
    }

    public void Subscribe(EventName type, EventAction action)
    {
        Initialize();
        if (type == EventName.NONE)
            return;
        elementArray[(int)type].element += action;
    }

    public void Unsubscribe(EventName type, EventAction action)
    {
        Initialize();
        if (type == EventName.NONE)
            return;
        elementArray[(int)type].element -= action;
    }

    public void Notify(EventName type)
    {
        Initialize();
        if (type == EventName.NONE)
            return;
        elementArray[(int)type].Execute();
    }

    private static EventManager Global = null;
    public static EventManager GetGlobal()
    {
        if (!Global)
        {
            Global = GameObject.Find("GlobalEventManager").GetComponent<EventManager>();
            Global.Initialize();
        }
        return Global;
    }
}

