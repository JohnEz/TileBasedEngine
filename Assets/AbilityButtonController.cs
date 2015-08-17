using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityButtonController : MonoBehaviour {

	public int slot = -1;
	public bool highlighted = false;

	Sprite defualtSprite;
	Sprite highlightedSprite;

	public string abilName = "";
	public string AbilDescription = "";

	// Use this for initialization
	void Start () {
		defualtSprite = GetComponent<Button> ().spriteState.disabledSprite;
		highlightedSprite = GetComponent<Button> ().spriteState.pressedSprite;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseEnter() {
		GetComponentInParent<UIManager> ().ShowDescription (abilName, AbilDescription);
	}

	public void OnMouseExit() {
		GetComponentInParent<UIManager> ().HideDescription ();
	}

	public void ButtonClicked() {
		if (!highlighted){
			GetComponentInParent<UnitManager>().ShowAbility(slot);
		}
	}

	public void SelectButton() {
		highlighted = true;
		GetComponent<Image>().sprite = highlightedSprite;
	}

	public void UnSelectButton() {
		highlighted = false;
		GetComponent<Image>().sprite = defualtSprite;
	}
}
