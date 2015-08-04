﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum Tile {
	FLOOR,
	CARPET,
	WALL,
	MAXTILETYPE
}

public class TileMap : MonoBehaviour {

	public TextAsset loadedXml;
	public GameObject selectedUnit;

	public Level currentLevel;

	public GameObject[] tileObjects;
	public TileType[] tileTypes;
	Node[] graph;

	//Setup method
	public void Initialise() {
		LevelLoader lLoader = new LevelLoader ();
		lLoader.ReadInFile (loadedXml);
		currentLevel = lLoader.GetLevel (0);

		GeneratePathfindingGraph ();
		GenerateMapVisuals ();
	}

	public float CostToEnterTile (int x, int y)
	{
		TileType tt = tileTypes[(int)currentLevel.tiles[y * currentLevel.maxSizeX + x]];

		if (!tt.isWalkable) { // || graph [y * currentLevel.maxSizeX + x].occupied) {
			return Mathf.Infinity;
		}

		return tt.movementCost;
	}

	public Vector3 TileCoordToWorldCoord(int x, int y) {
		return new Vector3 (x, y, -2) + transform.position;
	}
	
	public bool UnitCanEnterTile(int x, int y)
	{
		return tileTypes [(int)currentLevel.tiles [y * currentLevel.maxSizeX + x]].isWalkable && GetNode(x, y).myUnit == null;
	}

	public bool TileBlocksVision(int x, int y)
	{
		return tileTypes [(int)currentLevel.tiles [y * currentLevel.maxSizeX + x]].blocksVision;
	}

	public Node GetNode(int x, int y) {
		return graph [y * currentLevel.maxSizeX + x];
	}

	public ClickableTile GetClickableTile(int x, int y) {
		return tileObjects [y * currentLevel.maxSizeX + x].GetComponent<ClickableTile>();
	}

	// generates nodes for each tile
	void GeneratePathfindingGraph() {
		graph = new Node[currentLevel.maxSizeX * currentLevel.maxSizeY];

		//create nodes in array
		for (int y=0; y < currentLevel.maxSizeY; ++y) {
			for (int x=0; x < currentLevel.maxSizeX; ++x) {
				graph [y * currentLevel.maxSizeX + x] = new Node ();
				graph [y * currentLevel.maxSizeX + x].x = x;
				graph [y * currentLevel.maxSizeX + x].y = y;
			}
		}

		//find neighbours
		for (int y=0; y < currentLevel.maxSizeY; ++y) {
			for (int x=0; x < currentLevel.maxSizeX; ++x) {

				//set all neighbours
				if (x > 0) {
					graph[y * currentLevel.maxSizeX + x].neighbours[0] = graph[y*currentLevel.maxSizeX + x-1]; //left
				}
				if (x < currentLevel.maxSizeX-1) {
					graph[y * currentLevel.maxSizeX + x].neighbours[1] = graph[y*currentLevel.maxSizeX + x+1]; //right
				}
				if (y > 0) {
					graph[y * currentLevel.maxSizeX + x].neighbours[2] = graph[(y-1)*currentLevel.maxSizeX + x]; //down
				}
				if (y < currentLevel.maxSizeY-1) {
					graph[y * currentLevel.maxSizeX + x].neighbours[3] = graph[(y+1)*currentLevel.maxSizeX + x]; //up
				}

			}
		}

	}

	//creates the planes in the world to see
	void GenerateMapVisuals() {

		tileObjects = new GameObject[currentLevel.maxSizeY * currentLevel.maxSizeX];

		//initialize map tiles
		for (int y=0; y < currentLevel.maxSizeY; ++y) {
			for (int x=0; x < currentLevel.maxSizeX; ++x) {
				TileType tt = tileTypes[(int)currentLevel.tiles[y * currentLevel.maxSizeX + x]];

				int z = 0;

				if (currentLevel.tiles[y * currentLevel.maxSizeX + x] == Tile.WALL) {
					z = -3;
				}

				Vector3 pos = new Vector3(x, y, z) + transform.position;
				Quaternion rot = transform.rotation;

				GameObject go = (GameObject)Instantiate(tt.tileVisualPrefab, pos, rot);

				tileObjects[y * currentLevel.maxSizeX + x] = go;

				ClickableTile ct = go.GetComponent<ClickableTile>();
				ct.tileX = x;
				ct.tileY = y;
				ct.map = this;
				ct.uManager = GetComponent<UnitManager>();
			}
		}
	}

	// finds a path using dijkstra's algorithm
	public void GeneratePathTo(int x, int y) {
		List<Node> currentPath = null;
		Unit sUnit = selectedUnit.GetComponent<Unit> ();

		if (!UnitCanEnterTile (x, y)) {
			//return;
		}

		List<Node> unvisited = new List<Node> ();

		Node source = graph [sUnit.tileY * currentLevel.maxSizeX + sUnit.tileX];
		Node target = graph [y * currentLevel.maxSizeX + x];
		

		source.cost = 0;
		source.previous = null;

		// Initialize everything to have inf distance
		foreach(Node n in graph) {
			if (n != source) {
				n.cost = Mathf.Infinity;
				n.previous = null;
			}
			unvisited.Add(n);
		}

		// look through all unvisited nodes
		while (unvisited.Count > 0) {
			//find current lowest cost tile
			Node u = null;
			foreach (Node n in unvisited) {
				if ((u == null || n.cost < u.cost) && (n.myUnit == null || n == source || n == target)){
					u = n;
				}
			}

			if (u == target) {
				break; // found the end, exit
			}

			unvisited.Remove(u);

			// look through the neibours and set the shortest path
			foreach(Node n in u.neighbours) {
				if (n != null)
				{
					//float alt = dist[u] + u.distanceTo(n);
					float alt = u.cost + CostToEnterTile(n.x, n.y);
					if (alt < n.cost) {
						n.cost = alt;
						n.previous = u;
						n.directionToParent = new Vector2(u.x - n.x, u.y - n.y);
					}
				}
			}
		}

		//was there no possible path
		if (target.previous == null) {
			return;
		}

		//must have a path
		currentPath = new List<Node> ();
		Node curr = target;

		//loop through the chain and add to path
		while (curr != null) {
			if (curr != source){
				currentPath.Add(curr);
			}
			curr = curr.previous;
		}

		//reverse the path as its backwards
		currentPath.Reverse();

		sUnit.currentPath = currentPath;
	}

	//using dijkstra's algorithm it finds every tile that can be reached
	public void FinReachableTilesUnit() {
		Unit sUnit = selectedUnit.GetComponent<Unit> ();
		bool canDash = sUnit.actionPoints > 0;

		List<Node> reachableTiles = new List<Node> ();
		List<Node> reachableTilesDash = new List<Node> ();

		List<Node> unvisited = new List<Node> ();

		Node source = graph [sUnit.tileY * currentLevel.maxSizeX + sUnit.tileX];

		source.previous = null;
		source.cost = 0;

		// Initialize everything to have inf distance
		foreach(Node n in graph) {
			if (n != source) {
				n.cost = Mathf.Infinity;
				n.previous = null;
			}
			unvisited.Add(n);
		}



		// look through all unvisited nodes
		while (unvisited.Count > 0) {
			//find current lowest cost tile
			Node u = null;
			foreach (Node n in unvisited) {
				if (u == null || (n.cost < u.cost && (!n.myUnit || n == source))){
					u = n;
				}
			}

			//if the lowest cost is higher than the max move exit
			if (u.cost <= sUnit.remainingMove) {
				reachableTiles.Add(u);
			}
			else if (canDash && u.cost <= sUnit.remainingMove + sUnit.movespeed) {
				reachableTilesDash.Add(u);
			}
			else {
				break;
			}

			unvisited.Remove(u);
			
			// look through the neibours and set the shortest path
			foreach(Node n in u.neighbours) {
				if (n != null)
				{
					float alt = u.cost + CostToEnterTile(n.x, n.y);
					if (alt < n.cost) {
						n.cost = alt;
						n.previous = u;
						n.directionToParent = new Vector2(u.x - n.x, u.y - n.y);
					}
				}
			}
		}

		sUnit.reachableTiles = reachableTiles;
		sUnit.reachableTiles.RemoveAt (0);
		sUnit.reachableTilesWithDash = reachableTilesDash;

	}

	public void FindReachableTilesUnit() {
		Unit sUnit = selectedUnit.GetComponent<Unit> ();
		bool canDash = sUnit.actionPoints > 0;

		int range = sUnit.remainingMove + (sUnit.movespeed * Convert.ToInt32(canDash));
		
		List<Node> reachableTiles = new List<Node> ();
		List<Node> reachableTilesDash = new List<Node> ();

		foreach (Node n in FindReachableTiles(sUnit.tileX, sUnit.tileY, range, false)) {
			//if the lowest cost is higher than the max move exit
			if (n.cost <= sUnit.remainingMove) {
				reachableTiles.Add(n);
			}
			else if (n.cost <= range) {
				reachableTilesDash.Add(n);
			}
			else {
				break;
			} 
		}

		sUnit.reachableTiles = reachableTiles;
		sUnit.reachableTiles.RemoveAt (0);
		sUnit.reachableTilesWithDash = reachableTilesDash;
	}

	//using dijkstra's algorithm it finds every tile that can be reached
	public List<Node> FindReachableTiles(int x, int y, int range, bool ignoreSlows) {
		
		List<Node> unvisited = new List<Node> ();
		List<Node> reachableTiles = new List<Node> ();

		Node source = graph [y * currentLevel.maxSizeX + x];
		
		source.previous = null;
		source.cost = 0;
		
		// Initialize everything to have inf distance
		foreach(Node n in graph) {
			if (n != source) {
				n.cost = Mathf.Infinity;
				n.previous = null;
			}
			unvisited.Add(n);
		}
		
		
		
		// look through all unvisited nodes
		while (unvisited.Count > 0) {
			//find current lowest cost tile
			Node u = null;
			foreach (Node n in unvisited) {
				if (u == null || (n.cost < u.cost && (!n.myUnit || ignoreSlows || n == source))){
					u = n;
				}
			}

			//if its further than the range
			if (u.cost > range) {
				break;
			}

			reachableTiles.Add(u);

			unvisited.Remove(u);
			
			// look through the neibours and set the shortest path
			foreach(Node n in u.neighbours) {
				if (n != null)
				{
					float alt;
					if (CostToEnterTile(n.x, n.y) == Mathf.Infinity || !ignoreSlows) {
						alt = u.cost + CostToEnterTile(n.x, n.y);
					}
					else
					{
						alt = u.cost + 1;
					}

					if (alt < n.cost) {
						n.cost = alt;
						n.previous = u;
						n.directionToParent = new Vector2(u.x - n.x, u.y - n.y);
					}
				}
			}
		}

		return reachableTiles;
		
	}

	//gets the target square and ggets the path already generated to it
	public void GetPath(int x, int y) {

		Unit sUnit = selectedUnit.GetComponent<Unit> ();

		List<Node> currentPath = new List<Node> ();
		Node curr = graph[y * currentLevel.maxSizeX + x];
		
		//loop through the chain and add to path
		while (curr != null) {
			if (curr != graph[sUnit.tileY * currentLevel.maxSizeX + sUnit.tileX]){
				currentPath.Add(curr);
			}
			curr = curr.previous;
		}
		
		//reverse the path as its backwards
		currentPath.Reverse();
		
		sUnit.currentPath = currentPath;

	}

	public void FollowPath() {
		Unit sUnit = selectedUnit.GetComponent<Unit> ();

		CullPath ();

		sUnit.moving = true;
		//set the node's unit
		graph[sUnit.tileY * currentLevel.maxSizeX + sUnit.tileX].myUnit = null;
		graph[sUnit.currentPath.Last().y * currentLevel.maxSizeX + sUnit.currentPath.Last().x].myUnit = sUnit;
		
		sUnit.RemoveMovement();
	}

	public void HighlightTiles(List<Node> HTiles, Color colour, Color hover, int targetable)  {
		foreach (Node n in HTiles) {
			tileObjects[n.y * currentLevel.maxSizeX + n.x].GetComponent<ClickableTile>().HighlightTile(colour, hover, targetable);
		}
	}

	public void UnhighlightTiles(List<Node> HTiles)  {
		foreach (Node n in HTiles) {
			tileObjects[n.y * currentLevel.maxSizeX + n.x].GetComponent<ClickableTile>().UnhighlightTile();
		}
	}

	public void UnhighlightTiles()  {
		foreach (GameObject n in tileObjects) {
			n.GetComponent<ClickableTile>().UnhighlightTile();
		}
	}

	public void CullPath() {
		Unit sUnit = selectedUnit.GetComponent<Unit> ();

		Vector2 currentDirection = new Vector2(0, 0);

		List<Node> culledList = new List<Node> ();

		//loop through all the current path nodes
		for (int i = 0; i < sUnit.currentPath.Count; ++i)
		{
			//check to see if the unit has stepped on a trigger
			if (sUnit.currentPath[i].myTrigger != null) {
				Trigger trig = sUnit.currentPath[i].myTrigger;
				if (trig.myTargets == TargetType.All || (trig.myTargets == TargetType.Ally && trig.myCaster.team == sUnit.team) || (trig.myTargets == TargetType.Enemy && trig.myCaster.team != sUnit.team)) {

					//still need to check to see if last tile change direction
					if (sUnit.currentPath[i].directionToParent != currentDirection) {
						culledList.Add(sUnit.currentPath[i].previous);
					}

					//add the trigger as the last tile
					culledList.Add(sUnit.currentPath[i]);
					break;
				}
			}

			// if its the first node set the starting direction
			if ( i == 0) {
				currentDirection = sUnit.currentPath[i].directionToParent;
			}
			// else if the direction changed add the previous node
			else if (sUnit.currentPath[i].directionToParent != currentDirection) {
				culledList.Add(sUnit.currentPath[i].previous);
				currentDirection = sUnit.currentPath[i].directionToParent;
			}

			// if its the final node add it
			if ( i == sUnit.currentPath.Count - 1) {
				culledList.Add(sUnit.currentPath[i]);
			}

		}

		sUnit.currentPath = culledList;
	}

	public List<Node> FindSingleRangedTargets(Ability abil) {
		Unit sUnit = selectedUnit.GetComponent<Unit> ();

		List<Node> reachNodes = FindReachableTiles (sUnit.tileX, sUnit.tileY, abil.range, true);
		List<Node> targetableNodes = new List<Node> ();

		//remove the tile the unit is stood on
		reachNodes.RemoveAt(0);

		//check line of site for each tile
		foreach (Node n in reachNodes) {
			if (HasLineOfSight(GetNode(sUnit.tileX, sUnit.tileY), n)) {
				targetableNodes.Add(n);
			}
		}

		return targetableNodes;
	}

	public List<Node> FindSelfTarget() {
		List<Node> targetableNodes = new List<Node> ();

		targetableNodes.Add (GetNode (selectedUnit.GetComponent<Unit>().tileX, selectedUnit.GetComponent<Unit>().tileY));

		return targetableNodes;
	}

	public List<Node> FindLineTargets(Ability abil) {
		Unit sUnit = selectedUnit.GetComponent<Unit>();
		List<Node> targetableNodes = new List<Node> ();
		bool[] hitwall = new bool[4];

		//loop for the max range
		for (int i=1; i < abil.range; ++i) {
			//make sure the bools are false
			if (i==1) {
				hitwall[0] = false;
				hitwall[1] = false;
				hitwall[2] = false;
				hitwall[3] = false;
			}

			//if this direction has not yet hit a wall
			if (!hitwall[0]) {
				//check to see if the next tile is a wall;
				if (CostToEnterTile(sUnit.tileX+i, sUnit.tileY) == Mathf.Infinity) {
					hitwall[0] = true;
				}
				else {
					if (i != 1)
					{
						GetNode(sUnit.tileX+i, sUnit.tileY).previous = GetNode(sUnit.tileX+i-1, sUnit.tileY);
					} else {
						GetNode(sUnit.tileX+i, sUnit.tileY).previous = null;
					}
					GetNode(sUnit.tileX+i, sUnit.tileY).directionToParent = new Vector2(-1, 0);
					targetableNodes.Add(GetNode(sUnit.tileX+i, sUnit.tileY));

					if (GetNode(sUnit.tileX+i, sUnit.tileY).myUnit != null) {
						hitwall[0] = true;
					}
				}

			}

			//if this direction has not yet hit a wall
			if (!hitwall[1]) {
				//check to see if the next tile is a wall;
				if (CostToEnterTile(sUnit.tileX-i, sUnit.tileY) == Mathf.Infinity) {
					hitwall[1] = true;
				}
				else {
					if (i != 1)
					{
						GetNode(sUnit.tileX-i, sUnit.tileY).previous = GetNode(sUnit.tileX-i+1, sUnit.tileY);
					} else {
						GetNode(sUnit.tileX-i, sUnit.tileY).previous = null;
					}
					GetNode(sUnit.tileX-i, sUnit.tileY).directionToParent = new Vector2(1, 0);
					targetableNodes.Add(GetNode(sUnit.tileX-i, sUnit.tileY));

					if (GetNode(sUnit.tileX-i, sUnit.tileY).myUnit != null) {
						hitwall[1] = true;
					}
				}
				
			}

			//if this direction has not yet hit a wall
			if (!hitwall[2]) {
				//check to see if the next tile is a wall;
				if (CostToEnterTile(sUnit.tileX, sUnit.tileY+i) == Mathf.Infinity) {
					hitwall[2] = true;
				}
				else {
					if (i != 1)
					{
						GetNode(sUnit.tileX, sUnit.tileY+i).previous = GetNode(sUnit.tileX, sUnit.tileY+i-1);
					} else {
						GetNode(sUnit.tileX, sUnit.tileY+i).previous = null;
					}
					GetNode(sUnit.tileX, sUnit.tileY+i).directionToParent = new Vector2(0, -1);
					targetableNodes.Add(GetNode(sUnit.tileX, sUnit.tileY+i));

					if (GetNode(sUnit.tileX, sUnit.tileY+i).myUnit != null) {
						hitwall[2] = true;
					}
				}

			}

			//if this direction has not yet hit a wall
			if (!hitwall[3]) {
				//check to see if the next tile is a wall;
				if (CostToEnterTile(sUnit.tileX, sUnit.tileY-i) == Mathf.Infinity) {
					hitwall[3] = true;
				}
				else {
					if (i != 1)
					{
						GetNode(sUnit.tileX, sUnit.tileY-i).previous = GetNode(sUnit.tileX, sUnit.tileY-i+1);
					} else {
						GetNode(sUnit.tileX, sUnit.tileY-i).previous = null;
					}

					GetNode(sUnit.tileX, sUnit.tileY-i).directionToParent = new Vector2(0, 1);
					targetableNodes.Add(GetNode(sUnit.tileX, sUnit.tileY-i));

					if (GetNode(sUnit.tileX, sUnit.tileY-i).myUnit != null) {
						hitwall[3] = true;
					}
				}
				
			}

		}

		return targetableNodes;
	}

	//TODO YEAH DO THIS WHEN HELL FREEZES OVER
	public List<Node> FindConeTargets(Ability abil) {
		List<Node> targetableNodes = new List<Node> ();

		return targetableNodes;
	}

	public bool HasLineOfSight(Node start, Node end) {
		int deltaX = Math.Abs (end.x - start.x);
		int deltaY = Math.Abs (end.y - start.y);
		int stepX = -1;
		int stepY = -1;
		int error = 0;
		int x = start.x;
		int y = start.y;

		//find out which way the x and y should be stepping
		if (start.x < end.x) {
			stepX = 1;
		}

		if (start.y < end.y) {
			stepY = 1;
		}

		error = deltaX - deltaY;

		//temp
		int count = 0;

		while (!(x == end.x && y == end.y) && count < deltaX + deltaY) {
			float twoError = 2* error;
			if (twoError > (-1*deltaY)) {
				error -= deltaY;
				x += stepX;
			}
			if (twoError < deltaX) {
				error += deltaX;
				y += stepY;
			}

			if (TileBlocksVision(x, y)) {
				return false;
			}
			count++;
		}
		return true;
	}


}
