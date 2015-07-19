using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
		return new Vector3 (x, y, 0) + transform.position;
	}
	
	public bool UnitCanEnterTile(int x, int y)
	{
		return tileTypes [(int)currentLevel.tiles [y * currentLevel.maxSizeX + x]].isWalkable;
	}

	public Node GetNode(int x, int y) {
		return graph [y * currentLevel.maxSizeX + x];
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

				Vector3 pos = new Vector3(x, y, 0) + transform.position;
				Quaternion rot = transform.rotation;

				GameObject go = (GameObject)Instantiate(tt.tileVisualPrefab, pos, rot);

				tileObjects[y * currentLevel.maxSizeX + x] = go;

				ClickableTile ct = go.GetComponent<ClickableTile>();
				ct.tileX = x;
				ct.tileY = y;
				ct.map = this;
			}
		}
	}

	// finds a path using dijkstra's algorithm
	public void GeneratePathTo(int x, int y) {
		List<Node> currentPath = null;
		Unit sUnit = selectedUnit.GetComponent<Unit> ();

		if (!UnitCanEnterTile (x, y)) {
			return;
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
				if (u == null || (n.cost < u.cost && (!n.myUnit || n == source))){
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
	public void FindReachableTiles() {
		Unit sUnit = selectedUnit.GetComponent<Unit> ();
		bool canDash = sUnit.actionPoints > 0;

		List<Node> reachableTiles = new List<Node> ();
		List<Node> reachableTilesDash = new List<Node> ();

		//Dictionary<Node, float> dist = new Dictionary<Node, float> ();

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

	//gets the target square and ggets the path already generated to it
	public void GetPath(int x, int y) {

		Unit sUnit = selectedUnit.GetComponent<Unit> ();

		//if the units path is already here they must want to move
		/*if (sUnit.currentPath != null) {
			if (sUnit.currentPath.Last ().x == x && sUnit.currentPath.Last ().y == y) {
				sUnit.moving = true;
				//set the node's unit
				graph[sUnit.tileY * currentLevel.maxSizeX + sUnit.tileX].myUnit = null;
				graph[y * currentLevel.maxSizeX + x].myUnit = sUnit;

				sUnit.RemoveMovement();

				return;
			}
		}*/


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

		CullPath ();
	}

	public void FollowPath() {
		Unit sUnit = selectedUnit.GetComponent<Unit> ();

		sUnit.moving = true;
		//set the node's unit
		graph[sUnit.tileY * currentLevel.maxSizeX + sUnit.tileX].myUnit = null;
		graph[sUnit.currentPath.Last().y * currentLevel.maxSizeX + sUnit.currentPath.Last().x].myUnit = sUnit;
		
		sUnit.RemoveMovement();
	}

	public void HighlightTiles(List<Node> HTiles, Color c)  {
		foreach (Node n in HTiles) {
			tileObjects[n.y * currentLevel.maxSizeX + n.x].GetComponent<ClickableTile>().HighlightTile(c);
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

}
