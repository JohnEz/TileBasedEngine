using UnityEngine;
using System.Collections;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileY;
	public TileMap map;	

	bool highlighted = false;
	Color storedColour = Color.white;

	void OnMouseUp() {
		if (highlighted) {
			map.GetPath(tileX, tileY);
		}
	}

	void OnMouseEnter() {
		if (highlighted) {
			GetComponent<Renderer> ().material.color = Color.magenta;
		}
	}

	void OnMouseExit() {
		if (highlighted) {
			GetComponent<Renderer> ().material.color = storedColour;
		}
	}

	public void HighlightTile(Color c)
	{
		if (!highlighted) {
			highlighted = true;
			GetComponent<Renderer> ().material.color = c;
			storedColour = c;
		}
	}

	public void UnhighlightTile()
	{
		if (highlighted) {
			highlighted = false;
			GetComponent<Renderer> ().material.color = Color.white;
			storedColour = Color.white;
		}
	}
}
