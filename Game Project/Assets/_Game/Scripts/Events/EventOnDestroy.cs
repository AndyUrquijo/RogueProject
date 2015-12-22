using UnityEngine;
using System.Collections;





public class EventOnDestroy : EventSubject
{
	
    void OnDestroy()
    {
        subject.manager.Notify(type);
    }
}
