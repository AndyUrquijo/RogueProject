using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour {

	public GameObject unit;
	private Health health;
	private GameObject fill;
	private float offset;
	// Use this for initialization
	void Start () {
		if (!unit) 
			unit = transform.parent.gameObject;

		health = unit.GetComponentInChildren<Health> ();
		if (!health) 
			health = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<Health> ();

		fill = transform.Find ("Fill").gameObject;
		SpriteRenderer sprite = fill.GetComponent<SpriteRenderer> ();
		if(sprite)
			offset = sprite.bounds.extents.x;
	}
	
	// Update is called once per frame
	void Update () {
		float ratio = (float)health.health / health.maxHealth;
		Vector3 scale = fill.transform.localScale;
		scale.x = ratio;
		fill.transform.localScale = scale;

		if (offset != 0) {
			Vector3 position = fill.transform.parent.position;
			position.x -= offset * (1 - ratio);
			fill.transform.position = position;
		}
	}
}
