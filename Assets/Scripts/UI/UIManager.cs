using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject[] AbilityNumbers;
	public GameObject errorText;

	List<GameObject> currentIcons = new List<GameObject> ();
	List<GameObject> currentIconFrames = new List<GameObject> ();

	public PrefabLibrary prefabs;

	public Image descriptionBox;
	public Text abilityNameText;
	public Text abilityDescText;

	public Image turnOrderFrame;
	public Image currentTurnFrame;
	public GameObject portaitPrefab;
	public GameObject effectIconPrefab;

	public GameObject[] UnitFrames = new GameObject[6];

	public List<GameObject> currentPortaits = new List<GameObject>();

	// Use this for initialization
	public void Initialise () {
		//set the position of the description box and text
		descriptionBox = transform.FindChild ("AbilityDescriptionBox").GetComponent<Image> ();
		abilityNameText = descriptionBox.transform.FindChild ("AbilityName").GetComponent<Text> ();
		abilityDescText = descriptionBox.transform.FindChild ("AbilityDescription").GetComponent<Text> ();

		float y = GetComponent<RectTransform> ().rect.height / 2 - 130;
		Vector3 pos = new Vector3 (0, -y, 0);
		descriptionBox.transform.localPosition = pos;

		//hide the box until it needs to be seen
		HideDescription ();

		//set the position of the turn order frame
		turnOrderFrame = transform.FindChild ("TurnOrderFrame").GetComponent<Image> ();
		currentTurnFrame = transform.FindChild ("CurrentTurnFrame").GetComponent<Image> ();

		y = GetComponent<RectTransform> ().rect.height / 2 - 32;
		pos = new Vector3 (0, y, 0);

		turnOrderFrame.transform.localPosition = pos;
		currentTurnFrame.transform.localPosition = pos;

		UnitFrames[0] = transform.FindChild ("UnitFrame1").gameObject;
		UnitFrames[1] = transform.FindChild ("UnitFrame2").gameObject;
		UnitFrames[2] = transform.FindChild ("UnitFrame3").gameObject;
		UnitFrames[3] = transform.FindChild ("UnitFrame4").gameObject;
		UnitFrames[4] = transform.FindChild ("UnitFrame5").gameObject;

		UnitFrames[5] = transform.FindChild ("UnitFrameSelected").gameObject;
		UnitFrames[5].SetActive (false);
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
			if (u.myAbilities[i] != null) {
				if (u.myAbilities[i].Name != "") {
					CreateIcon(u.myAbilities[i], pos, i);
				}
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
	void CreateIcon(Ability abil, Vector3 pos, int slot) {
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

		AbilityButtonController controller = go.GetComponent<AbilityButtonController> ();

		controller.slot = slot;
		controller.abilName = abil.Name;
		controller.AbilDescription = abil.description;

		currentIcons.Add(go);
	}

	public void HighlightIcon(int ind) {

		AbilityButtonController controller;

		for (int i = 0; i < currentIcons.Count; ++i) {
			controller = currentIcons[i].GetComponent<AbilityButtonController>();

			//if its not the selected icon
			if (i != ind) {
				controller.UnSelectButton();
			} else {
				controller.SelectButton();
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

	public void HideDescription() {
		descriptionBox.enabled = false;
		abilityNameText.enabled = false;
		abilityDescText.enabled = false;
	}

	public void ShowDescription(string name, string desc) {
		descriptionBox.enabled = true;
		abilityNameText.enabled = true;
		abilityDescText.enabled = true;
		abilityNameText.text = name;
		abilityDescText.text = desc;

	}
	
	public void ChangeRound(List<GameObject> currentTurnOrder) {
		//delete old portaits
		foreach (GameObject go in currentPortaits) {
			go.GetComponent<TurnPortraitController>().targetAlpha = 0;
			Destroy(go, 1f);
		}

		currentPortaits = new List<GameObject> ();

		//create new portraits
		for (int i = 0; i < currentTurnOrder.Count; ++i) {

			// find the position
			Vector3 pos = new Vector3(i * 48, 1, 0);

			//create the object
			GameObject go = (GameObject)Instantiate(portaitPrefab, new Vector3(0, 0, 0), Quaternion.identity);

			//set the image and position
			go.transform.SetParent(turnOrderFrame.transform);
			go.transform.localPosition = pos;
			go.GetComponent<Image>().sprite = currentTurnOrder[i].GetComponent<Unit>().portait;

			//find alpha depending distance from the current turn
			float alpha = Mathf.Max(0, 1-(i*0.25f));

			go.GetComponent<Image>().color = new Color(1, 1, 1 , 0);
			go.GetComponent<TurnPortraitController>().targetAlpha = alpha;
			go.GetComponent<TurnPortraitController>().targetPos = pos;

			currentPortaits.Add(go);

		}

	}

	public void ChangeTurn(int turn) {

		for (int i = 0; i < currentPortaits.Count; ++i) {
			int diff = i - turn;

			// find the position
			Vector3 pos = new Vector3(diff * 48, 1, 0);

			//find alpha depending distance from the current turn
			float alpha = Mathf.Max(0, 1-(Mathf.Abs(diff)*0.25f));

			currentPortaits[i].GetComponent<TurnPortraitController>().targetPos = pos;
			currentPortaits[i].GetComponent<TurnPortraitController>().targetAlpha = alpha;
		}

	}

	public void SetupUnitFrames(GameObject[] characters) {
		for (int i = 0; i < characters.Length; ++i) {
			SetUnitFrame(i, characters[i].GetComponent<Unit>());
		}
	}

	public void SetSelectedFrame(Unit u) {
		SetUnitFrame (5, u);
		UnitFrames [5].SetActive (true);
		UnitFrames [5].GetComponent<PortraitController>().SetUnit(u);
	}

	public void DeselectSelectedUnit() {
		UnitFrames [5].SetActive (false);
	}

	public void SetUnitFrame(int index, Unit u) {
		UpdateUnitFrame (index, u);
	}

	public void UpdateUnitFrame(int index, Unit u) {
		if (index >= 0 && index < 6) {
			GameObject go = UnitFrames [index];
			if (u.healthBar) {
				//set the hp
				/////////////
				go.transform.FindChild ("HPBar").GetComponent<Image> ().fillAmount = u.healthBar.fillAmount;

				string hpText = (u.hp + u.shield).ToString () + "/" + (u.maxHP + u.shield).ToString ();
				go.transform.FindChild ("HPBar").FindChild ("Text").GetComponent<Text> ().text = hpText;

				//set the shield
				////////////////
				Image shieldBar = go.transform.FindChild ("ShieldBar").GetComponent<Image> ();
				shieldBar.fillAmount = u.shieldBar.fillAmount;

				//find the position of the shield
				float hpWidth = go.transform.FindChild ("HPBar").GetComponent<Image> ().rectTransform.rect.width;

				//how far the bar is past its mid point
				float pastMid = ((u.healthBar.fillAmount) - 0.5f) * hpWidth;
			
				//calculate the position for the shield bar
				Vector3 sPos = new Vector3 ((74 + pastMid + (hpWidth * 0.5f)) * (shieldBar.fillOrigin *-1), shieldBar.transform.localPosition.y, shieldBar.transform.localPosition.z);
				shieldBar.transform.localPosition = sPos;
			}

			//if the unit uses mana
			///////////////////////
			if (u.manaBar) {
				go.transform.FindChild ("ManaBar").GetComponent<Image> ().fillAmount = u.manaBar.fillAmount;

				string manaText = u.mana.ToString () + "/" + u.maxMana.ToString ();
				go.transform.FindChild ("ManaBar").FindChild ("Text").GetComponent<Text> ().text = manaText;
			}
		}

	}


}
