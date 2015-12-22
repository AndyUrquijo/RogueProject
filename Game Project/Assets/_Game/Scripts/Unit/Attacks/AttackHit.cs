using UnityEngine;
using System.Collections;

public class AttackHit : MonoBehaviour {

	private Attack attack;
	void Start () {
		attack = GetComponentInParent<Attack>();
	}
	
	void OnTriggerEnter2D(Collider2D other) {

        GameObject unit = other.transform.parent.gameObject; // Hitboxes are assumed to be children of main object.
		if (!unit)
			return;
		
		attack.Hit(unit);
    }
	
}
