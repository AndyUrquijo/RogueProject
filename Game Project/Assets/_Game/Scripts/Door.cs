using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public GameObject DestructionEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.transform.parent.gameObject;
        if (!obj) return;
        Attack attack = obj.GetComponent<Attack>();
        if (!attack) return;
        if (attack.owner.tag != "Player") return;


        GameObject instance = GameObject.Instantiate(DestructionEffect);
        instance.transform.position = transform.position;
        ParticleSystem partSys = instance.GetComponent<ParticleSystem>();
        float lifetime = partSys.duration + partSys.startLifetime;
        GameObject.Destroy(instance, lifetime);

        GameObject.Destroy(gameObject);
    }

}
