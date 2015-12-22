using UnityEngine;
using System.Collections;

public class GroundMove : Move
{

    void FixedUpdate()
    {

        if (state.state == State.Type.MOVE)
            MoveUpdate();
    }
}
