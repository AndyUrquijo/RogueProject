using UnityEngine;
using System.Collections;

public class Move : AbstractBehaviour
{

	public float topSpeed;
	public float acceleration;
    public float friction;

	protected void MoveUpdate()
	{
		Vector3 moveAcceleration = Vector3.right * acceleration * command.move; 


		if (moveAcceleration != Vector3.zero)
			body.rigidbody.AddForce(moveAcceleration);
		else
			Friction();

		Vector2 vel = body.rigidbody.velocity;
        float speedLimit = topSpeed * Mathf.Abs(command.move);

        if (Mathf.Abs(vel.x) > speedLimit)
            vel.x = Mathf.MoveTowards(vel.x, Mathf.Sign(vel.x) * speedLimit, friction * Time.fixedDeltaTime);

		body.rigidbody.velocity = vel;


		
	}

    protected void Friction()
    {
        return;
        // fricion only affects horizonal motion

       // float frictionForce = -Mathf.Sign(body.rigidbody.velocity.x) * friction;
       // if (friction * Time.fixedDeltaTime > Mathf.Abs(body.rigidbody.velocity.x))
       //     body.rigidbody.velocity = new Vector2(0, body.rigidbody.velocity.y); // Zero out x
       // else
       //     body.rigidbody.AddForce(Vector2.right * frictionForce);
    }

}
