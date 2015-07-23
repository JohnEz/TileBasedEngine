using UnityEngine;
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

		GameObject go = (GameObject)Instantiate (enemyTypes [(int)e], pos, Quaternion.identity);

		enemies [enemyCount] = go;
		everyUnit.Add(go);

		Unit u 			= go.GetComponent<Unit> ();
		AIBehaviours ai = go.GetComponent<AIBehaviours> ();

		u.tileX = x;
		u.tileY = y;
		u.map = map;
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
			if (go.GetComponent<Unit>().HP < 1) {
				// REMOVE UNIT FROM FUCKING EVERYTHING
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
				ChangeActionDisplay(a+1);
				List<Node> targetableTiles = new List<Node>();

				switch(sUnit.myAbilities[a].area) {
				case AreaType.Single: targetableTiles = map.FindSingleRangedTargets(sUnit.myAbilities[a]);
					map.HighlightTiles(targetableTiles, new Color(1, 0.7f, 0.7f), new Color(1,0.55f,0.55f), 0);
					ShowSingleTargets(targetableTiles, sUnit.myAbilities[a]);
					break;
				case AreaType.AOE: targetableTiles = map.FindSingleRangedTargets(sUnit.myAbilities[a]);
					map.HighlightTiles(targetableTiles, new Color(1, 0.7f, 0.7f), new Color(1,0.55f,0.55f), 1);
					break;
				case AreaType.Line: targetableTiles = map.FindLineTargets(sUnit.myAbilities[a]);
					map.HighlightTiles(targetableTiles, new Color(1, 0.7f, 0.7f), new Color(1,0.55f,0.55f), 1);
					break;
				}
			}
		}
	}

	//searches through tiles to find target types
	void ShowSingleTargets(List<Node> tiles, Ability abil) {
		//if the ability can hit allies
		Unit cUnit = null;

		if (abil.targets == TargetType.Ally || abil.targets == TargetType.All) {
			for(int i=0; i < MAXCHARACTERS; ++i) {
				cUnit = playerUnitObjects[i].GetComponent<Unit>();
				if (map.GetClickableTile(cUnit.tileX, cUnit.tileY).highlighted) {
					map.GetClickableTile(cUnit.tileX, cUnit.tileY).HighlightTile(new Color(1, 0.4f, 0.4f), new Color(1,0,0), 1);
				}
			}
		}

		if (abil.targets == TargetType.Enemy || abil.targets == TargetType.All) {
			for(int i=0; i < MAXENEMIES; ++i) {
				if (enemies[i] != null) {
					cUnit = enemies[i].GetComponent<Unit>();
					if (map.GetClickableTile(cUnit.tileX, cUnit.tileY).highlighted) {
						map.GetClickableTile(cUnit.tileX, cUnit.tileY).HighlightTile(new Color(1, 0.4f, 0.4f), new Color(1,0,0), 1);
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
		switch(currentDisplaying) {
		case Display.Movement: map.FollowPath ();
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

		if (sUnit.myAbilities[(int)currentDisplaying-1].area == AreaType.AOE) {
			if (map.GetClickableTile(x, y).targetable) {
				List<Node> aoeTiles = map.FindReachableTiles(x, y, sUnit.myAbilities[(int)currentDisplaying-1].AOERange, true);
				map.HighlightTiles(aoeTiles, new Color(1, 0.4f, 0.4f), new Color(1,0,0), -2);
				map.GetNode(x, y).reachableNodes = aoeTiles;
			}
		}
		else if (sUnit.myAbilities[(int)currentDisplaying-1].area == AreaType.Line) {
			if (map.GetClickableTile(x, y).targetable) {
				List<Node> lineTiles = new List<Node>();

				Node curr = map.GetNode(x,y);
				int currX = x;
				int currY = y;

				//add previous
				while(curr.previous != null) {
					lineTiles.Add(curr.previous);
					curr = curr.previous;
				}

				//add start and next
				while(map.GetClickableTile(currX, currY).targetable && map.GetNode(currX, currY).directionToParent.magnitude > 0.1f) {
					lineTiles.Add(map.GetNode(currX, currY));
					currX -= (int)map.GetNode(currX, currY).directionToParent.x;
					currY -= (int)map.GetNode(currX, currY).directionToParent.y;
				}

				map.HighlightTiles(lineTiles, new Color(1, 0.4f, 0.4f), new Color(1,0,0), -2);
				map.GetNode(x, y).reachableNodes = lineTiles;
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
						ct.HighlightTile(new Color(1, 0.7f, 0.7f), new Color(1,0.55f,0.55f), -2);
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
					map.GetClickableTile(n.x, n.y).HighlightTile(new Color(1, 0.7f, 0.7f), new Color(1,0.55f,0.55f), -2);
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

		switch (sUnit.myAbilities[a].area) {
		case AreaType.Single: sUnit.myAbilities[a].UseAbility(map.GetNode(x, y).myUnit, map);
			break;
		case AreaType.AOE: sUnit.myAbilities[a].UseAbility(map.GetNode(x,y).reachableNodes, map);
			break;
		case AreaType.Line: sUnit.myAbilities[a].UseAbility(map.GetNode(x,y).reachableNodes, map);
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
			case CharacterClass.Warrior: u.myAbilities[0] = new CripplingStrike(u);
			u.myAbilities[1] = new ShieldSlam(u);
			u.myAbilities[2] = new Charge(u);
            break;
        }
    }

}
