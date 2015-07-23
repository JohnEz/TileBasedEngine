using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Charge : Ability
{
	public Charge (Unit u) 
		: base (u) 
	{
		damage = 20;
		duration = 1;
		range = 5;
		area = AreaType.Line;
		targets = TargetType.Enemy;
		maxCooldown = 3;
	}

	public override void UseAbility (List<Node> targetSquares, TileMap map)
	{
		targetSquares.Reverse ();
		base.UseAbility (targetSquares, map);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		int count = 0;

		if (targetSquares.Last ().myUnit != null) {
			count = targetSquares.Count - 2;
			targetSquares.Last ().myUnit.TakeDamage(dmg);
			targetSquares.Last ().myUnit.ApplyEffect(new Stun("Charged", duration));
		} else {
			count = targetSquares.Count - 1;
		}

		if (targetSquares.Count > 1) {
			myCaster.SlideToTile (targetSquares [count].x, targetSquares [count].y);
		}

	}

	public override void UpdateAbility ()
	{
		if (!myCaster.moving) {
			AbilityFinished = true;
		}
	}
}


