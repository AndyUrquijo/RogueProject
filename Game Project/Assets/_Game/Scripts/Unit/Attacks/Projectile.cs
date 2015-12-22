using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {


	public float Speed;
    public float MaxSpeed;
    public float Gravity;

	private Vector3 velocity;

    Rigidbody2D rigidBody;

	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
		transform.parent = null;
	}

    public void Shoot(Vector3 velocity)
    {
        rigidBody.velocity = transform.parent.transform.right.normalized * Speed;
    }

    void FixedUpdate () {
		transform.position += velocity * Time.fixedDeltaTime;

	}


}
