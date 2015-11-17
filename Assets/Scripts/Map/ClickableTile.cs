using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public enum TileVisability {
	Undiscovered,
	Discovered,
	Visable
}

public class ClickableTile : MonoBehaviour, IPointerClickHandler {

	//location info
	public int tileX;
	public int tileY;
	public TileMap map;
	public UnitManager uManager;

	//highlighting
	public bool highlighted = false;
	public bool targetable = false;
	public bool flipped = false;
	public Color storedColour = Color.white;
	public Color mOverColour;

	//visability
	public List<Unit> visableTo = new List<Unit> ();
	public TileVisability myVisabilityState = TileVisability.Undiscovered;
	public Color undiscoveredColorMod = new Color(0.2f, 0.2f, 0.2f, 0);
	public Color discoveredColorMod = new Color(0.1f, 0.1f, 0.1f, 0);
	public Color visableColorMod = new Color(0, 0, 0, 0.5f);


	void Start() {
		SetColour (new Color (0.5f, 0.5f, 0.5f));
	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left) {
			if (targetable) {
				uManager.TileClicked(tileX, tileY);
			}
		}
	}
	#endregion

	void OnMouseEnter() {
		if (highlighted) {
			SetColour(mOverColour);
			uManager.TileEnter(tileX, tileY);
		}
	}

	void OnMouseExit() {
		if (highlighted) {
			SetColour(storedColour);
			uManager.TileExit(tileX, tileY);
		}
	}

	public void HighlightTile(Color colour, Color hover, int trgtAble)
	{
		highlighted = true;
		SetColour(colour);
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
		flipped = false;
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
			SetColour(new Color(0.5f, 0.5f, 0.5f));
			storedColour = Color.white;
			mOverColour = Color.white;
			targetable = false;
			flipped = false;
		}
	}

	public void SwitchColours() {
		SetColour(mOverColour);
		flipped = SwitchBool (flipped);
		Color store = mOverColour;
		mOverColour = storedColour;
		storedColour = store;
	}

	//sets the color of the tile
	public void SetColour(Color col) {
		Color mod = new Color (1, 1, 1, 1);

		// get the correct colour mod
		switch (myVisabilityState) {
		case TileVisability.Discovered : mod = discoveredColorMod;
			break;
		case TileVisability.Undiscovered: mod = undiscoveredColorMod;
			break;
		case TileVisability.Visable: mod = visableColorMod;
			break;
		default: mod = visableColorMod;
			break;
		}

		//set the color minus the mod
		GetComponent<Renderer> ().material.color = col - mod;
	}
	
	public void UpdateVision() {
		//if its visiable to atleast 1 unit
		if (visableTo.Count > 0) {
			GiveVision ();
		} else {
			RemoveVision ();
		}

	}

	//gives the tile vision
	public void GiveVision() {
		myVisabilityState = TileVisability.Visable;
		SetColour (new Color (0.5f, 0.5f, 0.5f));
		if (map.GetNode (tileX, tileY).myUnit != null) {
			map.GetNode (tileX, tileY).myUnit.ShowUnit();
		}
	}

	public void RemoveVision() {
		//remove vision
		switch (myVisabilityState) {
		case TileVisability.Discovered : break;
		case TileVisability.Undiscovered: break;
		case TileVisability.Visable: myVisabilityState = TileVisability.Discovered;
			break;

		}
		SetColour (new Color (0.5f, 0.5f, 0.5f));

		if (map.GetNode (tileX, tileY).myUnit != null) {
			map.GetNode (tileX, tileY).myUnit.HideUnit();
		}
	}
}
