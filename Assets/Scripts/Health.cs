using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	GameObject bar;
	public Vector3 offset;
	public float max = 100;
	float health;

	// Use this for initialization
	void Start () {
		health = max;
		bar = (GameObject)Instantiate(Resources.Load("HealthBar"));
		bar.transform.parent = transform;
		bar.transform.position = transform.position + offset;
	}

	void Update() {
		bar.transform.rotation = Camera.main.transform.rotation;
	}

	public void TakeDamage(float damage) {
		health -= damage;
		if (health <= 0) {
			Controller cont = GetComponent<Controller>();
			cont.Death();
		}
		else if (health > max)
			health = max;

		bar.transform.localScale = new Vector3(health/max,1,1);
	}

	public float CurrentHealth() {
		return health;
	}
}
