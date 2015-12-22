using UnityEngine;
using System.Collections;

public abstract class EventListener : EventSubject
{


    void OnEnable()
    {
        subject.manager.Subscribe(type, EventAction);
    }

    void OnDisable()
    {
        subject.manager.Unsubscribe(type, EventAction);
    }

    protected abstract void EventAction();
}
