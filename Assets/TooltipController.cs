using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour {

	public bool showing = false;
	public Image box;
	public Text title;
	public Text description;

	// Use this for initialization
	void Start () {
		box = GetComponent<Image> ();
		title = transform.FindChild ("AbilityName").GetComponent<Text> ();
		description = transform.FindChild ("AbilityDescription").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (showing) {
			RectTransform rect = GetComponent<RectTransform> ();
			float x;
			if (Input.mousePosition.x <= Screen.width / 2) {
				x = Input.mousePosition.x + (rect.rect.width / 2) + 4;
			} else {
				x = Input.mousePosition.x - (rect.rect.width / 2) - 4;
			}
			float y = Input.mousePosition.y - (rect.rect.height / 2) - 8;

			Vector3 newPosition = new Vector3(x, y, 0);
			transform.position = newPosition;
			ResizeBox();
		}
	}

	public void ShowTooltip(bool b) {
		showing = b;
		box.enabled = b;
		title.enabled = b;
		description.enabled = b;
	}

	public void SetTitle(string s) {
		title.text = s;
	}

	public void SetDescription(string s) {
		description.text = s;
	}

	public void ResizeBox() {
		float test = description.GetComponent<RectTransform> ().rect.height;
		GetComponent<RectTransform> ().sizeDelta = new Vector2(240, test + 32);
		Vector3 titlePos = title.transform.localPosition;
		titlePos.y = GetComponent<RectTransform> ().rect.height / 2 - 22;
		title.transform.localPosition = titlePos;
	}
}
