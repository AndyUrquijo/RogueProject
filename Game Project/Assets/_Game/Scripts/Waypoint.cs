using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

    [HideInInspector]
	public Vector3 destination;
    public float delay = 0;

	// Use this for initialization
	void Start () {
		destination = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = destination;	
	}
}
