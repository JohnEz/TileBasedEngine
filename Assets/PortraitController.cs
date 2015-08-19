using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour {

	public Vector3 targetPos;
	public float targetAlpha;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 curAlpha = new Vector2 (GetComponent<Image> ().color.a, 0);
		Vector2 tarAlpha = new Vector2 (targetAlpha, 0);

		// Smoothly animate towards the correct slot.
		if (Vector3.Distance (transform.localPosition, targetPos) > 0.1f) {
			transform.localPosition = Vector3.Lerp (transform.localPosition, targetPos, 12f * Time.deltaTime); 

			curAlpha = Vector2.Lerp(curAlpha, tarAlpha, 12f * Time.deltaTime);
			GetComponent<Image> ().color = new Color(1, 1, 1, curAlpha.x);
		} else {
			transform.localPosition = targetPos;
			GetComponent<Image> ().color = new Color(1, 1, 1, targetAlpha);
		}

	}
}
