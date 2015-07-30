using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject[] AbilityNumbers;
	public GameObject CombatText;

	List<GameObject> currentObjects;

	// Use this for initialization
	void Start () {
		currentObjects = new List<GameObject> ();
		DrawAbilities (3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DrawAbilities(int count) {

		int startX = ((32 * count) / 2) * -1;

		Quaternion rot = transform.rotation;

		for (int i = 0; i < count; ++i) {


			float y = GetComponent<RectTransform>().rect.height / 2 - 17;

			Vector3 pos = new Vector3(startX + (32*i)+16, -y, 0); // needs to divide because parent idk

			//GameObject go = (GameObject)Instantiate (AbilityNumbers [i], new Vector3(0,0,0), rot);
			GameObject go = (GameObject)Instantiate (AbilityNumbers [i], pos, rot);



			go.transform.SetParent(transform);
			go.transform.localPosition = pos;
			go.transform.localScale = new Vector3(1, 1, 1);


			currentObjects.Add(go);
		}


	}

}
