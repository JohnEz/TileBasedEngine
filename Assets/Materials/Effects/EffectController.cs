﻿using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour {

	public bool animFinished = false;

	void Start() {

	}

	// Update is called once per frame
	void Update () {
	}

	public void EndAnimation() {
		animFinished = true;
		Destroy(gameObject, 0); 
	}
}