using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public enum CharacterClass {
	DemoUnit,
	Warrior,
	Ranger,
	MAXCLASSES
}

public enum EnemyClass {
	Goblin,
	MAXCLASSES
}

public class UnitManager : MonoBehaviour {
	//files to load
	public TextAsset enemiesXml;
	public TextAsset playersUnitsXml;

	//the map manager
	public TileMap map;

	//turn order
	List<GameObject> currentQueue;
	List<GameObject> activeUnits;
	public int turn = 0;

	//different enemy types
	public GameObject[] enemyTypes;
	public GameObject[] enemies;
	public int MAXENEMIES = 100;
	public int enemyCount = 0;

	//the players units
	public int MAXCHARACTERS = 5;
	public int characterCount = 0;
	public GameObject[] classes;
	public GameObject[] playerUnitObjects;




	public void Initialise() {
		activeUnits = new List<GameObject> ();

		enemies = new GameObject[MAXENEMIES];
		playerUnitObjects = new GameObject[MAXCHARACTERS];
		loadEnemies ();
		loadCharacters ();

		//temp
		spawnUnit (1, 1, CharacterClass.DemoUnit);
		spawnUnit (2, 2, CharacterClass.Warrior);
		spawnUnit (2, 3, CharacterClass.Ranger);

		spawnEnemy (13, 3, EnemyClass.Goblin);

		currentQueue = activeUnits;

		NextUnitsTurn ();
	}

	//temp
	void spawnUnit(int x, int y, CharacterClass c) {
		if (characterCount < MAXCHARACTERS) {
			Vector3 pos = map.TileCoordToWorldCoord (x, y);

			GameObject go = (GameObject)Instantiate (classes [(int)c], pos, Quaternion.identity);

			playerUnitObjects [characterCount] = go;

			Unit u = go.GetComponent<Unit>();

			u.tileX = x;
			u.tileY = y;
			u.map = map;
			++characterCount;

			map.GetNode(x, y).myUnit = u;

			activeUnits.Add(go);
		}

	}

	void spawnEnemy(int x, int y, EnemyClass e) {
		Vector3 pos = map.TileCoordToWorldCoord (x, y);
		Quaternion rot = map.transform.rotation;

		GameObject go = (GameObject)Instantiate (enemyTypes [(int)e], pos, rot);

		enemies [enemyCount] = go;

		Unit u 			= go.GetComponent<Unit> ();
		AIBehaviours ai = go.GetComponent<AIBehaviours> ();

		u.tileX = x;
		u.tileY = y;
		u.map = map;
		ai.myMap = map;
		ai.myManager = this;
		++enemyCount;

		map.GetNode (x, y).myUnit = u;

		//TODO wait until discovered 
		activeUnits.Add (go);
	}

	void Update() {
		ManageTurn ();
	}

	void ManageTurn() {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();

		// is it a players turn or ai
		if (sUnit.playable) {
			if (sUnit.remainingMove < 1 && sUnit.actionPoints < 1 && !sUnit.moving) {
				EndTurn ();
			}
		} else {
			sUnit.AIUpdate();
			if (sUnit.remainingMove < 1 && sUnit.actionPoints < 1 && !sUnit.moving) {
				EndTurn ();
			}
		}
	}

	public void NextUnitsTurn() {
		map.selectedUnit = currentQueue[turn];
		currentQueue [turn].GetComponent<Unit> ().StartTurn ();
		//is it a character or ai
		if (currentQueue [turn].GetComponent<Unit> ().playable) {
			map.FindReachableTiles ();
			map.HighlightTiles (currentQueue [turn].GetComponent<Unit> ().reachableTiles, Color.blue);
			map.HighlightTiles (currentQueue [turn].GetComponent<Unit> ().reachableTilesWithDash, new Color(0.5f,1,0));
		} else {
			//run the AIs turn
			currentQueue [turn].GetComponent<AIBehaviours> ().FSM();
		}
	}

	public void EndTurn() {
		map.UnhighlightTiles ();
		//map.selectedUnit.GetComponent<Unit> ().MoveNextTile ();
		++turn;

		if (turn >= currentQueue.Count) {
			turn = 0;
			currentQueue = activeUnits;
		}

		NextUnitsTurn ();
	}

	void loadEnemies() {

	}

	void loadCharacters() {

	}
}
