using UnityEngine;
using System;
public class Lacerate : Ability
{
	public Lacerate (Unit u) : base(u)
	{
		damage = 40;
		duration = 2;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
		stacks = 3;
	}

	public override void UseAbility (Unit target, TileMap map)
	{
		base.UseAbility (target, map);

		int dmg = (int)(damage * myCaster.damageDealtMod);

		myCaster.AddComboPoints (2);
		target.TakeDamage(dmg);
		target.ApplyEffect(new DamageRecievedEffect("Lacerate", duration, 0.1f, stacks));
	}
}


