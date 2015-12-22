using UnityEngine;
using System.Collections;

public class Dash : AbstractBehaviour
{

	public float speed;
	public float duration;

	private float durationLeft;
	private Jump jump;
	void Start () {
		jump = GetComponent<Jump> ();
	}



	void Update () {
		
        // TODO when state changes to jump in the middle this fails
		if (state.state == State.Type.DASH) {
			if( durationLeft < 0 ) {
				state.state = State.Type.FALL;
				jump.gravityModifier = 1;
			}
			else
				durationLeft -= Time.deltaTime;
		}
		else if (command.dash) {
			state.state = State.Type.DASH;
			body.rigidbody.velocity = transform.right * speed;
			command.dash = false;
			durationLeft = duration;
			jump.gravityModifier = 0;
		}
	}
}
