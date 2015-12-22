using UnityEngine;
using System.Collections;

public class AnimationControl : AbstractBehaviour
{


    Animator anim;

    private float jumpSpeed = 1;
    private float groundTopSpeed = 1;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        Jump jump = GetComponent<Jump>();
        if (jump)
            jumpSpeed = jump.speed;
        GroundMove groundMove = GetComponent<GroundMove>();
        if (groundMove)
            groundTopSpeed = groundMove.topSpeed;
    }

    void Update()
    {
        float speedRatio = body.rigidbody.velocity.magnitude / groundTopSpeed;
        float fallSpeedRatio = -body.rigidbody.velocity.y / jumpSpeed;
        fallSpeedRatio = Mathf.Clamp(fallSpeedRatio,-1,1);
        anim.SetFloat("FallSpeed", fallSpeedRatio);
        anim.SetFloat("Speed", speedRatio);

        anim.SetBool("Fall", state.Is( State.Type.FALL ));
        anim.SetBool("Dash", state.Is(State.Type.DASH ));
        if (state.Toggled(State.Type.FALL))
            anim.SetTrigger("FallStart");
        if (state.Toggled(State.Type.DASH))
            anim.SetTrigger("DashStart");
        if (state.Toggled(State.Type.MOVE))
            anim.SetTrigger("MoveStart");
    }
}
