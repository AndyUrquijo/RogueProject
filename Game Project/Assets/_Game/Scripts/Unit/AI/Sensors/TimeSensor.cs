using UnityEngine;
using System.Collections;


// Sensor used to determine passage of time for an AI Agent. Time can be slowed down or sped up.
public class TimeSensor : MonoBehaviour {

	private float timer;
	public float Value
	{
		get { return timer; }
	}

	private float modifier;
	public float Modifier
	{
		get { return modifier; }
		set { modifier = value; }
	}

	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime*modifier;
	}

	public void Reset()
	{
		timer = 0;
	}
}
