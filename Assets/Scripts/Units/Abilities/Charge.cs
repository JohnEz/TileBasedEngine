using UnityEngine;
using System.Collections.Generic;
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
		base.UseAbility (targetSquares, map);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		int count = 0;

		while (count < targetSquares.Count && targetSquares[count].myUnit == null) {
			count++;
		}

		//if it got to the end
		if (count == targetSquares.Count) {
			myCaster.SlideToTile (targetSquares [count - 1].x, targetSquares [count - 1].y);
		} 
		//if it hit a target
		else if (targetSquares[count].myUnit != null) {
			if (count != 0) {
				myCaster.SlideToTile (targetSquares [count - 1].x, targetSquares [count - 1].y);
			}
			targetSquares[count].myUnit.TakeDamage(dmg);
			targetSquares[count].myUnit.ApplyEffect(new Stun("Charged", duration));
		}
	}

	public override void UpdateAbility ()
	{
		if (!myCaster.moving) {
			AbilityFinished = true;
		}
	}
}


