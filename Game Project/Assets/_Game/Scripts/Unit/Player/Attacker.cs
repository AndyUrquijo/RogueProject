using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attacker : AbstractBehaviour {

	public List<string> targets;
	public List<GameObject> attacks;
    private Transform model = null;

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "Model")
            {
                model = child;
                break;
            }
        }
    }

    void Update () {
        int attack = command.attack - 1;
        if (attack != -1 && attacks[attack] != null) 
			BeginAttack (attacks [attack]);
	}

	public void BeginAttack( GameObject attackObj )
	{
		GameObject instance = GameObject.Instantiate(attackObj);
		instance.transform.position = transform.position;
        if(model)
		    instance.transform.parent = model.transform;
        else
            instance.transform.parent = gameObject.transform;
		command.attack = 0;
		
		Attack attack = instance.GetComponent<Attack>();
		if (attack) {
			attack.targets = targets;
            attack.owner = gameObject;
			if (attack.duration == 0) {
				Animator anim = instance.GetComponent<Animator> ();
				float length = anim.runtimeAnimatorController.animationClips [0].length;
				attack.duration = length;
			}
		}
	}
}
