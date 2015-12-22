using UnityEngine;
using System.Collections;

[System.Serializable]
public class PatrolBehaviour : AIBehaviour {

	public int counter;

	public float reachRange = 3.0f;
	public float attackCooldown = 1;
	public float attackRadius = 1;

    [System.NonSerialized]
    GameObject player;

	public override void Initialize () {
		counter = 0;
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void MoveToWaypoint() {

        if (controller.waypoints.Length == 0)
        {
            controller.command.move = 0;
            return;
        }

        Waypoint next = controller.waypoints[counter];
		float distance = next.destination.x - controller.transform.position.x;
        controller.command.move = Mathf.Sign (distance);
		
		if (Mathf.Abs (distance) < reachRange)
			counter = (counter + 1) % controller.waypoints.Length;
            
	}

	public override void Update () {
        
		MoveToWaypoint ();

		attackCooldown -= Time.deltaTime;

		float distance = Vector3.Distance(player.transform.position, controller.transform.position);
		if (distance < attackRadius && attackCooldown <= 0) {
			bool right = (player.transform.position.x - controller.transform.position.x) > 0;
			controller.command.move = right?-1:1;
            controller.command.attack = 1;
			attackCooldown = 1;
		}
        
	}
}
