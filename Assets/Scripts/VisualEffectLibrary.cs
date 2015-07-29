using UnityEngine;
using System.Collections.Generic;

public class VisualEffectLibrary : MonoBehaviour {

	public GameObject[] sprites;
	public Dictionary<string, int> effects = new Dictionary<string, int>();

	// Use this for initialization
	void Start () {
		//TODO there has to be a better way of doing this
		//
		effects.Add ("Flash Freeze", 0);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public GameObject getEffect(string s) {

		GameObject go = sprites[effects[s]];
	
		return go;
	}

	public GameObject CreateEffect(string s, Vector3 pos) {
		GameObject go = (GameObject)Instantiate (sprites[effects[s]], pos, Quaternion.identity);
		
		return go;
	}
}
