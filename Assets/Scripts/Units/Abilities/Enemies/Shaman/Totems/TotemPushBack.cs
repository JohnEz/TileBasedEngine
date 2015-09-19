using UnityEngine;
using System;

public class TotemPushBack : Ability
{	
	
	public TotemPushBack (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		range = 6;
		area = AreaType.Self;
		targets = TargetType.Ally;
		maxCooldown = 4;
		shield = 50;
		
		AIPriority = 100;
		AIRanged = true;
		AISupportsAlly = true;
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		
		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Drum1", pos).GetComponent<EffectController> ());
		//myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Drum Double"));
		
		foreach (Node n in target.reachableNodes) {
			Unit sUnit = n.myUnit;
			
			//if the unit is in combat
			if (sUnit.isActive) {
				PushBack(n);
			}
		}
	}

	void PushBack(Node loc) {

		myVisualEffects.Add (effectLib.CreateVisualEffect ("Dash", map.TileCoordToWorldCoord(loc.x, loc.y), false, false).GetComponent<EffectController> ());
		//loop through all the neighbours
		foreach (Node n in loc.neighbours) {
			//if it contains a unit try to push them back
			if (n.myUnit != null && n.myUnit.team != myCaster.team) {
				//get the direction to push
				int dirX = n.x - loc.x;
				int dirY = n.y - loc.y;

				//starting tile
				int currX = n.x + dirX;
				int currY = n.y + dirY;

				int count = 0;
				//loop until we find a tile that cant be entered or if its 5 tiles
				while (map.UnitCanEnterTile(currX, currY) && count < 5) {
					currX += dirX;
					currY += dirY;
					++count;
				}

				currX -= dirX;
				currY -= dirY;
				//move the unit to that tile
				if (currX != n.x || currY != n.y) {
					n.myUnit.SlideToTile (currX, currY);
				}
			}
		}
	}
}


