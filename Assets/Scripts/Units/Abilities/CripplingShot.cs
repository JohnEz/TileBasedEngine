using UnityEngine;
using System;

public class CripplingShot : Ability
{
	public CripplingShot (Unit u) : base(u)
	{
		damage = 10;
		manaCost = 20;
		duration = 2;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		range = 5;
		maxCooldown = 1;
	}

	public override void UseAbility (Unit target, TileMap map)
	{
		base.UseAbility (target, map);

		int dmg = (int)(damage * myCaster.damageDealtMod);

		target.ApplyEffect (new Snare ("Crippled", duration));
		target.TakeDamage (dmg);

	}
}


