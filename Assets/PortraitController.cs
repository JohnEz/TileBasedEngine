using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PortraitController : MonoBehaviour {

	public Unit myUnit;

	public List<GameObject> EffectIcons = new List<GameObject>();

	public void SetUnit(Unit u) {
		myUnit = u;
		GetComponent<Image> ().sprite = u.portait;
	}

	public void UpdateFrame() {
		if (myUnit) {
			if (myUnit.healthBar && !myUnit.isDead) {
				//set the hp
				/////////////
				transform.FindChild ("HPBar").GetComponent<Image> ().fillAmount = myUnit.healthBar.fillAmount;
			
				string hpText = (myUnit.hp + myUnit.shield).ToString () + "/" + (myUnit.maxHP + myUnit.shield).ToString ();
				transform.FindChild ("HPBar").FindChild ("Text").GetComponent<Text> ().text = hpText;
			
				//set the shield
				////////////////

				if (myUnit.healthBar && myUnit.healthBar.fillAmount > 0) {
					Image shieldBar = transform.FindChild ("ShieldBar").GetComponent<Image> ();
					shieldBar.fillAmount = myUnit.shieldBar.fillAmount;
				
					//find the position of the shield
					float hpWidth = transform.FindChild ("HPBar").GetComponent<Image> ().rectTransform.rect.width;
				
					//how far the bar is past its mid point
					float pastMid = ((myUnit.healthBar.fillAmount) - 0.5f) * hpWidth;
				
					//calculate the position for the shield bar
					Vector3 sPos = new Vector3 ((74 + pastMid + (hpWidth * 0.5f)) * -shieldBar.fillOrigin, shieldBar.transform.localPosition.y, shieldBar.transform.localPosition.z);
					shieldBar.transform.localPosition = sPos;
				}
			}
		
			//if the unit uses mana
			///////////////////////
			if (myUnit.manaBar) {
				transform.FindChild ("ManaBar").GetComponent<Image> ().fillAmount = myUnit.manaBar.fillAmount;
			
				string manaText = myUnit.mana.ToString () + "/" + myUnit.maxMana.ToString ();
				transform.FindChild ("ManaBar").FindChild ("Text").GetComponent<Text> ().text = manaText;
			}

			UpdateEffectIcons ();
		}
	}

	public void UpdateEffectIcons() {
		//delete the old icons
		foreach (GameObject go in EffectIcons) {
			Destroy(go, 0);
		}

		EffectIcons = new List<GameObject> ();

		Image shieldBar = transform.FindChild ("ShieldBar").GetComponent<Image> ();
		int flip = 1;
		//if the portait is left sided or right
		flip = -((shieldBar.fillOrigin*2) - 1);


		//for each effect create an icon
		int x = 38 * flip; 	//starting X
		int y = 16;		//starting Y
		int moveX = 32 * flip;	//distance between icons
		int moveY = 32;	//distance between icons
		int count = 0;

		for(int i = 0; i < myUnit.myEffects.Count; ++i) {
			if (myUnit.myEffects[i].visible) {
				EffectIcons.Add(GetComponentInParent<UIManager> ().CreateEffectIcon(x+(count*moveX), y, myUnit.myEffects[i], transform));
				count++;
			}
		}

		for (int i = 0; i < myUnit.myTriggers.Count; ++i) {
			if (myUnit.myTriggers[i].visible) {
				EffectIcons.Add(GetComponentInParent<UIManager> ().CreateEffectIcon(x+(count*moveX), y, myUnit.myTriggers[i], transform));
				count++;
			}
		}

	}
}
