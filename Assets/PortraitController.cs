using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour {

	public Unit myUnit;

	public void SetUnit(Unit u) {
		GetComponent<Image> ().sprite = u.portait;
	}

	public void UpdateFrame() {
		if (myUnit.healthBar) {
			//set the hp
			/////////////
			transform.FindChild ("HPBar").GetComponent<Image> ().fillAmount = myUnit.healthBar.fillAmount;
			
			string hpText = (myUnit.hp + myUnit.shield).ToString () + "/" + (myUnit.maxHP + myUnit.shield).ToString ();
			transform.FindChild ("HPBar").FindChild ("Text").GetComponent<Text> ().text = hpText;
			
			//set the shield
			////////////////
			Image shieldBar = transform.FindChild ("ShieldBar").GetComponent<Image> ();
			shieldBar.fillAmount = myUnit.shieldBar.fillAmount;
			
			//find the position of the shield
			float hpWidth = transform.FindChild ("HPBar").GetComponent<Image> ().rectTransform.rect.width;
			
			//how far the bar is past its mid point
			float pastMid = ((myUnit.healthBar.fillAmount) - 0.5f) * hpWidth;
			
			//calculate the position for the shield bar
			Vector3 sPos = new Vector3 ((74 + pastMid + (hpWidth * 0.5f)) * (shieldBar.fillOrigin *-1), shieldBar.transform.localPosition.y, shieldBar.transform.localPosition.z);
			shieldBar.transform.localPosition = sPos;
		}
		
		//if the unit uses mana
		///////////////////////
		if (myUnit.manaBar) {
			transform.FindChild ("ManaBar").GetComponent<Image> ().fillAmount = myUnit.manaBar.fillAmount;
			
			string manaText = myUnit.mana.ToString () + "/" + myUnit.maxMana.ToString ();
			transform.FindChild ("ManaBar").FindChild ("Text").GetComponent<Text> ().text = manaText;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
