using UnityEngine;
using System.Collections;

public class DestroyOnEvent : EventListener {

    public float delay = 0;
    public int eventAmount = 1;
    private int eventCounter = 0;

    override protected void EventAction()
    {
        if (++eventCounter < eventAmount)
            return;

        ParticleSystem[] emitters = GetComponentsInChildren<ParticleSystem>(false);
        for (int i = 0; i < emitters.Length; i++)
        {
            //This algorithm relies on having each ParticleSystem component on a separate gameobject. 
            //Otherwise gameobjects may be destroyed sooner than expected
            emitters[i].transform.parent = null;
            emitters[i].Stop();
            GameObject.Destroy(emitters[i].gameObject, emitters[i].startLifetime);
        }
        GameObject.Destroy(gameObject, delay);

    }
    
}
