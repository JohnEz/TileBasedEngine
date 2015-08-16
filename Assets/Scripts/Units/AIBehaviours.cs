using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public enum Behaviour {
	Dumb,
	Scared,
	DumbRanged,
	Taunted,
	MAXBEHAVIOURS
}

public enum AIStrategy {
	Attack,
	MoveAttack,
	Dash,
	Pass,
	MAXAISTRATS
}

public class AIBehaviours : MonoBehaviour {

	public Behaviour myBehaviour = Behaviour.Dumb;
	public AIStrategy myStrat;
	public UnitManager myManager;
	public TileMap myMap;
	Unit myUnit;

	public Node target;

	public bool hasAttacked = false; // if the unit is meant to attack only allow it to once
	public bool inCloseCombat = false; // if the unit is in melee with another
	public bool turnPlanned = false;

	// Use this for initialization
	public void Initialise () {
		myUnit = GetComponent<Unit> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FSM() {
		switch (myBehaviour) {
		case Behaviour.Dumb: FindTargetClosest(myManager.playerUnitObjects, false);
			Dumb ();
			break;
		case Behaviour.DumbRanged: FindClosestLoS(myManager.playerUnitObjects);
			DumbRanged();
			break;
		}
		turnPlanned = true;
	}
	
	//find target closest to the unit, from array
	void FindTargetClosest(GameObject[] targets, bool ignoreUnits) {

		//if stunned
		if (myUnit.remainingMove <= 0 && myUnit.actionPoints <= 0 && myUnit.movespeed == 0) {
			return;
		}

		List<Node> shortestPath = new List<Node> ();
		myUnit.currentPath = new List<Node> ();
		float currentPathLength = Mathf.Infinity;
		int tx = 0;
		int ty = 0;


		// for each character, find path
		//for (int i = 0; i < myManager.characterCount; ++i) {
		for (int i = 0; i < targets.Count(); ++i) {
			//Unit targetUnit = myManager.playerUnitObjects[i].GetComponent<Unit>();
			Unit targetUnit = targets[i].GetComponent<Unit>();
			if (targetUnit != null) {
				//if the target is alive
				if (!targetUnit.isDead && targetUnit != myUnit) {

					//find the path to the target
					myMap.GeneratePathTo(targetUnit.tileX, targetUnit.tileY, ignoreUnits);

					// if the path is shorter than current path
					if (myUnit.currentPath.Count < currentPathLength && myUnit.currentPath.Count > 0) {
						// set shortest path
						shortestPath = myUnit.currentPath;
						currentPathLength = myUnit.currentPath.Count;
						tx = targetUnit.tileX;
						ty = targetUnit.tileY;
					}
				}
			}
		}

		//set the target
		target = myMap.GetNode (tx, ty);
		myUnit.currentPath = shortestPath;


	}

	//find target closest to the unit, from array
	void FindTargetClosestAllyInMelee() {

		//if stunned
		if (myUnit.remainingMove <= 0 && myUnit.actionPoints <= 0 && myUnit.movespeed == 0) {
			return;
		}
		
		List<Node> shortestPath = new List<Node> ();
		myUnit.currentPath = new List<Node> ();
		float currentPathLength = Mathf.Infinity;
		int tx = 0;
		int ty = 0;
		
		
		// for each character, find path
		//for (int i = 0; i < myManager.characterCount; ++i) {
		for (int i = 0; i < myManager.enemies.Count(); ++i) {
			GameObject targetGo = myManager.enemies[i];
			if (targetGo) {
				Unit targetUnit = myManager.enemies[i].GetComponent<Unit>();
				AIBehaviours targetAI = myManager.enemies[i].GetComponent<AIBehaviours>();
				if (targetUnit != null) {
					//if the target is alive
					if (!targetUnit.isDead && targetUnit != myUnit && targetAI.inCloseCombat) {
					
						//find the path to the target
						myMap.GeneratePathTo(targetUnit.tileX, targetUnit.tileY);
					
						// if the path is shorter than current path
						if (myUnit.currentPath.Count < currentPathLength && myUnit.currentPath.Count > 0) {
							// set shortest path
							shortestPath = myUnit.currentPath;
							currentPathLength = myUnit.currentPath.Count;
							tx = targetUnit.tileX;
							ty = targetUnit.tileY;
						}
					}
				}
			}
		}
		
		//set the target
		target = myMap.GetNode (tx, ty);
		myUnit.currentPath = shortestPath;
		
		
	}

	void SupportAllyInMelee() {
		FindTargetClosestAllyInMelee();
	}

	// run to and attack closest target
	void Dumb(){

		// if the ai is stunned, or didnt have a path for some reason
		if (myUnit.currentPath == null) {
			return;
		}

		//if no path could be found, look for an ally i can support
		if (myUnit.currentPath.Count == 0) {
			SupportAllyInMelee ();

			//if no path could be found still, there isnt even an ally to support
			if (myUnit.currentPath.Count == 0) {
				//pass go
				myUnit.ShowCombatText("Passed", myUnit.statusCombatText);
			} else {
				// move to support the ally
				MoveToSupportAlly();
			}

		} else {
			//run normal turn
			BasicTurn();
		}

	}

	void DumbRanged() {
		//if no path could be found, look for an ally i can support
		if (myUnit.currentPath == null) {
			//pass go
			myUnit.ShowCombatText ("Passed", myUnit.statusCombatText);
			return;
		}

		BasicTurn ();

	}

	void FindClosestLoS(GameObject[] targets) {

		//if stunned return null
		if (myUnit.remainingMove <= 0 && myUnit.actionPoints <= 0 && myUnit.movespeed == 0) {
			return;
		}
		
		List<Node> shortestPath = null;
		myUnit.currentPath = null;
		float currentPathLength = Mathf.Infinity;
		

		
		//check each unit
		foreach (GameObject go in targets) {
			Unit sUnit = go.GetComponent<Unit>();
			if (!sUnit.isDead) {
				List<Node> losNodes = myMap.FindSingleRangedTargets (myUnit.myAbilities [0], sUnit, true);
				if (myMap.AIFindClosestTile(sUnit.tileX, sUnit.tileY, losNodes)) {
					if (myUnit.currentPath.Count < currentPathLength) {
						shortestPath = myUnit.currentPath;
						currentPathLength = myUnit.currentPath.Count;
						target = myMap.GetNode (sUnit.tileX, sUnit.tileY);
					}
				}
			}

		}

		//set the target
		myUnit.currentPath = shortestPath;

		
	}

	//Basic turn - attack if in range or move and attack, or dash
	void BasicTurn(){

		myUnit.currentPath.Remove (target);

		// is it already in melee?
		if (myUnit.currentPath.Count == 0) {
			myStrat = AIStrategy.Attack;
			myUnit.attacking = true;
			hasAttacked = false;
			inCloseCombat = true;
		} 
		// if target is in first move range, move and attack
		else if (myUnit.remainingMove > 0 || myUnit.movespeed > 0) {
			if (myUnit.currentPath.Count <= myUnit.movespeed) {
				myStrat = AIStrategy.MoveAttack;
				myUnit.moving = true;
				myUnit.attacking = true;
				myUnit.remainingMove -= (int)myUnit.currentPath.Last().cost;
				hasAttacked = false;
				inCloseCombat = true;
			} 
			// if the unit is really far away, dash
			else{
				//check to see if it is now in melee
				if (myUnit.currentPath.Count <= myUnit.movespeed*2) {
					inCloseCombat = true;
				} else {
					inCloseCombat = false;
				}
				myStrat = AIStrategy.Dash;
				myUnit.moving = true;
				FindFurthestTileInPath();
				myUnit.remainingMove += myUnit.movespeed - (int)myUnit.currentPath.Last().cost;
				--myUnit.actionPoints;
				hasAttacked = true;
			}
		}


		if (myUnit.moving) {
			myMap.CullPath ();
			myMap.GetNode (myUnit.tileX, myUnit.tileY).myUnit = null;
			myUnit.currentPath.Last ().myUnit = myUnit;
		}
	}

	//Basic turn - attack if in range or move and attack, or dash
	void BasicTurnRanged(){
		
		myUnit.currentPath.Remove (target);
		
		// is it already in melee?
		if (myUnit.currentPath.Count == 0) {
			myStrat = AIStrategy.Attack;
			myUnit.attacking = true;
			hasAttacked = false;
		} 
		// if target is in first move range, move and attack
		else if (myUnit.remainingMove > 0 || myUnit.movespeed > 0) {
			if (myUnit.currentPath.Count <= myUnit.movespeed) {
				myStrat = AIStrategy.MoveAttack;
				myUnit.moving = true;
				myUnit.attacking = true;
				myUnit.remainingMove -= (int)myUnit.currentPath.Last().cost;
				hasAttacked = false;
			} 
			// if the unit is really far away, dash
			else{
				myStrat = AIStrategy.Dash;
				myUnit.moving = true;
				FindFurthestTileInPath();
				myUnit.remainingMove += myUnit.movespeed - (int)myUnit.currentPath.Last().cost;
				--myUnit.actionPoints;
				hasAttacked = true;
			}
		}
		
		
		if (myUnit.moving) {
			myMap.CullPath ();
			myMap.GetNode (myUnit.tileX, myUnit.tileY).myUnit = null;
			myUnit.currentPath.Last ().myUnit = myUnit;
		}
	}

	//moves up to an ally unit
	void MoveToSupportAlly() {
		myUnit.currentPath.Remove (target);

		// the unit is next to its target
		if (myUnit.currentPath.Count == 0) {
			//pass go
			myUnit.ShowCombatText("Passed", myUnit.statusCombatText);
		} 
		//the unit is in normal move range
		else if (myUnit.currentPath.Count <= myUnit.movespeed) {
			myUnit.moving = true;
			myUnit.remainingMove -= (int)myUnit.currentPath.Last().cost;  // there may not be a 
			hasAttacked = true;
			inCloseCombat = true;
		} 
		// if the unit is really far away, dash
		else{
			//check to see if it is now in melee
			if (myUnit.currentPath.Count <= myUnit.movespeed*2) {
				inCloseCombat = true;
			} else {
				inCloseCombat = false;
			}

			myStrat = AIStrategy.Dash;
			myUnit.moving = true;
			FindFurthestTileInPath();
			myUnit.remainingMove += myUnit.movespeed - (int)myUnit.currentPath.Last().cost;
			--myUnit.actionPoints;
			hasAttacked = true;
		}

		if (myUnit.moving) {
			myMap.CullPath ();
			myMap.GetNode (myUnit.tileX, myUnit.tileY).myUnit = null;
			myUnit.currentPath.Last ().myUnit = myUnit;
		}
		
		
	}
	
	public void Attack() {
		//temp needs to have weighted priority
		if (!hasAttacked) {
			myUnit.myAbilities[0].UseAbility(target.myUnit);
			hasAttacked = true;
		}
	}

	void FindFurthestTileInPath() {
		Node curr = myUnit.currentPath.Last ();

		while (curr.cost > myUnit.remainingMove + myUnit.movespeed) {
			myUnit.currentPath.Remove(curr);
			curr = myUnit.currentPath.Last ();
		}
	}

}
