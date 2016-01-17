using UnityEngine;
using System.Collections;
using UnityEditor;

// Sensor used to determine passage of time for an AI Agent. Time can be slowed down or sped up.
public class TimeSensor : MonoBehaviour {

	public float timer;
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

	void Update () {
		timer += Time.deltaTime*modifier;
	}

	public void Reset()
	{
		timer = 0;
	}

    public void OnDrawGizmos()
    {
        if(timer != 0)
            Handles.Label(transform.position + Vector3.down*0.5f, "Timer: " + timer);
    }
}
