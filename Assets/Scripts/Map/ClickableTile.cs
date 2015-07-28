using UnityEngine;
using System.Collections;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileY;
	public TileMap map;
	public UnitManager uManager;

	public bool highlighted = false;
	public bool targetable = false;
	public Color storedColour = Color.white;
	public Color mOverColour;

	void OnMouseUp() {
		if (targetable) {
			uManager.TileClicked(tileX, tileY);
		}
	}

	void OnMouseEnter() {
		if (highlighted) {
			GetComponent<Renderer> ().material.color = mOverColour;
			uManager.TileEnter(tileX, tileY);
		}
	}

	void OnMouseExit() {
		if (highlighted) {
			GetComponent<Renderer> ().material.color = storedColour;
			uManager.TileExit(tileX, tileY);
		}
	}

	public void HighlightTile(Color colour, Color hover, int trgtAble)
	{
		highlighted = true;
		GetComponent<Renderer> ().material.color = colour;
		storedColour = colour;
		mOverColour = hover;
		switch (trgtAble) {
		case 0: targetable = false;
			break;
		case 1: targetable = true;
			break;
		case -1: targetable = SwitchBool(targetable);
			break;
		}
	}

	bool SwitchBool (bool b) {
		if (b) {
			return false;
		}
		return true;
	}

	public void UnhighlightTile()
	{
		if (highlighted) {
			highlighted = false;
			GetComponent<Renderer> ().material.color = new Color(0.5f, 0.5f, 0.5f);
			storedColour = Color.white;
			targetable = false;
		}
	}
}
