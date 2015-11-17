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
	GoblinAxeThrower,
	GoblinDrummer,
	GoblinShaman,
	EarthTotem,
	FireTotem,
	WaterTotem,
	WindTotem,
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
	PrefabLibrary effectLibrary;

	//turn order
	List<GameObject> currentQueue;
	List<GameObject> activeUnits;
	List<GameObject> everyUnit;
	public int turn = 0;
	public int deadCount = 0;

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
	public GameObject healthBar;
	public GameObject hpText;
	public GameObject manaBar;
	public GameObject manaText;
	public GameObject shieldBar;

	bool waitingForCamera = false;

	Unit selectedUnit = null;

	Display currentDisplaying; //used so things dont get run when they dont need to

	public void Initialise() {
		map = GetComponent<TileMap>();
		effectLibrary = GetComponent<PrefabLibrary> ();

		activeUnits = new List<GameObject> ();
		everyUnit = new List<GameObject> ();
		currentQueue = new List<GameObject> ();
		currentDisplaying = Display.Movement;

		enemies = new GameObject[MAXENEMIES];
		playerUnitObjects = new GameObject[MAXCHARACTERS];
		loadEnemies ();
		loadCharacters ();

		//temp
		spawnUnit (5, 1, CharacterClass.Warrior);
		spawnUnit (4, 1, CharacterClass.Highwayman);
		spawnUnit (3, 1, CharacterClass.Ranger);
		spawnUnit (2, 1, CharacterClass.Elementalist);
		spawnUnit (1, 1, CharacterClass.Acolyte);

		GetComponent<GameManager> ().UI.GetComponent<UIManager> ().SetupUnitFrames (playerUnitObjects);

		//spawnEnemy (4, 2, EnemyClass.Goblin);
		List<Unit> squad = new List<Unit> ();

		squad.Add(spawnEnemy (42, 4, EnemyClass.GoblinShaman));

		foreach (Unit u in squad) {
			u.GetComponent<AIBehaviours>().myGroup = squad;
		}

		squad = new List<Unit> ();
		squad.Add(spawnEnemy (15, 4, EnemyClass.GoblinAxeThrower));
		squad.Add(spawnEnemy (15, 5, EnemyClass.Goblin));
		squad.Add(spawnEnemy (17, 1, EnemyClass.Goblin));
		squad.Add(spawnEnemy (15, 8, EnemyClass.GoblinAxeThrower));

		foreach (Unit u in squad) {
			u.GetComponent<AIBehaviours>().myGroup = squad;
		}


		squad = new List<Unit> ();
		squad.Add(spawnEnemy (21, 4, EnemyClass.GoblinDrummer));
		squad.Add(spawnEnemy (25, 8, EnemyClass.GoblinAxeThrower));
		squad.Add(spawnEnemy (26, 1, EnemyClass.GoblinAxeThrower));
		squad.Add(spawnEnemy (26, 4, EnemyClass.Goblin));
		squad.Add(spawnEnemy (27, 4, EnemyClass.Goblin));

		foreach (Unit u in squad) {
			u.GetComponent<AIBehaviours>().myGroup = squad;
		}

		activeUnits.Sort(CompareListByInitiative);

		foreach (GameObject go in activeUnits) {
			currentQueue.Add(go);
		}

		//set the starting vision
		foreach (GameObject go in playerUnitObjects) {
			map.DetectVisability(go.GetComponent<Unit>());
		}

		GetComponentInChildren<UIManager> ().ChangeRound (currentQueue);

		NextUnitsTurn ();
	}
	
	void spawnUnit(int x, int y, CharacterClass c) {
		if (characterCount < MAXCHARACTERS) {

			//set its position in the world and spawn in
			Vector3 pos = map.TileCoordToWorldCoord (x, y);
			pos = new Vector3(pos.x, pos.y, -2);
			GameObject character = (GameObject)Instantiate (classes [(int)c], pos, Quaternion.identity);



			//set its starting values
			Unit u = character.GetComponent<Unit>();
			u.tileX = x;
			u.tileY = y;
			u.map = map;
			u.ID = everyUnit.Count;
            u.uManager = this;
			u.hp = u.maxHP;
			u.mana = u.maxMana;

			//add a shieldbar
			AddUnitUIElement (shieldBar, u);

			//add a manabar
			AddUnitUIElement (manaBar, u);
			AddUnitUIElement (manaText, u);

			//add a healthbar
			AddUnitUIElement (healthBar, u);
			AddUnitUIElement (hpText, u);

            //give the unit its spells TODO find a better way of doing this instead of hardcoding
            GiveCharacterAbilities(u, c);

			//make the map know where the unit is
			map.GetNode(x, y).myUnit = u;

			//add it to the manager lists
			playerUnitObjects [characterCount] = character; // list of players characters
			everyUnit.Add(character);						// list of every unit
			activeUnits.Add(character);						// list of currently active units

			//increase the character counter
			++characterCount;
		}

	}

	public Unit spawnEnemy(int x, int y, EnemyClass e, bool isAtive = false) {

		//set starting position and spawn unit
		Vector3 pos = map.TileCoordToWorldCoord (x, y);
		pos = new Vector3(pos.x, pos.y, -2);
		GameObject go = (GameObject)Instantiate (enemyTypes [(int)e], pos, Quaternion.identity);


		Unit u 			= go.GetComponent<Unit> ();
		AIBehaviours ai = go.GetComponent<AIBehaviours> ();

		//set its starting values
		u.tileX = x;
		u.tileY = y;
		u.map = map;
		u.team = 2;
		u.ID = everyUnit.Count;
		u.uManager = this;
		u.hp = u.maxHP;
		u.mana = u.maxMana;
		ai.myMap = map;
		ai.myManager = this;
		ai.Initialise ();
		++enemyCount;

		//add a shieldbar
		AddUnitUIElement (shieldBar, u);
		
		//add a manabar
		AddUnitUIElement (manaBar, u);
		AddUnitUIElement (manaText, u);

		//add a healthbar
		AddUnitUIElement (healthBar, u);
		AddUnitUIElement (hpText, u);

		//give the enemy their abilities
		GiveEnemyAbilities (u, e);

		//let the map know where the unit is
		map.GetNode (x, y).myUnit = u;
		 
		// add to all the lists
		enemies [enemyCount] = go;	//enemy array
		everyUnit.Add(go);			//every unit

		if (isAtive) {
			activeUnits.Add(go);	 // list of currently active units
			u.isActive = true;
		}

		return u;
	}

	void AddUnitUIElement(GameObject prefab, Unit u) {
		GameObject go = Instantiate (prefab) as GameObject;
		RectTransform tempRect = go.GetComponent<RectTransform> ();
		go.transform.SetParent (u.transform.FindChild("UnitCanvas"));

		go.name = prefab.name;
		
		tempRect.transform.localPosition = prefab.transform.localPosition;
		tempRect.transform.localScale = prefab.transform.localScale;
		tempRect.transform.rotation = prefab.transform.localRotation;

	}

	public void SelectUnit(Unit u) {
		if (selectedUnit) {
			selectedUnit.selected = false;
		}
		selectedUnit = u;
		selectedUnit.selected = true;
		GetComponent<GameManager> ().UI.GetComponent<UIManager> ().SetSelectedFrame (selectedUnit);
	}

	void Update() {
		if (!waitingForCamera) {
			ManageTurn ();
		} else if (!GetComponent<GameManager> ().cam.GetComponent<CameraController> ().movingToDestination) {
			waitingForCamera = false;

			Unit sUnit = currentQueue [turn].GetComponent<Unit>();


			//is it a character or ai
			if (sUnit.playable) {
				if (sUnit.movespeed > 0) {
					ShowMovement ();
				} else {
					currentDisplaying = Display.Movement;
				}
				GetComponentInChildren<UIManager>().DrawAbilities(4, currentQueue [turn].GetComponent<Unit>());
			} else {
				//run the AIs turn
				currentQueue [turn].GetComponent<AIBehaviours> ().FSM ();
				GetComponentInChildren<UIManager>().DrawAbilities(0, null);
			}
		}
	}

	void ManageTurn() {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();

		// is it a players turn or ai
		if (sUnit.playable) {
			if ((sUnit.remainingMove < 1 && sUnit.actionPoints < 1 && !sUnit.moving && !sUnit.attacking) || sUnit.isDead) {
				EndTurn ();
			}
		} else {
			if ((sUnit.remainingMove < 1 && sUnit.actionPoints < 1 && !sUnit.moving && !sUnit.attacking) || sUnit.isDead) {
				EndTurn ();
			}
		}

		foreach (GameObject go in everyUnit) {
			sUnit = go.GetComponent<Unit>();
			if (sUnit.hp < 1 && !sUnit.isDead) {
				//TODO REMOVE UNIT FROM FUCKING EVERYTHING
				sUnit.CheckTriggers(TriggerType.Death);
				
				int ind = currentQueue.IndexOf(go);

				sUnit.isDead = true;
				map.GetNode(sUnit.tileX, sUnit.tileY).myUnit = null;
				go.transform.position = new Vector3(5, 5, 10);
				
				if (ind == turn) {
					EndTurn ();
				}

			}
		}
	}

	//checks to see if the unit is spotted by any inactive AI
	public void CheckAIVisability(Unit u) {
		foreach (GameObject go in enemies) {
			if (go != null) {
				Unit enemy = go.GetComponent<Unit>();
				//if the enemy has los on the unit
				if (!enemy.isActive && map.HasLineOfSight(map.GetNode(enemy.tileX, enemy.tileY), map.GetNode(u.tileX, u.tileY), enemy.sight)) {
					foreach(Unit squadie in enemy.GetComponent<AIBehaviours>().myGroup) {
						squadie.isActive = true;
						activeUnits.Add(squadie.gameObject);
						squadie.GetComponent<AIBehaviours>().SpottedPlayer();
					}
				}
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
		map.selectedUnit = currentQueue [turn];
		
		sUnit.StartTurn ();
		if (!sUnit.isDead) {

			//move camera to unit's position
			GetComponent<GameManager> ().cam.GetComponent<CameraController> ().MoveToTarget (sUnit.transform.position);

			waitingForCamera = true;
		} else {
			deadCount++;
			EndTurn();
		}

	}

	public void EndTurn() {
		map.UnhighlightTiles ();
		map.selectedUnit.GetComponent<Unit> ().currentPath = null;

		//find which effects are on the tile and apply them to the unit
		Unit sUnit = map.selectedUnit.GetComponent<Unit> ();
		Node unitsNode = map.GetNode (sUnit.tileX, sUnit.tileY);

		foreach (Effect eff in unitsNode.myEffects) {
			if (eff.targets == -1 || eff.targets == sUnit.team) {
				sUnit.ApplyEffect(eff);
			}
		}

		map.DetectVisability ();

		++turn;

		GetComponentInChildren<UIManager> ().ChangeTurn (turn - deadCount);
		GetComponentInChildren<UIManager> ().UpdateAllUnitFrames ();

		if (turn >= currentQueue.Count) {
			turn = 0;
			deadCount = 0;

			currentQueue = new List<GameObject>();
			activeUnits.Sort(CompareListByInitiative);
			
			foreach (GameObject go in activeUnits) {
				currentQueue.Add(go);
			}

			GetComponentInChildren<UIManager> ().ChangeRound (currentQueue);
			GetComponentInChildren<UIManager> ().ChangeTurn (turn - deadCount);
		}

		NextUnitsTurn ();
	}

	static int CompareListByInitiative(GameObject g1, GameObject g2) {

		Unit u1 = g1.GetComponent<Unit> ();
		Unit u2 = g2.GetComponent<Unit> ();

		// high the better so use u2
		int val = u2.init.CompareTo (u1.init);

		//if there init is the same, go off id i guess?
		if (val == 0) {
			//lower better so us u1
			val = u1.ID.CompareTo (u2.ID);
		}

		//return the value
		return val;
		
	}

	public void ShowAbility(int a) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		//check to see if the current character is playable
		if (sUnit.playable && !sUnit.UnitBusy ()) {
			if (sUnit.actionPoints > 0) {
				// dont try to reload the same ability
				if ((int)currentDisplaying != a + 1 && sUnit.myAbilities [a] != null) {
					//if the unit has mana
					if (sUnit.mana >= sUnit.myAbilities [a].manaCost) {
						// if the ability isnt on cooldown
						if (sUnit.myAbilities [a].cooldown < 1) {
							// check if the user has any guard if it needs it
							if ((sUnit.myAbilities [a].usesGuard && sUnit.guardPoints > 0) || !sUnit.myAbilities [a].usesGuard) {

								//highlight icon
								GetComponentInChildren<UIManager> ().HighlightIcon (a);

								ChangeActionDisplay (a + 1);
								List<Node> targetableTiles = new List<Node> ();

								switch (sUnit.myAbilities [a].area) {
								case AreaType.Single:
									targetableTiles = map.FindSingleRangedTargets (sUnit.myAbilities [a], sUnit);
									map.HighlightTiles (targetableTiles, new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.4f, 0.4f), 0);
									ShowSingleTargets (targetableTiles, sUnit.myAbilities [a]);
									break;
								case AreaType.AOE:
									targetableTiles = map.FindSingleRangedTargets (sUnit.myAbilities [a], sUnit);
									map.HighlightTiles (targetableTiles, new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.4f, 0.4f), 1);
									break;
								case AreaType.LineAOE:
									targetableTiles = map.FindLineTargets (sUnit.myAbilities [a], true, true);
									map.HighlightTiles (targetableTiles, new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.4f, 0.4f), 1);
									break;
								case AreaType.Line:
									targetableTiles = map.FindLineTargets (sUnit.myAbilities [a]);
									map.HighlightTiles (targetableTiles, new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.4f, 0.4f), 1);
									break;
								case AreaType.Floor:
									targetableTiles = map.FindSingleRangedTargets (sUnit.myAbilities [a], sUnit);
									map.HighlightTiles (targetableTiles, new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.3f, 0.3f), 1);
									ShowFloorTargets (targetableTiles, sUnit.myAbilities [a]);
									break;
								case AreaType.Self:
									targetableTiles.Add (map.GetNode (sUnit.tileX, sUnit.tileY));
									map.HighlightTiles (targetableTiles, new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.3f, 0.3f), 1);
									break;
								case AreaType.All:
									targetableTiles = FindAllTargets (sUnit.myAbilities [a]);
									map.HighlightTiles (targetableTiles, new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.3f, 0.3f), 1);
									break;
								}
							} else {
								GetComponentInChildren<UIManager> ().ShowErrorText ("Unit doesnt have any guard");
								sUnit.GetComponent<AudioSource> ().PlayOneShot (effectLibrary.getSoundEffect ("Error"));
							}
						} else {
							GetComponentInChildren<UIManager> ().ShowErrorText ("Ability is on cooldown");
							sUnit.GetComponent<AudioSource> ().PlayOneShot (effectLibrary.getSoundEffect ("Error"));
						}
					} else {
						GetComponentInChildren<UIManager> ().ShowErrorText ("Not enough mana");
						sUnit.GetComponent<AudioSource> ().PlayOneShot (effectLibrary.getSoundEffect ("Error"));
					}
				}
			} else {
				GetComponentInChildren<UIManager> ().ShowErrorText ("No action points left");
				sUnit.GetComponent<AudioSource> ().PlayOneShot (effectLibrary.getSoundEffect ("Error"));
			}
		}
	}

	//searches through tiles to find target types
	void ShowSingleTargets(List<Node> tiles, Ability abil) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();

		//loop through every active unit TODO many need to add the next queue too
		foreach (GameObject go in everyUnit) {
			Unit u = go.GetComponent<Unit>();
			//check to see if the ability hits this type of unit
			if (map.GetClickableTile(u.tileX, u.tileY).highlighted && !u.isDead) {
				if (abil.targets == TargetType.All || (abil.targets == TargetType.Ally && u.team == sUnit.team) || (abil.targets == TargetType.Enemy && u.team != sUnit.team)) {
					map.GetClickableTile(u.tileX, u.tileY).HighlightTile(new Color(0.85f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), 1);
				}
			}
		}


	}

	List<Node> FindAllTargets(Ability abil) {
		List<Node> targetNodes = new List<Node> ();

		//loop through all active units
		foreach (GameObject go in everyUnit) {
			Unit sUnit = go.GetComponent<Unit>();

			//if the unit is not dead
			if (!sUnit.isDead) {
				//check if the unit is the correct type and add it
				if (abil.targets == TargetType.All || (abil.targets == TargetType.Enemy && sUnit.team != abil.myCaster.team) || (abil.targets == TargetType.Ally && sUnit.team == abil.myCaster.team)) {
					targetNodes.Add(map.GetNode(sUnit.tileX, sUnit.tileY));
				}
			}
		}

		return targetNodes;
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
			map.HighlightPath(currentQueue [turn].GetComponent<Unit> ().currentPath);
			break;
		case Display.Nothing: break;
		default: AbilityTileEnter(x, y);
			break;
		}

	}

	public void TileExit(int x, int y) {
		switch(currentDisplaying) {
		case Display.Movement: map.UnhighlightPath(currentQueue [turn].GetComponent<Unit> ().currentPath);
			break;
		case Display.Nothing: break;
		default: AbilityTileExit(x, y);
			break;
		}
	}

	void AbilityTileEnter(int x, int y) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		if (map.GetClickableTile (x, y).targetable) {
			if (sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.AOE || sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.LineAOE) {

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
			} else if (sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.All) {
				Node target = map.GetNode(x, y);
				target.reachableNodes = FindAllTargets(sUnit.myAbilities [(int)currentDisplaying - 1]);
				map.HighlightTiles (target.reachableNodes, new Color(0.85f, 0.3f, 0.3f), new Color(0.85f,0.4f,0.4f), -2);
			}
		} 
	}


	void AbilityTileExit(int x, int y) {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		
		if (sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.AOE || sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.LineAOE) {
			if (map.GetClickableTile (x, y).targetable && map.GetNode (x, y).reachableNodes != null) {
				foreach (Node n in map.GetNode(x,y).reachableNodes) {
					ClickableTile ct = map.GetClickableTile (n.x, n.y);
					if (ct.targetable) {
						ct.HighlightTile (new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.4f, 0.4f), -2);
					} else {
						ct.UnhighlightTile ();
					}
				}
				map.GetNode (x, y).reachableNodes = null;

			}
		} else if (sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.Line || sUnit.myAbilities [(int)currentDisplaying - 1].area == AreaType.All) {
			if (map.GetClickableTile (x, y).targetable && map.GetNode (x, y).reachableNodes != null) {
				foreach (Node n in map.GetNode(x,y).reachableNodes) {
					map.GetClickableTile (n.x, n.y).HighlightTile (new Color (0.6f, 0.3f, 0.3f), new Color (0.85f, 0.4f, 0.4f), -2);
				}
				map.GetNode (x, y).reachableNodes = null;
				
			}
		}
	}

	public void ShowMovement() {
		Unit sUnit = currentQueue [turn].GetComponent<Unit> ();
		if (!sUnit.moving && sUnit.playable) {
			map.UnhighlightTiles ();
			currentDisplaying = Display.Movement;
			sUnit.DrawReachableTiles ();
		}
	}

	void UseAbility(int x, int y) {
		if (map.GetNode (x, y).reachableNodes == null) {
			AbilityTileEnter(x, y);
		}

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
		case AreaType.Self:
		case AreaType.LineAOE:
		case AreaType.All:
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
		case CharacterClass.Warrior: u.myAbilities[0] = new GuardianStrike(u, map, effectLibrary);
			u.myAbilities[1] = new Barge(u, map, effectLibrary);
			u.myAbilities[2] = new ShieldSlam(u, map, effectLibrary);
			u.myAbilities[3] = new CounterAttack(u, map, effectLibrary);
            break;
		case CharacterClass.Acolyte: u.myAbilities[0] = new WordOfHealing(u, map, effectLibrary);
			u.myAbilities[1] = new Smite(u, map, effectLibrary);
			u.myAbilities[2] = new DivineSacrifice(u, map, effectLibrary);
			u.myAbilities[3] = new Amplify(u, map, effectLibrary);
			break;
		case CharacterClass.Highwayman: u.myAbilities[0] = new Lacerate(u, map, effectLibrary);
			u.myAbilities[1] = new Flurry(u, map, effectLibrary);
			u.myAbilities[2] = new ShadowStep(u, map, effectLibrary);
			u.myAbilities[3] = new SmokeBomb(u, map, effectLibrary);
			break;
		case CharacterClass.Elementalist: u.myAbilities[0] = new ArcanePulse(u, map, effectLibrary);
			u.myAbilities[1] = new Fireball(u, map, effectLibrary);
			u.myAbilities[2] = new FlashFreeze(u, map, effectLibrary);
			u.myAbilities[3] = new ArcaneSpark(u, map, effectLibrary);
			break;
		case CharacterClass.Ranger: u.myAbilities[0] = new TripleShot(u, map, effectLibrary);
			u.myAbilities[1] = new CripplingShot(u, map, effectLibrary);
			u.myAbilities[2] = new ExploitWeakness(u, map, effectLibrary);
			u.myAbilities[3] = new CracklingArrow(u, map, effectLibrary);
			break;
        }
    }

	void GiveEnemyAbilities(Unit u, EnemyClass e) {
		switch (e) {
		case EnemyClass.Goblin: u.myAbilities[0] = new Clobber(u, map, effectLibrary);
			break;
		case EnemyClass.GoblinAxeThrower: u.myAbilities[0] = new AxeThrow(u, map, effectLibrary);
			break;
		case EnemyClass.GoblinDrummer: u.myAbilities[0] = new BattleRhythem(u, map, effectLibrary);
			u.myAbilities[1] = new Inspire(u, map, effectLibrary);
			u.myAbilities[2] = new SonicWave(u, map, effectLibrary);
			break;
		case EnemyClass.GoblinShaman: u.myAbilities[0] = new HammerSlam(u, map, effectLibrary);
			u.myAbilities[1] = new FlamingAxe(u, map, effectLibrary);
			u.myAbilities[2] = new Combustion(u, map, effectLibrary);
			u.myAbilities[3] = new SpawnTotem(u, map, effectLibrary);
			break;
		case EnemyClass.EarthTotem: u.myAbilities[0] = new TotemSnare(u, map, effectLibrary);
			u.myAbilities[1] = new TotemShield(u, map, effectLibrary);
			break;
		case EnemyClass.FireTotem: u.myAbilities[0] = new TotemFireball(u, map, effectLibrary);
			u.myAbilities[1] = new TotemFlameShield(u, map, effectLibrary);
			break;
		case EnemyClass.WaterTotem: u.myAbilities[0] = new TotemHeal(u, map, effectLibrary);
			//u.myAbilities[1] = new TotemMist(u, map, effectLibrary);
			break;
		case EnemyClass.WindTotem: u.myAbilities[0] = new TotemCooldownReduction(u, map, effectLibrary);
			u.myAbilities[1] = new TotemPushBack(u, map, effectLibrary);
			break;
		}

	}

}
