using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public enum Behaviour {
	Dumb,
	Scared,
	Ranged,
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

	// Use this for initialization
	void Start () {
		myUnit = GetComponent<Unit> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FSM() {
		switch (myBehaviour) {
		case Behaviour.Dumb: FindTargetClosest();
			Dumb ();
			break;
		}
	}

	//TODO check there was atleast 1 path
	//find target depending on behaviour
	void FindTargetClosest() {

		List<Node> shortestPath = new List<Node> ();
		myUnit.currentPath = new List<Node> ();
		float currentPathLength = Mathf.Infinity;
		int tx = 0;
		int ty = 0;


		// for each character, find path
		for (int i = 0; i < myManager.characterCount; ++i) {
			Unit targetUnit = myManager.playerUnitObjects[i].GetComponent<Unit>();

			//if the target is alive
			if (!targetUnit.isDead) {

				//find the path to the target
				myMap.GeneratePathTo(targetUnit.tileX, targetUnit.tileY);

				// if the path is shorter than current path
				if (myUnit.currentPath.Count < currentPathLength) {
					// set shortest path
					shortestPath = myUnit.currentPath;
					currentPathLength = myUnit.currentPath.Count;
					tx = targetUnit.tileX;
					ty = targetUnit.tileY;
				}
			}
		}

		//set the target
		target = myMap.GetNode (tx, ty);
		shortestPath.Remove (target);
		myUnit.currentPath = shortestPath;


	}

	// run to and attack closest target
	void Dumb(){
		// is it already in melee?
		if (myUnit.currentPath.Count < 1) {
			myStrat = AIStrategy.Attack;
			myUnit.attacking = true;
		} else if (myUnit.currentPath.Count <= myUnit.movespeed) {
			myStrat = AIStrategy.MoveAttack;
			myUnit.moving = true;
			myUnit.attacking = true;
			myUnit.remainingMove -= (int)myUnit.currentPath.Last().cost;
		} else {
			myStrat = AIStrategy.Dash;
			myUnit.moving = true;
			FindFurthestTileInPath();
			myUnit.remainingMove += myUnit.movespeed - (int)myUnit.currentPath.Last().cost;
			--myUnit.actionPoints;
		}

		if (myUnit.moving) {
			myMap.CullPath ();
			myMap.GetNode (myUnit.tileX, myUnit.tileY).myUnit = null;
			myUnit.currentPath.Last ().myUnit = myUnit;
		}
	}

	//TODO MAKE THIS
	public void Attack() {
		//temp
		//myUnit.remainingMove = 0;
		//myUnit.actionPoints = 0;
	}

	void FindFurthestTileInPath() {
		Node curr = myUnit.currentPath.Last ();

		while (curr.cost > myUnit.remainingMove + myUnit.movespeed) {
			myUnit.currentPath.Remove(curr);
			curr = myUnit.currentPath.Last ();
		}
	}

}
