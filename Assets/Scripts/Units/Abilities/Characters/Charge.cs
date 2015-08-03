using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Charge : Ability
{
	public Charge (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 10;
		duration = 2;
		range = 5;
		area = AreaType.Line;
		targets = TargetType.Enemy;
		maxCooldown = 3;
	}

	public override void UseAbility (Node n)
	{
		//get the nodes in the path
		List<Node> targetSquares = n.reachableNodes;
		targetSquares.Reverse ();
		base.UseAbility (n);

		//caluclate the damage
		int dmg = (int)(damage * myCaster.damageDealtMod);

		int count = 0;

		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Dash", pos, false, false).GetComponent<EffectController> ());

		//check to see if the final tile has a unit
		if (targetSquares.Last ().myUnit != null) {
			// find the index of the tile before the last one
			count = targetSquares.Count - 2;

			//if the unit is on another team
			if (targetSquares.Last ().myUnit.team != myCaster.team) {
				//make the unit take damage
				targetSquares.Last ().myUnit.TakeDamage(dmg * targetSquares.Count+1);
				targetSquares.Last ().myUnit.ApplyEffect(new Snare("Charged", duration));
			}

			// if there is a tile to move to
			if (targetSquares.Count > 1) {
				myCaster.SlideToTile (targetSquares [count].x, targetSquares [count].y);
			}
		} else {
			//else slide to the last tile
			myCaster.SlideToTile (targetSquares [targetSquares.Count-1].x, targetSquares [targetSquares.Count-1].y);
		}

	}

	public override void UpdateAbility ()
	{
		if (!myCaster.moving) {
			AbilityFinished = true;
		}
	}
}


