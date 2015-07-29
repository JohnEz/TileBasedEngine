using UnityEngine;
using System;

public class PointBlank : Ability
{
	public PointBlank (Unit u, TileMap m, VisualEffectLibrary el) : base(u, m , el)
	{
		damage = 20;
		duration = 0;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		int dmg = (int)(damage * myCaster.damageDealtMod);
		int combo = myCaster.UseComboPoints ();

		int dirX = myCaster.tileX - target.tileX;
		int dirY = myCaster.tileY - target.tileY;

		int currX = myCaster.tileX + dirX;
		int currY = myCaster.tileY + dirY;

		int count = 0;

		while (map.UnitCanEnterTile(currX, currY) && count < combo) {
			currX += dirX;
			currY += dirY;
			++count;
		}

		currX -= dirX;
		currY -= dirY;

		if (currX != myCaster.tileX || currY != myCaster.tileY) {
			myCaster.SlideToTile (currX, currY);
		}

		target.TakeDamage (dmg * combo);
	}

	public override void UpdateAbility ()
	{
		if (!myCaster.moving) {
			AbilityFinished = true;
		}
	}
}


