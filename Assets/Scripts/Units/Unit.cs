using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Unit : MonoBehaviour {

	public int tileX;
	public int tileY;
	public TileMap map;

	public List<Node> currentPath;
	public List<Node> reachableTiles;
	public List<Node> reachableTilesWithDash;

	public int maxHP = 100;
	public int maxMana = 100;
	public int movespeed = 4;
	public int damageReduction = 16;
	public int init = 5;
	public int maxAP = 1;
	public bool teir1 = true;

	public int HP;
	public int Mana;
	public int remainingMove;
	public int actionPoints;

	public bool isDead = false;

	public bool playable = true; // if its an npc or playable character
	public bool moving 		= false;
	public bool attacking 	= false;

	void Start() {
		HP = maxHP;
		Mana = maxMana;
		remainingMove = movespeed;
		actionPoints = maxAP;
	}

	public void StartTurn() {
		remainingMove = movespeed;
		actionPoints = maxAP;
	}

	void Update() {
		//call the correct update method depending on unit
		if (playable) {
			PlayerUpdate();
		} else {
			//AIUpdate();
		}
	}

	public void PlayerUpdate() {
		if (currentPath != null) {
			int currNode = 0;
			Vector3 start = map.TileCoordToWorldCoord(tileX, tileY) +
				new Vector3(0,0,-1.5f);
			Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
				new Vector3(0,0,-1.5f);

			Debug.DrawLine(start, end, Color.blue);

			while ( currNode < currentPath.Count - 1) {
				start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
					new Vector3(0,0,-1.5f);
				end = map.TileCoordToWorldCoord(currentPath[currNode+1].x, currentPath[currNode+1].y) +
					new Vector3(0,0,-1.5f);
				
				Debug.DrawLine(start, end, Color.blue);
				
				++currNode;
			}
		}
		
		if (moving) {
			map.UnhighlightTiles();
			// Have we moved our visible piece close enough to the target tile that we can
			// advance to the next step in our pathfinding?
			if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.1f) {
				SlideMovement ();
				if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.1f) {
					// if the unit can still move
					if (remainingMove > 0 || actionPoints > 0) {
						map.FindReachableTiles ();
						map.HighlightTiles (reachableTiles, Color.blue);
						map.HighlightTiles (reachableTilesWithDash, new Color(0.5f,1,0));
					}
					moving = false;
					transform.position = map.TileCoordToWorldCoord (tileX, tileY);
				}
			}
			
			// Smoothly animate towards the correct map tile.
			transform.position = Vector3.Lerp (transform.position, map.TileCoordToWorldCoord (tileX, tileY), 5f * Time.deltaTime);
		}
	}

	public void AIUpdate() {

		AIBehaviours myAI = GetComponent<AIBehaviours> ();

		if (moving) {
			if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.01f) {
				SlideMovement ();
				if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.01f) {
					moving = false;
					transform.position = map.TileCoordToWorldCoord (tileX, tileY);
				}

			}

			// Smoothly animate towards the correct map tile.
			transform.position = Vector3.Lerp (transform.position, map.TileCoordToWorldCoord (tileX, tileY), 5f * Time.deltaTime);
		} else if (attacking) {
			myAI.Attack();
			attacking = false;
		}
		else {
			remainingMove = 0;
			actionPoints = 0;
		}
	}

	public void SlideMovement() {
		// if there is no path return
		if(currentPath==null)
			return;

		// get the new first node and move
		tileX = currentPath[0].x;
		tileY = currentPath[0].y;

		// remove the old tile
		currentPath.RemoveAt (0);

		//at the target tile
		if (currentPath.Count == 0) {
			//map.GetNode (tileX, tileY).occupied = true;
			//clear path finding info
			currentPath = null;
		}


	}

	public void MoveNextTile() {
		map.UnhighlightTiles();

		while (remainingMove > 0 || actionPoints > 0) {

			if (currentPath == null || currentPath.Count == 1) {
				break;
			}

			if (remainingMove < 1 && currentPath.Count > 1) {
				remainingMove = movespeed;
				--actionPoints;
				
			}

			//map.GetNode (tileX, tileY).occupied = false;
			// remove the old tile
			currentPath.RemoveAt (0);

			remainingMove -= (int)map.CostToEnterTile(currentPath [0].x, currentPath [0].y);

			// get the new first node and move
			tileX = currentPath[0].x;
			tileY = currentPath[0].y;
			transform.position = map.TileCoordToWorldCoord(tileX, tileY);

			//at the target tile
			if (currentPath.Count == 1) {
				//map.GetNode (tileX, tileY).occupied = true;
				//clear path finding info
				currentPath = null;
			}
		}

		if (remainingMove > 0 || actionPoints > 0) {
			map.FindReachableTiles ();
			map.HighlightTiles (reachableTiles, Color.blue);
			map.HighlightTiles (reachableTilesWithDash, new Color(0.5f,1,0));
		}
	}

	public void RemoveMovement() {
		//remove movement cost
		if (currentPath.Last ().cost > remainingMove) {
			remainingMove += movespeed - (int)currentPath.Last ().cost;
			--actionPoints;
		} else {
			remainingMove -= (int)currentPath.Last ().cost;
		}
	}
}
