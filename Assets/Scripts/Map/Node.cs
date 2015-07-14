using UnityEngine;
using System.Collections;

public class Node {

	public Node[] neighbours;
	public Node previous;
	public int x;
	public int y;
	public bool occupied = false;
	
	public Node() {
		neighbours = new Node[4];
	}
	
	public float distanceTo(Node n) {
		return Vector2.Distance (new Vector2(x, y), new Vector2(n.x, n.y));
	}
}
