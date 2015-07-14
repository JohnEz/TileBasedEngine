using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public TileMap map;

	// Use this for initialization
	void Start () {
		map.Initialise ();
		GetComponent<UnitManager> ().Initialise ();
	}

}
