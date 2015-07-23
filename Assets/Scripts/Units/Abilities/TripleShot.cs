using UnityEngine;
using System;

public class TripleShot : Ability
{
	public TripleShot (Unit u) : base(u)
	{
		damage = 10;
		range = 5;
		maxCooldown = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
	}

	public override void UseAbility (Unit target, TileMap map)
	{
		base.UseAbility (target, map);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		myCaster.GainMana (15);

		target.TakeDamage (dmg);
		target.TakeDamage (dmg);
		target.TakeDamage (dmg);
	}
}


