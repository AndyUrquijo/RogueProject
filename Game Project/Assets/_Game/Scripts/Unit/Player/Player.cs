using UnityEngine;
using System.Collections;

public class Player : AbstractBehaviour {

	string hold = "";

    void Update () {

        command.move = Input.GetAxis("Horizontal");

		command.attack = 0;

		CheckAttack("Attack", 1);


		command.release = false;
		if (hold != "" && Input.GetButtonUp(hold)) {
			command.release = true;
            hold = "";
        }
	
		command.jump.Toggle = Input.GetButton("Jump");
		command.dash = Input.GetKeyDown (KeyCode.Q);
	}

	void CheckAttack(string button, int val)
	{
		if (Input.GetButtonDown (button)) {
			command.attack = val;
			if( hold == "" )
				hold = button;
		}
	}
}
