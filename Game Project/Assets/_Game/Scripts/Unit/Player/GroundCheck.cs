using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

	public bool grounded;

    public Rigidbody2D body;
	private int groundCounter;
	private State state;

	public float downwardSpeed;
	void Start () {
		body = GetComponentInParent<Rigidbody2D> ();
		state = transform.parent.GetComponentInChildren<State> ();
	}

	void Update()
	{
		if( IsGrounded() && state.state == State.Type.FALL )
			state.state = State.Type.MOVE;

        if( !IsGrounded() && state.state == State.Type.MOVE )
            state.state = State.Type.FALL;

    }

    bool IsGrounded() {
		downwardSpeed = Vector2.Dot (body.velocity, Vector2.down);
		return downwardSpeed > -0.1 && groundCounter > 0;
	}
	void FixedUpdate () {
		grounded = IsGrounded ();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Ground")
			groundCounter++;
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Ground")
			groundCounter--;
	}

    void OnDrawGizmos()
    {
        //Gizmos.color = IsGrounded() ? Color.green : Color.red;
        //Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}
