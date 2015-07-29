using UnityEngine;
using System;
using System.Collections.Generic;


public class Lunge : Ability
{
	public Lunge (Unit u, TileMap m, VisualEffectLibrary el) : base(u, m , el)
	{
		damage = 35;
		duration = 1;
		range = 3;
		area = AreaType.Line;
		targets = TargetType.Enemy;
		maxCooldown = 2;
	}

	public override void UseAbility (Node n)
	{
		List<Node> targetSquares = n.reachableNodes;
		targetSquares.Reverse ();
		base.UseAbility (n);
		int dmg = (int)(damage * myCaster.damageDealtMod);
		

		if (targetSquares [0].myUnit == null) {
			myCaster.SlideToTile (targetSquares [0].x, targetSquares [0].y);
			if (targetSquares.Count > 1) {
				if (targetSquares [1].myUnit != null) {
					targetSquares [1].myUnit.TakeDamage (dmg);
					myCaster.AddComboPoints (1);
				}
			}
		} else {
			targetSquares[0].myUnit.TakeDamage(dmg);
			myCaster.AddComboPoints(1);
		}

	}

	public override void UpdateAbility ()
	{
		if (!myCaster.moving) {
			AbilityFinished = true;
		}
	}
}


