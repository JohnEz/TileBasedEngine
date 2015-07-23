using UnityEngine;
using System;

public class DivineSacrifice : Ability
{
	public DivineSacrifice (Unit u) : base(u)
	{
		damage = 10;
		duration = 4;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
	}

	public override void UseAbility (Unit target, TileMap map)
	{
		base.UseAbility (target, map);
		int dmg = (int)(damage * myCaster.damageDealtMod);
		
		target.TakeDamage(dmg);
		//apply an on death trigger
	}
}


