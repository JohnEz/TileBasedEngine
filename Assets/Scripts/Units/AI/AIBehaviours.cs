using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


public enum Behaviour {
	Dumb,
	Scared,
	DumbRanged,
	Support,
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
	public Unit myUnit;

	public Node target;
	public List<Unit> myGroup;
	
	public bool hasAttacked = false; // if the unit is meant to attack only allow it to once
	public bool inCloseCombat = false; // if the unit is in melee with another
	public bool turnPlanned = false;
	public bool foundTarget = false;

	public int selectedAbility = 0;

	// Use this for initialization
	public void Initialise () {
		myUnit = GetComponent<Unit> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void PickAbility() {
		int total = 0;

		//loop through every ability and add their priorities
		foreach (Ability abil in myUnit.myAbilities) {
			if (CanUseAbility(abil)) {
				total += abil.AIPriority;
			}
		}

		int roll = UnityEngine.Random.Range (1, total);
		int count = 0;

		for (int i = 0; i < myUnit.myAbilities.Count(); ++i) {
			if (CanUseAbility(myUnit.myAbilities[i])) {
				count += myUnit.myAbilities[i].AIPriority;
				if (count >= roll) {
					selectedAbility = i;
					break;
				}
			}
		}
	}

	public bool CanUseAbility(Ability abil) {
		if (abil != null && abil.cooldown < 1 && abil.manaCost <= myUnit.mana) {
			return true;
		}
		return false;
	}

	public virtual void FSM() {

		GameObject[] targets;

		if (myUnit.actionPoints < 1 && myUnit.remainingMove < 1) {
			turnPlanned = true;
			return;
		}

		PickAbility ();

		//does it target ally or enemy
		if (myUnit.myAbilities [selectedAbility].AISupportsAlly) {
			targets = myManager.enemies;
		} else {
			targets = myManager.playerUnitObjects;
		}

		//is it a ranged or melee move?
		if (myUnit.myAbilities [selectedAbility].area == AreaType.Self) {
			target = myMap.GetNode (myUnit.tileX, myUnit.tileY);
			myUnit.currentPath = new List<Node> ();

			target.reachableNodes = new List<Node> ();

			//add all allies to reachable nodes
			foreach (GameObject go in myManager.enemies) {
				if (go != null) {
					Unit sUnit = go.GetComponent<Unit> ();

					if (sUnit != null && sUnit.isActive && !sUnit.isDead) {
						target.reachableNodes.Add (myMap.GetNode (sUnit.tileX, sUnit.tileY));
					}
				}
			}
		} else if (myUnit.myAbilities [selectedAbility].area == AreaType.SelfAOE) {
			target = myMap.GetNode (myUnit.tileX, myUnit.tileY);
			myUnit.currentPath = new List<Node> ();

			//get aoe around myself
			target.reachableNodes = myMap.FindReachableTiles (myUnit.tileX, myUnit.tileY, myUnit.myAbilities [selectedAbility].AOERange, true);


		} else if (myUnit.myAbilities [selectedAbility].AIRanged) {
			FindClosestLoS(targets);
		} else {
			FindTargetClosest(targets, false);
		}

		switch (myBehaviour) {
		case Behaviour.Dumb: Dumb ();
			break;
		case Behaviour.DumbRanged: DumbRanged();
			break;
		case Behaviour.Support: DumbRanged();
			break;
		}

		turnPlanned = true;
	}

	//find target closest to the unit, from array
	void FindTargetClosest(GameObject[] targets, bool ignoreUnits) {

		foundTarget = false;

		//if stunned
		if (myUnit.remainingMove <= 0 && myUnit.actionPoints <= 0 && myUnit.movespeed == 0) {
			return;
		}

		List<Node> shortestPath = new List<Node> ();
		myUnit.currentPath = new List<Node> ();
		float currentPathCost = Mathf.Infinity;
		int tx = 0;
		int ty = 0;


		// for each character, find path
		for (int i = 0; i < targets.Count(); ++i) {
			//get the target
			Unit targetUnit = targets[i].GetComponent<Unit>();
			if (targetUnit != null) {
				//if the target is alive
				if (!targetUnit.isDead && targetUnit != myUnit && targetUnit.isActive) {

					//find the path to the target
					myMap.GeneratePathTo(targetUnit.tileX, targetUnit.tileY, ignoreUnits);
					//if a path was found
					if (myUnit.currentPath.Count > 0) {
						//remove the targets node
						myUnit.currentPath.Remove(myMap.GetNode (targetUnit.tileX, targetUnit.tileY));
						//remove all unreachable tiles
						FindFurthestTileInPath();

						// if the target is next to the unit
						if (myUnit.currentPath.Count == 0) {
							// if the current closest isnt already next to
							if (currentPathCost != 0) {
								foundTarget = true;
								// set shortest path
								shortestPath = myUnit.currentPath;
								currentPathCost = 0;
								tx = targetUnit.tileX;
								ty = targetUnit.tileY;
							}
						} //else if the target is closer than the previous
						else if (myUnit.currentPath.Last().cost < currentPathCost) {
							// set shortest path
							foundTarget = true;
							shortestPath = myUnit.currentPath;
							currentPathCost = myUnit.currentPath.Last().cost;
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

	//find target closest to the unit, from array
	void FindClosestAllyInMelee() {

		foundTarget = false;

		//if stunned
		if (myUnit.remainingMove <= 0 && myUnit.actionPoints <= 0 && myUnit.movespeed == 0) {
			return;
		}
		
		List<Node> shortestPath = new List<Node> ();
		myUnit.currentPath = new List<Node> ();
		float currentPathCost = Mathf.Infinity;
		
		
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

						//if a path was found
						if (myUnit.currentPath.Count > 0) {
							//remove the targets node
							myUnit.currentPath.Remove(myMap.GetNode (targetUnit.tileX, targetUnit.tileY));
							//remove all unreachable tiles
							FindFurthestTileInPath();
							//cull the path to reduce size
							//myMap.CullPath();
							
							// if the target is next to the unit
							if (myUnit.currentPath.Count == 0) {
								// set shortest path
								foundTarget = true;
								shortestPath = myUnit.currentPath;
								currentPathCost = 0;
								//set the target
								target = myMap.GetNode (targetUnit.tileX, targetUnit.tileY);
							} //else if the target is closer than the previous
							else if (myUnit.currentPath.Last().cost < currentPathCost) {
								// set shortest path
								foundTarget = true;
								shortestPath = myUnit.currentPath;
								currentPathCost = myUnit.currentPath.Last().cost;
								//set the target
								target = myMap.GetNode (targetUnit.tileX, targetUnit.tileY);
							}
						}
					}
				}
			}
		}
		

		myUnit.currentPath = shortestPath;
	}

	void SupportAllyInMelee() {
		FindClosestAllyInMelee();
	}

	// run to and attack closest target
	void Dumb(){

		// if the ai is stunned, or didnt have a path for some reason
		if (myUnit.currentPath == null) {
			return;
		}

		//if no path could be found, look for an ally i can support
		if (!foundTarget) {
			SupportAllyInMelee ();

			//if no path could be found still, there isnt even an ally to support
			if (!foundTarget) {
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

	public void DumbRanged() {
		//if no path could be found, look for an ally i can support
		if (myUnit.currentPath == null) {
			//pass go
			myUnit.ShowCombatText ("Passed", myUnit.statusCombatText);
			return;
		}

		BasicRangedTurn ();

	}

	void FindClosestLoS(GameObject[] targets) {

		//if stunned return null
		if (myUnit.remainingMove <= 0 && myUnit.actionPoints <= 0 && myUnit.movespeed == 0) {
			return;
		}
		
		List<Node> shortestPath = null;
		myUnit.currentPath = null;
		float currentPathCost = Mathf.Infinity;
		int currentDist = 0;
		

		
		//check each unit
		foreach (GameObject go in targets) {
			Unit sUnit = go.GetComponent<Unit>();
			if (!sUnit.isDead && sUnit.isActive && sUnit != myUnit) {
				List<Node> losNodes = myMap.FindSingleRangedTargets (myUnit.myAbilities [selectedAbility], sUnit, true);
				if (myMap.AIFindClosestTile(sUnit.tileX, sUnit.tileY, losNodes)) {

					myUnit.currentPath.Remove(myMap.GetNode (sUnit.tileX, sUnit.tileY));
					FindFurthestTileInPath();
					//myMap.CullPath();

					// if the path is shorter than current path
					if (myUnit.currentPath.Count == 0) {

						//if there was already a target check this one is closer
						if (foundTarget && currentPathCost == 0){
							//if the manhattan distance is closer
							if (currentDist > FindManDistance(myUnit.tileX, myUnit.tileY, sUnit.tileX, sUnit.tileY)) {
								// set shortest path
								shortestPath = myUnit.currentPath;
								target = myMap.GetNode(sUnit.tileX, sUnit.tileY);
								currentDist = FindManDistance(myUnit.tileX, myUnit.tileY, target.x, target.y);
							}
						} else {
							foundTarget = true;
							// set shortest path
							shortestPath = myUnit.currentPath;
							currentPathCost = 0;
							target = myMap.GetNode(sUnit.tileX, sUnit.tileY);
							currentDist = FindManDistance(myUnit.tileX, myUnit.tileY, target.x, target.y);
						}
					} else if (myUnit.currentPath.Last().cost < currentPathCost) {
						// set shortest path
						foundTarget = true;
						shortestPath = myUnit.currentPath;
						currentPathCost = myUnit.currentPath.Last().cost;
						target = myMap.GetNode(sUnit.tileX, sUnit.tileY);
					}
				}
			}

		}

		//set the target
		myUnit.currentPath = shortestPath;

		
	}

	int FindManDistance(int x1, int y1, int x2, int y2) {
		int dist = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

		return dist;
	}

	//Basic turn - attack if in range or move and attack, or dash
	void BasicTurn(){

		// is it already in melee?
		if (myUnit.currentPath.Count == 0) {
			myStrat = AIStrategy.Attack;
			myUnit.attacking = true;
			hasAttacked = false;
			inCloseCombat = true;
		} 
		// if target is in first move range, move and attack
		else if (myUnit.remainingMove > 0 || myUnit.movespeed > 0) {
			if (myUnit.currentPath.Last().cost <= myUnit.movespeed) {
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
				if (myUnit.currentPath.Last().cost <= myUnit.movespeed*2) {
					inCloseCombat = true;
				} else {
					inCloseCombat = false;
				}
				myStrat = AIStrategy.Dash;
				myUnit.moving = true;
				myUnit.remainingMove += myUnit.movespeed - (int)myUnit.currentPath.Last().cost;
				--myUnit.actionPoints;
				hasAttacked = true;
			}
		}

		if (myUnit.moving) {
			//myMap.GetNode (myUnit.tileX, myUnit.tileY).myUnit = null;
			//myUnit.currentPath.Last ().myUnit = myUnit;
		}
	}

	//Basic turn - attack if in range or move and attack, or dash
	void BasicRangedTurn(){
		
		// is it already in melee?
		if (myUnit.currentPath.Count == 0) {
			myStrat = AIStrategy.Attack;
			myUnit.attacking = true;
			hasAttacked = false;
		} 
		// if target is in first move range, move and attack
		else if (myUnit.remainingMove > 0 || myUnit.movespeed > 0) {
			if (myUnit.currentPath.Last().cost <= myUnit.movespeed) {
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
				myUnit.remainingMove += myUnit.movespeed - (int)myUnit.currentPath.Last().cost;
				--myUnit.actionPoints;
				hasAttacked = true;
			}
		}
		
		if (myUnit.moving) {
			//myMap.GetNode (myUnit.tileX, myUnit.tileY).myUnit = null;
			//myUnit.currentPath.Last ().myUnit = myUnit;
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
		else if (myUnit.currentPath.Last().cost <= myUnit.movespeed) {
			myUnit.moving = true;
			myUnit.remainingMove -= (int)myUnit.currentPath.Last().cost;  // there may not be a 
			hasAttacked = true;
			inCloseCombat = true;
		} 
		// if the unit is really far away, dash
		else{
			//check to see if it is now in melee
			if (myUnit.currentPath.Last().cost <= myUnit.movespeed*2) {
				inCloseCombat = true;
			} else {
				inCloseCombat = false;
			}

			myStrat = AIStrategy.Dash;
			myUnit.moving = true;
			myUnit.remainingMove += myUnit.movespeed - (int)myUnit.currentPath.Last().cost;
			--myUnit.actionPoints;
			hasAttacked = true;
		}

		if (myUnit.moving) {
			myMap.GetNode (myUnit.tileX, myUnit.tileY).myUnit = null;
			myUnit.currentPath.Last ().myUnit = myUnit;
		}
		
		
	}
	
	public void Attack() {
		//temp needs to have weighted priority
		if (!hasAttacked) {
			myUnit.myAbilities[selectedAbility].UseAbility(target);
			hasAttacked = true;
		}
	}

	void FindFurthestTileInPath() {
		if (myUnit.currentPath.Count > 0) {
			Node curr = myUnit.currentPath.Last ();

			while (curr.cost > myUnit.remainingMove + myUnit.movespeed && myUnit.currentPath.Count > 1) {
				myUnit.currentPath.Remove (curr);
				curr = myUnit.currentPath.Last ();
			}
		}
	}

	public void SpottedPlayer() {
		
	}

}
