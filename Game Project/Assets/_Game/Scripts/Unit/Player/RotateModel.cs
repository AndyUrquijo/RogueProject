using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotateModel : AbstractBehaviour
{
    public float angularSpeed = 45.0f;

    private Quaternion rotationTarget;
    private List<Transform> models;

    void Start()
    {
        models = new List<Transform>();
        foreach(Transform child in transform)
        {
            if (child.gameObject.tag == "Model")
                models.Add(child);
        }
    }

	void FixedUpdate ()
    {
        if (command.move > 0.1)
            rotationTarget = Quaternion.LookRotation(Vector3.forward);
        else if (command.move < -0.1)
            rotationTarget = Quaternion.LookRotation(Vector3.back);

        foreach (Transform model in models)
            model.rotation = Quaternion.RotateTowards(model.rotation, rotationTarget,angularSpeed*Time.deltaTime);
    }
}
