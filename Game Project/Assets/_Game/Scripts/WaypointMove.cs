using UnityEngine;
using System.Collections;

public class WaypointMove : MonoBehaviour
{

    public Waypoint[] waypoints;
    public int counter;
    public float speed;

    public float reachRange = 3.0f;

    private float timer;
    void Start()
    {
        waypoints = GetComponentsInChildren<Waypoint>();
        counter = 0;
    }

    void MoveToWaypoint()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            return;
        }

        Waypoint next = waypoints[counter];
        Vector3 displacement = next.destination - transform.position;
        transform.position += displacement.normalized * speed * Time.fixedDeltaTime;

        if (Mathf.Abs(displacement.magnitude) < reachRange)
        {
            counter = (counter + 1) % waypoints.Length;
            timer = waypoints[counter].delay;
        }
    }

    void FixedUpdate()
    {
        MoveToWaypoint();
    }
}
