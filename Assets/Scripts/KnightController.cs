using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MovementInfo))]
[RequireComponent(typeof(BoxCollider))]
public class KnightController : Controller {

	Animator anim;
	GameWorld world;
	BoxCollider collider;
	Rigidbody body;
	MovementInfo info;
	public Attack dagger;
	public Attack maul;
	int cooldown;
	int cdtime;
	public float radius = 10;
	public float attack_range = 1;
	bool alive;
	bool right_swing;

	// Use this for initialization
	void Start () {
		cooldown = 0;
		world = GameObject.Find("World").GetComponent<GameWorld>();
		body = GetComponent<Rigidbody>();
		info = GetComponent<MovementInfo>();
		anim = GetComponentInChildren<Animator>();
		collider = GetComponentInChildren<BoxCollider>();
		alive = true;
		right_swing = false;
	}

	bool IsGrounded() {
		RaycastHit hit;
		Vector3 center = transform.position + collider.center;
		Vector3 offset = collider.size/2;
		for (int i = 0; i < 4; i++) {
			Vector3 origin = center + Vector3.up * 0.1f + Vector3.Scale(offset, new Vector3((i&1)<<1 - 1, -1, (i&2)-1));
			Debug.DrawRay(origin, -Vector3.up*0.2f, Color.red);
			if (Physics.Raycast(origin, -Vector3.up, out hit, 0.2f))
				return true;
		}
		return false;
	}

	// Update is called once per frame
	void Update () {
		if (!alive) {
			if (cooldown > 0) cooldown--;
			else Destroy(gameObject);
			return;
		}

		if (!world.paused) {
			anim.speed = 1;

			Vector3 player_pos = world.current_player.transform.position;
			if ((transform.position - player_pos).magnitude > radius) {
				anim.SetBool("forward", false);
				anim.SetBool("backward", false);
				return;
			}

			// move forward
			Vector3 add = new Vector3(0,0,0);
			if (cooldown == 0) {
				if ((transform.position - player_pos).magnitude < attack_range) {
					anim.SetBool("forward", false);
					if (right_swing) {
						anim.SetTrigger("dagger");
						dagger.Begin();
						cdtime = 2*dagger.total_frames+20;
						cooldown = cdtime;
					}
					else {
						anim.SetTrigger("maul");
						maul.Begin();
						cdtime = 2*maul.total_frames+20;
						cooldown = cdtime;
					}
					right_swing = !right_swing;
				}
				else {
					Vector3 target = new Vector3(player_pos.x, transform.position.y, player_pos.z);
					transform.LookAt(target, Vector3.up);
					anim.SetBool("backward", false);
					anim.SetBool("forward", true);
					add = info.v_forward * transform.forward;
				}
			}
			else if (cooldown < cdtime/2 + 10){
				Vector3 target = new Vector3(player_pos.x, transform.position.y, player_pos.z);
				transform.LookAt(target, Vector3.up);
				anim.SetBool("backward", true);
				add = -info.v_backward * transform.forward;
			}
			add.y = body.velocity.y;

			body.velocity = add;
			if (cooldown > 0) cooldown--;
		}
		else if (alive) {
			anim.speed = 0;
			body.Sleep();
		}
	}

	public override void Death() {
		anim.SetTrigger("death");
		alive = false;
		cooldown = 40;
	}
}
