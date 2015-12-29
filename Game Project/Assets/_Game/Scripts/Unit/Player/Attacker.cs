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
		instance.transform.position += transform.position;
		instance.transform.rotation = transform.rotation;
		instance.transform.parent = gameObject.transform;
        
		if(model)
		{
		    instance.transform.parent = model.transform;
			instance.transform.rotation = model.rotation;
		}
		command.attack = 0;
		
		AttackAction attackAction = instance.GetComponent<AttackAction>();
		if (attackAction) {
			attackAction.targets = targets;
			attackAction.owner = gameObject;

			attackAction.StartAttack();
		}
	}
}
