using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public int maxHealth;
	public int health;
	// Use this for initialization
	void Start () {
		health = maxHealth;

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void TakeDamage(int damage)
	{

		health -= damage;
		if (health <= 0)
			GameObject.Destroy (gameObject);

	}
}
