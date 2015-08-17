using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	const float MOVESPEED = 12; //speed the camera moves
	const float ZOOMSPEED = 0.5f; //speed the camera zooms
	const float MINZOOM = 2f; //minimum zoom value
	const float MAXZOOM = 8f; //maximum zoom value
	const float BOUNDARY = 5; //distance from edge of screen that the camera starts to move

	public float screenWidth = 0;
	public float screenHeight = 0;

	public bool movingToDestination = false;
	Vector3 targetLocation;

	// Use this for initialization
	void Start () {
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMovement ();
		UpdateZoom ();
	}

	//updates the position of the camera via keyboard input
	void UpdateMovement() {
		//if the camera is moving to a destination
		if (movingToDestination) {

			if (Vector3.Distance (transform.position, targetLocation) < 0.2f) {
				movingToDestination = false;
			}
			//Smoothly animate towards the correct location
			transform.position = Vector3.Lerp (transform.position, targetLocation, 6f * Time.deltaTime);
		} else {
			//allow user to move camera
			Vector3 move = new Vector3 (0, 0, 0);
			
			
			if (Input.GetKey(KeyCode.W) || Input.mousePosition.y > screenHeight - BOUNDARY) {
				move += new Vector3(0, 1, 0);
			}
			if (Input.GetKey(KeyCode.A) || Input.mousePosition.x < BOUNDARY) {
				move -= new Vector3(1, 0, 0);
			}
			if (Input.GetKey(KeyCode.S) || Input.mousePosition.y < BOUNDARY) {
				move -= new Vector3(0, 1, 0);
			}
			if (Input.GetKey(KeyCode.D) || Input.mousePosition.x > screenWidth - BOUNDARY) {
				move += new Vector3(1, 0, 0);
			}
			
			move.Normalize ();
			transform.position += (move * MOVESPEED) * Time.deltaTime;
		}
	}

	void UpdateZoom() {
		float d = Input.GetAxis("Mouse ScrollWheel");


		Camera cam = GetComponent<Camera> ();

		if (d > 0f)
		{
			//zoom in
			cam.orthographicSize -= ZOOMSPEED;

			if (cam.orthographicSize < MINZOOM) {
				cam.orthographicSize = MINZOOM;
			}
		}
		else if (d < 0f)
		{
			//zoom out
			cam.orthographicSize += ZOOMSPEED;
			
			if (cam.orthographicSize > MAXZOOM) {
				cam.orthographicSize = MAXZOOM;
			}
		}
	}


	public void MoveToTarget(Vector3 pos) {
		movingToDestination = true;
		targetLocation = new Vector3 (pos.x, pos.y, transform.position.z);
	}
}
