using UnityEngine;
using System.Collections;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileY;
	public TileMap map;
	public UnitManager uManager;

	public bool highlighted = false;
	public bool targetable = false;
	Color storedColour = Color.white;
	Color mOverColour;

	void OnMouseUp() {
		if (highlighted) {
			uManager.TileClicked(tileX, tileY);
		}
	}

	void OnMouseOver() {
		if (highlighted) {
			GetComponent<Renderer> ().material.color = mOverColour;
			uManager.TileHover(tileX, tileY);
		}
	}

	void OnMouseExit() {
		if (highlighted) {
			GetComponent<Renderer> ().material.color = storedColour;
		}
	}

	public void HighlightTile(Color c, Color m)
	{
		highlighted = true;
		GetComponent<Renderer> ().material.color = c;
		storedColour = c;
		mOverColour = m;
	}

	public void UnhighlightTile()
	{
		if (highlighted) {
			highlighted = false;
			GetComponent<Renderer> ().material.color = Color.white;
			storedColour = Color.white;
			targetable = false;
		}
	}
}
