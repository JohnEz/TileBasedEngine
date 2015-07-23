using UnityEngine;
using System;

public class ShieldSlam : Ability
{
	public ShieldSlam(Unit u)
		: base(u)
	{
		damage = 20;
		duration = 1;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 2;
	}

	public override void UseAbility (Unit t, TileMap map)
	{
		base.UseAbility (t, map);

		if (t.mySize <= UnitSize.Normal) {
			int dmg = (int)(damage * myCaster.damageDealtMod);
			int diffX = t.tileX - myCaster.tileX;
			int diffY = t.tileY - myCaster.tileY;

			if (map.UnitCanEnterTile(myCaster.tileX+(diffX*2), myCaster.tileY+(diffY*2))) {
				t.SlideToTile(myCaster.tileX+(diffX*2), myCaster.tileY+(diffY*2));
			}

			t.TakeDamage(dmg);
			t.ApplyEffect(new Stun("Shield Slammed", duration));
		} else {
			int dmg = (int)(damage * myCaster.damageDealtMod);

			t.TakeDamage(dmg*3);
		}
	}

	public override void UpdateAbility ()
	{
		if (!myTarget.moving) {
			AbilityFinished = true;
		}
	}


}

