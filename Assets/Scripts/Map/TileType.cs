using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType {

	public string name;
	public GameObject tileVisualPrefab;

	public bool isWalkable = true;
	public bool blocksVision = false;
	public float movementCost = 1;
	public float LosCost = 1;

}
