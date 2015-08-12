using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	Vector3 startPos;
	Vector3 targetPos;
	Vector3 direction;
	float speed;

	static int projectileCounter = -1;

	public bool reachedTarget = false;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}

	public void Initialise(Vector3 target, float spd) {
		startPos = transform.position;
		targetPos = target;
		speed = spd;


		direction = targetPos - startPos;
		direction.Normalize();

		var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = rotation;

		//transform.rotation = Quaternion.LookRotation (direction);
		//transform.LookAt(direction);
	}

	// Update is called once per frame
	void Update () {

		float dis = Vector3.Distance (transform.position, targetPos);

		// check how close this is to target location
		if (dis < speed * Time.deltaTime) {
			// it has reached its target
			reachedTarget = true;
		} else {
			//move towards target
			transform.position += (direction * speed) * Time.deltaTime;
		}
	}
}
