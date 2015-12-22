using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor;
// Used to make speed and height variables calculate each other automatically
[CustomEditor(typeof(Jump))]
public class JumpEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Jump jump = (Jump)target;
        float jumpSpeed = EditorGUILayout.FloatField(new GUIContent("Speed*", "The vertical jumping speed"), jump.speed);
        float jumpHeight = EditorGUILayout.FloatField(new GUIContent("Height*", "The predicted jumping height"), jump.height);
        float jumpBoostedHeight = EditorGUILayout.FloatField(new GUIContent("Boosted Height*", "The predicted jumping height while boosting"), jump.boostedHeight);
        jump.gravity = EditorGUILayout.FloatField("Gravity", jump.gravity);

        if (GUI.changed)
        {
            if (jumpSpeed != jump.speed && !float.IsNaN(jumpSpeed))
            {
                jumpHeight = jumpSpeed * jumpSpeed / (2.0f * jump.gravity);
                jumpBoostedHeight = jumpSpeed * jumpSpeed / (2.0f * (jump.gravity - jump.holdBoost));
            }
            else if (jumpHeight != jump.height && !float.IsNaN(jumpHeight))
            {
                jumpSpeed = Mathf.Sqrt(2.0f * jumpHeight * jump.gravity);
                jumpBoostedHeight = jumpSpeed * jumpSpeed / (2.0f * (jump.gravity - jump.holdBoost));
            }
            else // boosted height is maintained when other variables change.
            {
                jumpSpeed = Mathf.Sqrt(2.0f * jumpBoostedHeight * (jump.gravity - jump.holdBoost));
                jumpHeight = jumpSpeed * jumpSpeed / (2.0f * jump.gravity);
            }

        }

        jump.height = jumpHeight;
        jump.boostedHeight = jumpBoostedHeight;
        jump.speed = jumpSpeed;
    }
}
#endif



public class Jump : AbstractBehaviour
{

    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float height;
    [HideInInspector]
    public float boostedHeight;
    [HideInInspector]
    public float gravity = 10;

    public float holdBoost;

    [HideInInspector]
    public float gravityModifier = 1;

    // TODO: Create a data type to store each successive jump individual traits
    public int jumpAmount = 1;
    private int jumpCounter = 0;

    private bool jumping;

    private bool doubleJumped;

    void Update()
    {
        if (state.state == State.Type.MOVE)
            jumpCounter = 0;

        bool canJump = state.state == State.Type.MOVE
            || state.state == State.Type.FALL;

        if (command.jump.On && canJump)
        {
            if (jumpCounter < jumpAmount)
            {
                state.state = State.Type.FALL;
                jumpCounter++;

                // Vertical jump speed is independent of previous falling/jumping speed
                Vector3 vel = body.rigidbody.velocity;
                vel.y = speed;
                body.rigidbody.velocity = vel;
            }
        }



    }

    void FixedUpdate()
    {
        if (command.jump && body.rigidbody.velocity.y > 0)
            body.rigidbody.AddForce(Vector3.up * holdBoost);

        body.rigidbody.AddForce(Vector3.down * gravity * gravityModifier);
    }
}

