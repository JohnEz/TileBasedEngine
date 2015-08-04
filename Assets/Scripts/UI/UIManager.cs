﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject[] AbilityNumbers;
	public GameObject errorText;

	List<GameObject> currentObjects = new List<GameObject> ();

	List<GameObject> currentIcons = new List<GameObject> ();
	List<GameObject> currentIconFrames = new List<GameObject> ();

	public PrefabLibrary prefabs;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DrawAbilities(int count, Unit u) {

		ClearOldAbilities ();

		int startX = ((32 * count) / 2) * -1;

		for (int i = 0; i < count; ++i) {
		
			float y = GetComponent<RectTransform>().rect.height / 2 - 17;
			Vector3 pos = new Vector3(startX + (32*i)+16, -y, 0);

			CreateIconFrame(i, pos);
			if (u.myAbilities[i].Name != "") {
				CreateIcon(u.myAbilities[i], pos);
			}
		}


	}

	void ClearOldAbilities() {
		//destroy icons
		foreach (GameObject go in currentIcons) {
			Destroy(go, 0);
		}
		currentIcons = new List<GameObject> ();

		//destroy frames
		foreach (GameObject go in currentIcons) {
			Destroy(go, 0);
		}
		currentIcons = new List<GameObject> ();
	}

	//creates a numbered icon frame
	void CreateIconFrame(int i, Vector3 pos) {

		GameObject go = (GameObject)Instantiate (AbilityNumbers [i], pos, transform.rotation);
		
		
		go.transform.SetParent(transform);
		go.transform.localPosition = pos;
		go.transform.localScale = new Vector3(1, 1, 1);
		
		currentIconFrames.Add(go);
	}

	//creates an icon at the location, s looks for a prefab
	void CreateIcon(Ability abil, Vector3 pos) {
		Vector3 position = pos + new Vector3 (0, 32, 0);

		GameObject go = (GameObject)Instantiate (prefabs.getIcon(abil.Name), position, transform.rotation);

		go.transform.SetParent(transform);
		go.transform.localPosition = position;
		go.transform.localScale = new Vector3(1, 1, 1);

		// if the unit doesnt have enough mana, darken icon
		if (abil.myCaster.mana < abil.manaCost || (abil.usesCombo && abil.myCaster.comboPoints < 1)) {
			go.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
		} else {
			go.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
		}

		//show cooldown, dont show if its 0
		if (abil.cooldown > 0) {
			go.transform.FindChild ("CooldownText").GetComponent<Text> ().text = abil.cooldown.ToString();
		} else {
			go.transform.FindChild ("CooldownText").GetComponent<Text>().text = "";
		}



		currentIcons.Add(go);
	}

	public void HighlightIcon(int ind) {
		for (int i = 0; i < currentIcons.Count; ++i) {
			//if its not the selected icon
			if (i != ind) {
				if (currentIcons[i].GetComponent<Image>().color == new Color(1, 1, 1)) {
					currentIcons[i].GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
				}
			} else {
				//highlight icon
				currentIcons[i].GetComponent<Image>().color = new Color(1, 1, 1);
			}
		}
	}

	public void ShowErrorText(string s) {
		GameObject temp = Instantiate (errorText) as GameObject;
		RectTransform tempRect = temp.GetComponent<RectTransform> ();
		temp.transform.SetParent (transform);

		float y = GetComponent<RectTransform>().rect.height / 3;
		Vector3 pos = new Vector3 (0, y, 0);

		tempRect.transform.localPosition = pos;
		tempRect.transform.localScale = errorText.transform.localScale;
		tempRect.transform.rotation = errorText.transform.localRotation;
		
		temp.GetComponent<Text> ().text = s;
		Destroy(temp.gameObject, 2); 
	}

}