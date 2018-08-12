using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MovementInfo))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : Controller {

	Animator anim;
	GameWorld world;
	BoxCollider collider;
	Rigidbody body;
	MovementInfo info;
	int cooldown;
	public float radius = 10;
	Attack attack;

	// Use this for initialization
	void Start () {
		cooldown = 0;
		world = GameObject.Find("World").GetComponent<GameWorld>();
		body = GetComponent<Rigidbody>();
		info = GetComponent<MovementInfo>();
		anim = GetComponentInChildren<Animator>();
		collider = GetComponentInChildren<BoxCollider>();
		attack = GetComponentInChildren<Attack>();
	}

	bool IsGrounded() {
		RaycastHit hit;
		Vector3 center = transform.position + collider.center;
		Vector3 offset = collider.size/2;
		for (int i = 0; i < 4; i++) {
			Vector3 origin = center + Vector3.up * 0.1f + Vector3.Scale(offset, new Vector3((i&1)<<1 - 1, -1, (i&2)-1));
			if (Physics.Raycast(origin, -Vector3.up, out hit, 0.2f))
				return true;
		}
		return false;
	}

	void RetargetCamera(GameObject targetObject) {
		Transform camera = transform.Find("Main Camera");
		Vector3 camoff = camera.localPosition;
		camera.parent = targetObject.transform;
		camera.localPosition = camoff;
		camera.localRotation = Quaternion.identity;
	}

	void SetLayerRecursively(GameObject obj, int newLayer) {
        if (obj == null) return;

        obj.layer = newLayer;
        foreach (Transform child in obj.transform) {
            if (null == child) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

	void ChangeLayerRecursively(GameObject obj) {
		obj.layer = gameObject.layer;
		foreach (Transform t in obj.GetComponentsInChildren<Transform>()) {
			ChangeLayerRecursively(t.gameObject);
		}
	}

	GameObject ClosestToTakeOver() {
		int enemy_mask = 1 << LayerMask.NameToLayer("Enemy");
		Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemy_mask);

		GameObject totake = null;
		float distance = -1;
		foreach(Collider col in hits) {
			if (col.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
				Health h = col.gameObject.GetComponent<Health>();
				if (h != null && h.max == 100 && h.CurrentHealth() < 50) {
					float dist = (transform.position - col.gameObject.transform.position).magnitude;
					if (distance == -1 || distance > dist) {
						distance = dist;
						totake = col.gameObject;
					}
				}
			}
		}
		return totake;
	}

	// Update is called once per frame
	void Update () {
		if (!world.paused) {
			anim.speed = 1;
			transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * info.v_rotation);

			Vector3 add = new Vector3(0,0,0);
			anim.SetBool("forward", false);
			anim.SetBool("backward", false);
			anim.SetBool("left", false);
			anim.SetBool("right", false);
			if (Input.GetKey("w")) {
				anim.SetBool("forward", true);
				add = info.v_forward * transform.forward;
			}
			else if (Input.GetKey("s")) {
				anim.SetBool("backward", true);
				add = -info.v_backward * transform.forward;
			}

			Vector3 left = Vector3.Cross(transform.forward, transform.up);
			if (Input.GetKey("a")) {
				anim.SetBool("left", true);
				add += info.v_left * left;
			}
			else if (Input.GetKey("d")) {
				anim.SetBool("right", true);
				add += -info.v_right * left;
			}

			RaycastHit hit;
			add.y = body.velocity.y;
			if(Input.GetKey("space") && IsGrounded()) {
				anim.SetTrigger("jump");
				add.y = info.v_jump;
			}

			if (Input.GetMouseButtonDown(0) && cooldown == 0) {
				anim.SetTrigger("attack");
				attack.Begin();
				cooldown = attack.total_frames;
			}

			if (Input.GetMouseButtonDown(1)) {
				if (gameObject == world.player) {
					GameObject target = ClosestToTakeOver();
					print(target.name);
					if (target != null) {
						Destroy(target.GetComponent<Controller>());
						target.AddComponent<PlayerController>();
						RetargetCamera(target);
						body.velocity = new Vector3(0,0,0);
						transform.position = new Vector3(0,-1000,0);
						anim.speed = 0;
						SetLayerRecursively(target, gameObject.layer);
						world.current_player = target;
						Destroy(gameObject.GetComponent<PlayerController>());
					}
				}
				else {
					world.player.transform.position = transform.position;
					world.player.transform.rotation = transform.rotation;
					world.player.AddComponent<PlayerController>();
					RetargetCamera(world.player);
					world.current_player = world.player;
					Destroy(gameObject);
				}
			}

			body.velocity = add;
			anim.SetBool("grounded", IsGrounded());
			if (cooldown > 0) cooldown--;
			if (gameObject != world.player) GetComponent<Health>().TakeDamage(0.05f);
		}
		else {
			anim.speed = 0;
			body.Sleep();
		}
	}

	public override void Death() {
		if (gameObject != world.player) {
			world.player.transform.position = transform.position;
			world.player.transform.rotation = transform.rotation;
			world.player.AddComponent<PlayerController>();
			RetargetCamera(world.player);
			world.current_player = world.player;
			Destroy(gameObject);
		}
		else {
			SceneManager.LoadScene("Lost", LoadSceneMode.Single);
		}
	}
}
