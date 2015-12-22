using UnityEngine;
using System.Collections;

public class ChaseBehaviour : AIBehaviour {

    // maximum distance traversed away from initial point and way points. May chase wherever with 0.
    public float maxDistance;


    public override void Initialize() { }

    // Update is called once per frame
    public override void Update () {
        
        //bool disengage = true;
        if (maxDistance != 0)
        {
            foreach (Waypoint waypoint in controller.waypoints)
            {
                Vector3 displacement = controller.transform.position - waypoint.destination;
                if (displacement.magnitude < maxDistance)
                {
                    //disengage = false;
                    break;
                }
            }
        }
        //else
            //disengage = false;

    

    }
}
