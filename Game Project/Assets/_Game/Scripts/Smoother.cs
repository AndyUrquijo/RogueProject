using UnityEngine;
using System.Collections;

// Used to smooth out jerky movement
public class Smoother : MonoBehaviour {

    public float maxAcceleration = 10.0f;
    public float maxSpeed = 10.0f;

    private Vector3 prevVel;
    private Vector3 prevPos;

    public void Start()
    {
        prevPos = transform.position;
        prevVel = Vector3.zero;
    }

    public void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        transform.localPosition = Vector3.zero;

        Vector3 currPos = transform.position;
        Vector3 currVel = (currPos - prevPos) / dt;

        Vector3 accel = (currVel - prevVel) / dt;

        if (accel.magnitude > maxAcceleration)
            accel = Vector3.ClampMagnitude(accel, maxAcceleration);

        currVel = prevVel + accel * dt;

        if (currVel.magnitude > maxSpeed)
            currVel = Vector3.ClampMagnitude(currVel, maxSpeed);

        currPos = prevPos + currVel * dt;

        transform.position = currPos;

        prevPos = currPos;
        prevVel = currVel;

    }
}
