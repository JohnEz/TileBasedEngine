using UnityEngine;
using System.Collections.Generic;

public class PrefabLibrary : MonoBehaviour {

	//visual effects
	public GameObject[] spritePrefabs;
	public Dictionary<string, int> visualEffects = new Dictionary<string, int>();

	//audio effects
	public AudioClip[] audioClips;
	public Dictionary<string, int> soundEffects = new Dictionary<string, int>();

	//projectiles
	public GameObject[] projPrefabs;
	public Dictionary<string, int> projectiles = new Dictionary<string, int>();

	// Use this for initialization
	void Start () {
		AttachAudioEffects ();
		AttachVisualEffects ();
		AttachProjectiles ();
	}

	void AttachVisualEffects() {
		//TODO there has to be a better way of doing this
		// attach strings to indexs of the effects
		visualEffects.Add ("Flash Freeze", 0);
		visualEffects.Add ("Slash1", 1);
		visualEffects.Add ("Word of Healing", 2);
		visualEffects.Add ("Righteous Shield", 3);
		visualEffects.Add ("Divine Sacrifice", 4);
		visualEffects.Add ("Hit1", 5);
		visualEffects.Add ("Triple Shot", 6);
		visualEffects.Add ("Exploit Weakness", 7);
		visualEffects.Add ("Point Blank", 8);
		visualEffects.Add ("Dash", 9);
		visualEffects.Add ("Arcane Pulse", 10);
		visualEffects.Add ("Fireball Explosion", 11);
	}

	void AttachAudioEffects() {
		soundEffects.Add ("Flash Freeze", 0);
		soundEffects.Add ("Blunt1", 1);
		soundEffects.Add ("Blunt2", 2);
		soundEffects.Add ("Blunt3", 3);
		soundEffects.Add ("Sword1", 4);
		soundEffects.Add ("Sword2", 5);
		soundEffects.Add ("Sword3", 6);
		soundEffects.Add ("Sword4", 7);
		soundEffects.Add ("Word of Healing", 8);
		soundEffects.Add ("Point Blank", 9);
		soundEffects.Add ("Lacerate", 10);
		soundEffects.Add ("Crippling Strike", 11);
		soundEffects.Add ("Shield Slam", 12);
		soundEffects.Add ("Arcane Pulse", 13);
		soundEffects.Add ("TripleShot", 14);
		soundEffects.Add ("TripleShot Hit", 15);
		soundEffects.Add ("Fireball Cast", 16);
		soundEffects.Add ("Fireball Hit", 17);

	}

	void AttachProjectiles() {
		projectiles.Add ("Arrow1", 0);
		projectiles.Add ("Crippling Shot", 1);
		projectiles.Add ("Fireball", 2);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public GameObject getVisualEffect(string s) {

		GameObject go = spritePrefabs[visualEffects[s]];
	
		return go;
	}

	public AudioClip getSoundEffect(string s) {
		AudioClip ac = audioClips [soundEffects [s]];

		return ac;
	}

	public GameObject CreateVisualEffect(string s, Vector3 pos, bool flip = false, bool infront = true) {

		if (infront) {
			pos = new Vector3 (pos.x, pos.y, -2.5f);
		} else {
			pos = new Vector3 (pos.x, pos.y, -1.5f);
		}

		GameObject go = (GameObject)Instantiate (spritePrefabs[visualEffects[s]], pos, Quaternion.identity);

		//if the visual effect needs to be flipped
		if (flip) {
			Vector3 theScale = go.transform.localScale;
			theScale.x *= -1;
			go.transform.localScale = theScale;
		}

		return go;
	}

	public GameObject CreateProjectile(string s, Vector3 pos, Vector3 target, float speed) {
		GameObject go = (GameObject)Instantiate (projPrefabs[projectiles[s]], pos, Quaternion.identity);;

		go.GetComponent<ProjectileController> ().Initialise (target, speed);

		return go;
	}
}
