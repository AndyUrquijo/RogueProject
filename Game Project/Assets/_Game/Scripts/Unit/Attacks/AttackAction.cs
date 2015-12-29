using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AttackAction : MonoBehaviour {

	public List<string> targets;
	[HideInInspector]
	public GameObject owner;

	public abstract void StartAttack();
}
