using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Multishot : AttackAction {

	public GameObject projectile;

	public List<float> shotAngles;

	override public void StartAttack()
	{

		foreach (float shotAngle in shotAngles) 
		{
				
			GameObject instance = GameObject.Instantiate(projectile);
			instance.transform.position = transform.position;
			instance.transform.parent = null;

			instance.transform.rotation = Quaternion.Euler(0, 0, shotAngle);

			Projectile projComp = instance.GetComponent<Projectile>();

			if (projComp) {
				projComp.targets = targets;
				projComp.owner = gameObject;
				projComp.StartAttack();
			}
			
		}
	}
}
