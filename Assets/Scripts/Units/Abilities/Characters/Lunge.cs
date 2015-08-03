using UnityEngine;
using System;
using System.Collections.Generic;


public class Lunge : Ability
{
	public Lunge (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 20;
		duration = 1;
		range = 3;
		area = AreaType.Line;
		targets = TargetType.Enemy;
		maxCooldown = 3;
	}

	public override void UseAbility (Node n)
	{
		List<Node> targetSquares = n.reachableNodes;
		targetSquares.Reverse ();
		base.UseAbility (n);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Dash", pos, false, false).GetComponent<EffectController> ());

		int count = 0;
		int x = targetSquares [0].x;
		int y = targetSquares [0].y;

		//we want to skip the first square as we can skip that one
		x -= (int)targetSquares [0].directionToParent.x;
		y -= (int)targetSquares [0].directionToParent.y;

		// loop until it finds an empty tile
		while (map.GetNode(x,y).myUnit != null) {

			//if it leaped an enemy add a combo point
			if (map.GetNode(x,y).myUnit.team != myCaster.team) {
				++count;
			}

			//increase xy for next loop
			x -= (int)targetSquares [0].directionToParent.x;
			y -= (int)targetSquares [0].directionToParent.y;
		}

		//if there was a target in either first or second tile, take damage
		if (targetSquares [0].myUnit != null) {
			if (targetSquares [0].myUnit.team != myCaster.team) {
				targetSquares [0].myUnit.TakeDamage (dmg);
				++count; // add another combo as we skiped this tile before
			}
		} else if (targetSquares [1].myUnit != null) {
			if (targetSquares [1].myUnit.team != myCaster.team) {
				targetSquares [1].myUnit.TakeDamage (dmg);
			}
		}


		//check to see if unit can enter tile
		if (map.UnitCanEnterTile (x, y)) {
			//unit can move to final tile
			myCaster.SlideToTile (x, y);
			myCaster.AddComboPoints(count);

			myCaster.ApplyEffect(new DamageDealtEffect("Lunge", 1, 0.5f)); 
		} else {
			//if there is no target in the first square
			if (targetSquares [0].myUnit == null) {
				myCaster.SlideToTile (targetSquares [0].x, targetSquares [0].y);
			}
		}

	}

	public override void UpdateAbility ()
	{
		if (!myCaster.moving) {
			AbilityFinished = true;
		}
	}
}


