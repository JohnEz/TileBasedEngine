using UnityEngine;
using System.Collections.Generic;

public class Node {

	public Node[] neighbours;
	public Node previous;
	public int x;
	public int y;
	public Unit myUnit = null;
	public Trigger myTrigger = null;
	public Vector2 directionToParent;
	public float cost = Mathf.Infinity;
	public float dist = 0;
	public List<Node> reachableNodes = null;
	
	public Node() {
		neighbours = new Node[4];
	}
	
	public float distanceTo(Node n) {
		return Vector2.Distance (new Vector2(x, y), new Vector2(n.x, n.y));
	}
}
