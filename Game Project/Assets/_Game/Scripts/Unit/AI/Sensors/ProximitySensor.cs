using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProximitySensor : AISensor {

    public string tagFilter = "";
    public int counter = 0;
    List<GameObject> detected;

    // Use this for initialization
    void Start()
    {
        detected = new List<GameObject>();
    }

    void Update () {
        counter = detected.Count;
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(tagFilter == "" || other.tag == tagFilter)
        {
            detected.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (tagFilter == "" || other.tag == tagFilter)
        {
            detected.Remove(other.gameObject);
        }
    }

    public GameObject[] Detect()
    {
        return detected.ToArray(); // NOTE: Potential bad performance
    }
}
