using UnityEngine;
using System.Collections;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileY;
	public TileMap map;	

	bool highlighted = false;

	void OnMouseUp() {
		if (highlighted) {
			map.GetPath(tileX, tileY);
		}
	}

	public void HighlightTile(Color c)
	{
		if (!highlighted) {
			highlighted = true;
			GetComponent<Renderer> ().material.color = c;
		}
	}

	public void UnhighlightTile()
	{
		if (highlighted) {
			highlighted = false;
			GetComponent<Renderer> ().material.color = Color.white;
		}
	}
}
