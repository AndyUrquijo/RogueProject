using UnityEngine;
using System.Collections;

public class Projectile : Attack {


	public float Speed;
    public float MaxSpeed;
    public float Gravity;

    Vector3 velocity;

	void Start () {
	}


    override public void StartAttack()
    {
		base.StartAttack();
		velocity = transform.transform.right.normalized * Speed;
		transform.parent = null;
    }

    void FixedUpdate () {
		velocity += Gravity*Vector3.down * Time.fixedDeltaTime;
		transform.position += velocity * Time.fixedDeltaTime;

	}


}
