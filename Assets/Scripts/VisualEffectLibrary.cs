using UnityEngine;
using System.Collections.Generic;

public class VisualEffectLibrary : MonoBehaviour {

	public GameObject[] sprites;
	public Dictionary<string, int> effects = new Dictionary<string, int>();

	// Use this for initialization
	void Start () {
		//TODO there has to be a better way of doing this
		// attach strings to indexs of the effects
		effects.Add ("Flash Freeze", 0);
		effects.Add ("Slash1", 1);
		effects.Add ("Word of Healing", 2);
		effects.Add ("Righteous Shield", 3);
		effects.Add ("Divine Sacrifice", 4);
		effects.Add ("Hit1", 5);
		effects.Add ("Triple Shot", 6);
		effects.Add ("Exploit Weakness", 7);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public GameObject getEffect(string s) {

		GameObject go = sprites[effects[s]];
	
		return go;
	}

	public GameObject CreateEffect(string s, Vector3 pos) {

		pos = new Vector3(pos.x, pos.y, -2.5f);

		GameObject go = (GameObject)Instantiate (sprites[effects[s]], pos, Quaternion.identity);
		
		return go;
	}
}
