using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

/* This class manages every unit in the game from the playable characters to the enemies
 * It also manages the turn order and gameplay flow
 * TODO seperate turn order to gamemanager
 * */

public enum CharacterClass {
	Acolyte,
	Elementalist,
	Highwayman,
	Ranger,
	Warrior,
	MAXCLASSES
}

public enum EnemyClass {
	Goblin,
	MAXCLASSES
}

enum Display {
	Movement,
	Ability1,
	Ability2,
	Ability3,
	Ability4,
	Ability5,
	Ability6,
	Ability7,
	Ability8,
	Nothing,
	MAXDISPLAY
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

	Display currentDisplaying; //used so things dont get run when they dont need to

	public void Initialise() {
		map = GetComponent<TileMap>();

		activeUnits = new List<GameObject> ();
		currentDisplaying = Display.Movement;

		enemies = new GameObject[MAXENEMIES];
		playerUnitObjects = new GameObject[MAXCHARACTERS];
		loadEnemies ();
		loadCharacters ();

		//temp
		spawnUnit (1, 1, CharacterClass.Acolyte);
		spawnUnit (2, 1, CharacterClass.Elementalist);
		spawnUnit (2, 2, CharacterClass.Ranger);
		spawnUnit (3, 1, CharacterClass.Highwayman);
		spawnUnit (3, 2, CharacterClass.Warrior);


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
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		map.selectedUnit = currentQueue[turn];
		sUnit.StartTurn ();
		//is it a character or ai
		if (sUnit.playable) {
			ShowMovement();
		} else {
			//run the AIs turn
			currentQueue [turn].GetComponent<AIBehaviours> ().FSM();
		}
	}

	public void EndTurn() {
		map.UnhighlightTiles ();
		map.selectedUnit.GetComponent<Unit> ().currentPath = null;
		++turn;

		if (turn >= currentQueue.Count) {
			turn = 0;
			currentQueue = activeUnits;
		}

		NextUnitsTurn ();
	}

	public void ShowAbility(int a) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		//check to see if the current character is playable
		if (sUnit.playable) {
			// if ability is not already being displayed
			if (a != (int)currentDisplaying - 1) {
				ChangeActionDisplay(a+1);
				List<Node> targetableTiles = new List<Node>();

				//TEMP
				Ability abil = new Ability();
				abil.area = AreaType.Single;
				abil.target = TargetType.Enemy;
				abil.range = 10;

				switch(abil.area) {
				case AreaType.Single: targetableTiles = map.FindSingleRangedTargets(abil);
					map.HighlightTiles(targetableTiles, new Color(1, 0.7f, 0.7f), new Color(1,0.55f,0.55f));
					ShowSingleTargets(targetableTiles, abil);
					break;
				}

			}


		}
	}

	//searches through tiles to find target types
	void ShowSingleTargets(List<Node> tiles, Ability abil) {
		//if the ability can hit allies
		Unit cUnit = null;

		if (abil.target == TargetType.Ally || abil.target == TargetType.All) {
			for(int i=0; i < MAXCHARACTERS; ++i) {
				cUnit = playerUnitObjects[i].GetComponent<Unit>();
				if (map.GetClickableTile(cUnit.tileX, cUnit.tileY).highlighted) {
					map.GetClickableTile(cUnit.tileX, cUnit.tileY).targetable = true;
					map.GetClickableTile(cUnit.tileX, cUnit.tileY).HighlightTile(new Color(1, 0.4f, 0.4f), new Color(1,0,0));
				}
			}
		}

		if (abil.target == TargetType.Enemy || abil.target == TargetType.All) {
			for(int i=0; i < MAXENEMIES; ++i) {
				if (enemies[i] != null) {
					cUnit = enemies[i].GetComponent<Unit>();
					if (map.GetClickableTile(cUnit.tileX, cUnit.tileY).highlighted) {
						map.GetClickableTile(cUnit.tileX, cUnit.tileY).targetable = true;
						map.GetClickableTile(cUnit.tileX, cUnit.tileY).HighlightTile(new Color(1, 0.4f, 0.4f), new Color(1,0,0));
					}
				}
			}
		}




	}

	//function that gets rid of everything to do with previous actions
	void ChangeActionDisplay(int a) {
		map.UnhighlightTiles();
		currentQueue [turn].GetComponent<Unit> ().currentPath = null;
		currentDisplaying = (Display)(a);
	}

	public void TileClicked(int x, int y) {
		Unit u = map.GetNode (x, y).myUnit;

		switch(currentDisplaying) {
		case Display.Movement: map.FollowPath ();
			break;
		}
	}

	public void TileHover(int x, int y) {
		switch(currentDisplaying) {
		case Display.Movement: map.GetPath(x, y);
			break;
		}

	}

	public void ShowMovement() {
		if (!currentQueue [turn].GetComponent<Unit> ().moving) {
			map.UnhighlightTiles();
			currentDisplaying = Display.Movement;
			currentQueue [turn].GetComponent<Unit> ().DrawReachableTiles ();
		}
	}

	void loadEnemies() {

	}

	void loadCharacters() {

	}
}
