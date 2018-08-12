using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Attack : MonoBehaviour {

	CapsuleCollider hurtbox;
	public int damage = 10;
	public int total_frames = 40;
	public int start_frame = 10;
	public int end_frame = 30;
	int left;

	void OnTriggerStay(Collider col) {
		if (left >= start_frame && left < end_frame) {
			GameObject hit = col.gameObject;
			if (hit.layer != gameObject.layer) {
				if (hit.GetComponents<Health>().Length != 0) {
					Health h = hit.GetComponent<Health>();
					h.TakeDamage(damage);
					left = end_frame;
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		hurtbox = GetComponent<CapsuleCollider>();
		left = end_frame;
	}

	void Update () {
		if (left < end_frame) left++;
	}

	public void Begin() {
		left = 0;
	}
}
