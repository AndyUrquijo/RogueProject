using UnityEngine;
using System.Collections;

[System.Serializable]
public class ShootingBehaviour : AIBehaviour {

    float timer = 0;
    float time = 1.0f;

    public GameObject Projectile;

    public override void Initialize()
    {

    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if( timer <= 0 )
        {
            timer = time;
            controller.command.jump.Toggle = true;
        }
    }

}
