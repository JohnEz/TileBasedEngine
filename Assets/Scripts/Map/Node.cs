using UnityEngine;
using System.Collections;

public class Node {

	public Node[] neighbours;
	public Node previous;
	public int x;
	public int y;
	public Unit myUnit = null;
	public Vector2 directionToParent;
	public float cost = Mathf.Infinity;
	
	public Node() {
		neighbours = new Node[4];
	}
	
	public float distanceTo(Node n) {
		return Vector2.Distance (new Vector2(x, y), new Vector2(n.x, n.y));
	}
}
