using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : AttackAction {


	public float duration;
	public int damage;
	public float push;
	public float pushDuration;

	private Vector3 pushDirection;
    
	[HideInInspector]

	override public void StartAttack () {

		if (duration == 0) {
			Animator anim = GetComponent<Animator> ();
			if( anim != null )
				duration = anim.runtimeAnimatorController.animationClips [0].length;
		}
		
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
