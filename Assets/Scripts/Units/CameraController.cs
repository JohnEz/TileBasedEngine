using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	const float MOVESPEED = 0.2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 move = new Vector3 (0, 0, 0);

	
		if (Input.GetKey(KeyCode.W)) {
			move += new Vector3(0, 1, 0);
		}

		if (Input.GetKey(KeyCode.A)) {
			move -= new Vector3(1, 0, 0);
		}
		if (Input.GetKey(KeyCode.S)) {
			move -= new Vector3(0, 1, 0);
		}
		if (Input.GetKey(KeyCode.D)) {
			move += new Vector3(1, 0, 0);
		}

		move.Normalize ();
		transform.position += (move * MOVESPEED);

	}
}
