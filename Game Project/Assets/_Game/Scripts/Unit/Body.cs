using UnityEngine;
using System.Collections;

public class Body : AbstractBehaviour {

	[HideInInspector]
	public new Rigidbody2D rigidbody;
	[HideInInspector]
	public new Collider2D collider;

	override public void Initialize() {
        base.Initialize();
		rigidbody = GetComponent<Rigidbody2D> ();
		collider = transform.FindChild ("Body").GetComponent<Collider2D> ();
	}
	
}
