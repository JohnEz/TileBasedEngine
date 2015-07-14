﻿using UnityEngine;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

	public GameObject[] AbilityNumbers;

	List<GameObject> currentObjects;

	// Use this for initialization
	void Start () {
		currentObjects = new List<GameObject> ();
		DrawAbilities (8);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DrawAbilities(int count) {

		int startX = ((32 * count) / 2) * -1;

		Quaternion rot = transform.rotation;

		for (int i = 0; i < count; ++i) {

			Vector3 pos = new Vector3(startX + (32*i)+16, -300, 0) / 39.625f; // needs to divide because parent idk
			Vector3 scale = new Vector3(1, 1, 1);

			//GameObject go = (GameObject)Instantiate (AbilityNumbers [i], new Vector3(0,0,0), rot);
			GameObject go = (GameObject)Instantiate (AbilityNumbers [i], pos, rot);

			go.transform.SetParent(transform);

			go.transform.localScale = scale;


			currentObjects.Add(go);
		}


	}
}
