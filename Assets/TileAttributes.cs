using UnityEngine;
using System.Collections;

public class TileAttributes : MonoBehaviour {

    public string name;
    public bool isWalkable = true;
    public bool blocksVision = false;
    public float movementCost = 1;
    public float LosCost = 1;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
