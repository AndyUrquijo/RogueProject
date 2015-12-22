using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour {

	public float resistance; // 0: non resistance, 1: full resistance
	private  Body body;

	private Vector2 appliedForce;
	private float durationLeft;
	private float durationTotal;
	private GameObject dustObj;
	private ParticleSystem dust;
	void Start () {
		body = GetComponent<Body> ();
		dustObj = transform.Find ("Dust").gameObject;
		dust = dustObj.GetComponent<ParticleSystem> ();
	}
	
	void FixedUpdate()
	{
		if (durationLeft > 0) {
			durationLeft -= Time.fixedDeltaTime;
			body.rigidbody.AddForce (appliedForce * durationLeft / durationTotal);
		} else
			dust.Stop ();
	}

	public void TakePush( Vector2 push, float duration ) {
		appliedForce = push*(1-resistance);
		durationTotal = durationLeft = duration;
		dust.Play ();
		dustObj.transform.rotation = Quaternion.Euler (-90, 90, 0);
	}

}
