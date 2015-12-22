using UnityEngine;
using System.Collections;

public class AirMove : Move {

	void FixedUpdate() {
		
		if (state.state == State.Type.FALL)
			MoveUpdate ();
	}


}
