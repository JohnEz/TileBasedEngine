﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

/* This class manages every unit in the game from the playable characters to the enemies
 * It also manages the turn order and gameplay flow
 *  seperate turn order to gamemanager
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
	TileMap map;
	VisualEffectLibrary effectLibrary;

	//turn order
	List<GameObject> currentQueue;
	List<GameObject> activeUnits;
	List<GameObject> everyUnit;
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
		effectLibrary = GetComponent<VisualEffectLibrary> ();

		activeUnits = new List<GameObject> ();
		everyUnit = new List<GameObject> ();
		currentDisplaying = Display.Movement;

		enemies = new GameObject[MAXENEMIES];
		playerUnitObjects = new GameObject[MAXCHARACTERS];
		loadEnemies ();
		loadCharacters ();

		//temp
		spawnUnit (1, 1, CharacterClass.Acolyte);
		spawnUnit (2, 1, CharacterClass.Elementalist);
		spawnUnit (3, 1, CharacterClass.Ranger);
		spawnUnit (4, 1, CharacterClass.Highwayman);
		spawnUnit (5, 1, CharacterClass.Warrior);

		spawnEnemy (3, 2, EnemyClass.Goblin);

		currentQueue = activeUnits;

		NextUnitsTurn ();
	}

	//temp
	void spawnUnit(int x, int y, CharacterClass c) {
		if (characterCount < MAXCHARACTERS) {
			Vector3 pos = map.TileCoordToWorldCoord (x, y);

			pos = new Vector3(pos.x, pos.y, -2);

			GameObject go = (GameObject)Instantiate (classes [(int)c], pos, Quaternion.identity);

			playerUnitObjects [characterCount] = go;
			everyUnit.Add(go);

			Unit u = go.GetComponent<Unit>();

			u.tileX = x;
			u.tileY = y;
			u.map = map;
            u.uManager = this;
			++characterCount;

            //give the unit its spells TODO find a better way of doing this instead of hardcoding
            GiveCharacterAbilities(u, c);

			map.GetNode(x, y).myUnit = u;

			activeUnits.Add(go);
		}

	}

	void spawnEnemy(int x, int y, EnemyClass e) {
		Vector3 pos = map.TileCoordToWorldCoord (x, y);

		pos = new Vector3(pos.x, pos.y, -2);

		GameObject go = (GameObject)Instantiate (enemyTypes [(int)e], pos, Quaternion.identity);

		enemies [enemyCount] = go;
		everyUnit.Add(go);

		Unit u 			= go.GetComponent<Unit> ();
		AIBehaviours ai = go.GetComponent<AIBehaviours> ();

		u.tileX = x;
		u.tileY = y;
		u.map = map;
		u.team = 2;
		ai.myMap = map;
		ai.myManager = this;
		++enemyCount;

		map.GetNode (x, y).myUnit = u;

		// wait until discovered 
		activeUnits.Add (go);
	}

	void Update() {
		ManageTurn ();
	}

	void ManageTurn() {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();

		// is it a players turn or ai
		if (sUnit.playable) {
			if (sUnit.remainingMove < 1 && sUnit.actionPoints < 1 && !sUnit.moving && !sUnit.attacking) {
				EndTurn ();
			}
		} else {
			if (sUnit.remainingMove < 1 && sUnit.actionPoints < 1 && !sUnit.moving) {
				EndTurn ();
			}
		}

		foreach (GameObject go in everyUnit) {
			sUnit = go.GetComponent<Unit>();
			if (sUnit.hp < 1) {
				// REMOVE UNIT FROM FUCKING EVERYTHING
				sUnit.CheckTriggers(TriggerType.Death);
				currentQueue.Remove(go);
			}
		}
	}

	public void EndTurnButton() {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		// if its not an ai turn and the unit isnt currently moving or attacking
		if (sUnit.playable && !sUnit.moving && !sUnit.attacking) {
			EndTurn();
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
			//TODO SORT ACTIVE UNITS BY INIT
			currentQueue = activeUnits;
		}

		NextUnitsTurn ();
	}

	public void ShowAbility(int a) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		//check to see if the current character is playable
		if (sUnit.playable && sUnit.actionPoints > 0) {
			if ((int)currentDisplaying != a+1 && sUnit.myAbilities[a] != null) {
				if (sUnit.mana >= sUnit.myAbilities[a].manaCost) {
					ChangeActionDisplay(a+1);
					List<Node> targetableTiles = new List<Node>();

					switch(sUnit.myAbilities[a].area) {
					case AreaType.Single: targetableTiles = map.FindSingleRangedTargets(sUnit.myAbilities[a]);
						map.HighlightTiles(targetableTiles, new Color(0.6f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), 0);
						ShowSingleTargets(targetableTiles, sUnit.myAbilities[a]);
						break;
					case AreaType.AOE: targetableTiles = map.FindSingleRangedTargets(sUnit.myAbilities[a]);
						map.HighlightTiles(targetableTiles, new Color(0.6f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), 1);
						break;
					case AreaType.Line: targetableTiles = map.FindLineTargets(sUnit.myAbilities[a]);
						map.HighlightTiles(targetableTiles, new Color(0.6f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), 1);
						break;
					case AreaType.Floor : targetableTiles = map.FindSingleRangedTargets(sUnit.myAbilities[a]);
						map.HighlightTiles(targetableTiles, new Color(0.6f, 0.3f, 0.3f), new Color(0.85f, 0.3f, 0.3f), 1);
						ShowFloorTargets(targetableTiles, sUnit.myAbilities[a]);
						break;
					}
				}
			}
		}
	}

	//searches through tiles to find target types
	void ShowSingleTargets(List<Node> tiles, Ability abil) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();

		//loop through every active unit TODO many need to add the next queue too
		foreach (GameObject go in currentQueue) {
			Unit u = go.GetComponent<Unit>();
			//check to see if the ability hits this type of unit
			if (map.GetClickableTile(u.tileX, u.tileY).highlighted) {
				if (abil.targets == TargetType.All || (abil.targets == TargetType.Ally && u.team == sUnit.team) || (abil.targets == TargetType.Enemy && u.team != sUnit.team)) {
					map.GetClickableTile(u.tileX, u.tileY).HighlightTile(new Color(0.85f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), 1);
				}
			}
		}


	}

	void ShowFloorTargets(List<Node> tiles, Ability abil) {

		//unhighlight tiles 
		foreach (Node n in tiles) {
			if (n.myUnit != null) {
				map.GetClickableTile(n.x, n.y).UnhighlightTile();
			}
		}
	}

	//function that gets rid of everything to do with previous actions
	void ChangeActionDisplay(int a) {
		map.UnhighlightTiles();
		currentQueue [turn].GetComponent<Unit> ().currentPath = null;
		currentDisplaying = (Display)(a);
	}

	void TileClickedMovement(int x, int y) {
		if (currentQueue [turn].GetComponent<Unit> ().currentPath == null) {
			map.GetPath(x, y);
		}

		currentQueue [turn].GetComponent<Unit> ().CheckTriggers (TriggerType.Move);
		map.FollowPath ();
	}

	public void TileClicked(int x, int y) {
		switch(currentDisplaying) {
		case Display.Movement: TileClickedMovement(x, y);
			break;
		case Display.Nothing: 
			break;
		default: UseAbility(x, y);
			break;
		}
	}

	public void TileEnter(int x, int y) {
		switch(currentDisplaying) {
		case Display.Movement: map.GetPath(x, y);
			break;
		case Display.Nothing: break;
		default: AbilityTileEnter(x, y);
			break;
		}

	}

	public void TileExit(int x, int y) {
		switch(currentDisplaying) {
		case Display.Movement: break;
		case Display.Nothing: break;
		default: AbilityTileExit(x, y);
			break;
		}
	}

	void AbilityTileEnter(int x, int y) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		if (map.GetClickableTile (x, y).targetable) {
			if (sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.AOE) {

				List<Node> aoeTiles = map.FindReachableTiles (x, y, sUnit.myAbilities [(int)currentDisplaying - 1].AOERange, true);
				map.HighlightTiles (aoeTiles, new Color(0.85f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), -2);
				map.GetNode (x, y).reachableNodes = aoeTiles;
			} else if (sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.Line) {

				List<Node> lineTiles = new List<Node> ();

				int dirX = 0;
				int dirY = 0;
				int currX = x;
				int currY = y;

				//find end of line
				while (map.GetClickableTile(currX, currY).targetable) {
					dirX = (int)map.GetNode (currX, currY).directionToParent.x;
					dirY = (int)map.GetNode (currX, currY).directionToParent.y;
					currX -= dirX;
					currY -= dirY;
				}

				currX += dirX;
				currY += dirY;
				Node curr = map.GetNode (currX, currY);

				//find the path
				while (curr.previous != null) {
					lineTiles.Add (curr);
					curr = curr.previous;
				}

				lineTiles.Add (curr);

				map.HighlightTiles (lineTiles, new Color(0.85f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), -2);
				map.GetNode (x, y).reachableNodes = lineTiles;
			}
		}
	}


	void AbilityTileExit(int x, int y) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		
		if (sUnit.myAbilities[(int)currentDisplaying-1].area == AreaType.AOE) {
			if (map.GetClickableTile(x, y).targetable && map.GetNode(x,y).reachableNodes != null) {
				foreach (Node n in map.GetNode(x,y).reachableNodes) {
					ClickableTile ct = map.GetClickableTile(n.x, n.y);
					if (ct.targetable) {
						ct.HighlightTile(new Color(0.6f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), -2);
					}
					else {
						ct.UnhighlightTile();
					}
				}
				map.GetNode(x,y).reachableNodes = null;

			}
		}
		else if (sUnit.myAbilities[(int)currentDisplaying-1].area == AreaType.Line) {
			if (map.GetClickableTile(x, y).targetable && map.GetNode(x,y).reachableNodes != null) {
				foreach (Node n in map.GetNode(x,y).reachableNodes) {
					map.GetClickableTile(n.x, n.y).HighlightTile(new Color(0.6f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), -2);
				}
				map.GetNode(x,y).reachableNodes = null;
				
			}
		}
	}

	public void ShowMovement() {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		if (!sUnit.moving && sUnit.playable) {
			map.UnhighlightTiles();
			currentDisplaying = Display.Movement;
			sUnit.DrawReachableTiles ();
		}
	}

	void UseAbility(int x, int y) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();

		int a = (int)currentDisplaying - 1;

		sUnit.attacking = true;

		sUnit.CheckTriggers (TriggerType.Use_Ability);

		switch (sUnit.myAbilities[a].area) {
		case AreaType.Single: sUnit.myAbilities[a].UseAbility(map.GetNode(x, y).myUnit);
			map.UnhighlightTiles();
			break;
		case AreaType.AOE:
		case AreaType.Line:
		case AreaType.Floor: sUnit.myAbilities[a].UseAbility(map.GetNode(x,y));
			map.UnhighlightTiles();
			break;
		}

		sUnit.actionPoints = 0;
		sUnit.remainingMove = 0;
	}

	void loadEnemies() {

	}

	void loadCharacters() {

	}

    void GiveCharacterAbilities(Unit u, CharacterClass c)
    {
        switch (c)
        {
		case CharacterClass.Warrior: u.myAbilities[0] = new CripplingStrike(u, map, effectLibrary);
			u.myAbilities[1] = new ShieldSlam(u, map, effectLibrary);
			u.myAbilities[2] = new Charge(u, map, effectLibrary);
            break;
		case CharacterClass.Acolyte: u.myAbilities[0] = new WordOfHealing(u, map, effectLibrary);
			u.myAbilities[1] = new RighteousShield(u, map, effectLibrary);
			u.myAbilities[2] = new DivineSacrifice(u, map, effectLibrary);
			break;
		case CharacterClass.Highwayman: u.myAbilities[0] = new Lacerate(u, map, effectLibrary);
			u.myAbilities[1] = new Lunge(u, map, effectLibrary);
			u.myAbilities[2] = new PointBlank(u, map, effectLibrary);
			break;
		case CharacterClass.Elementalist: u.myAbilities[0] = new Fireball(u, map, effectLibrary);
			u.myAbilities[1] = new FlashFreeze(u, map, effectLibrary);
			u.myAbilities[2] = new ManaTrap(u, map, effectLibrary);
			break;
		case CharacterClass.Ranger: u.myAbilities[0] = new TripleShot(u, map, effectLibrary);
			u.myAbilities[1] = new CripplingShot(u, map, effectLibrary);
			u.myAbilities[2] = new ExploitWeakness(u, map, effectLibrary);
			break;
        }
    }

}
