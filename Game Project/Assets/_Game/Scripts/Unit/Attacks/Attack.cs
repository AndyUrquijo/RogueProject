using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : MonoBehaviour {


	public float duration;
	public int damage;
	public float push;
	public float pushDuration;
	public List<string> targets;

	private Vector3 pushDirection;
    public GameObject owner;

	void Start () {
		GameObject.Destroy (gameObject, duration);
		if( transform.parent )
			pushDirection = transform.parent.right;
		else
			pushDirection = transform.right;
			
	}
	
	void Update () {
	
	}

	public void Hit(GameObject unit)
	{
		if( !targets.Contains (unit.tag) )
			return;

		Health health = unit.GetComponentInChildren<Health> ();
		if(health) 
			health.TakeDamage (damage);
		Knockback knockback = unit.GetComponentInChildren<Knockback>();
		if(knockback)
			knockback.TakePush(pushDirection.normalized*push, pushDuration);
		if( health || knockback )
			GameObject.Destroy (gameObject);
	}
}
