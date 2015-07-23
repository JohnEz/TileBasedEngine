using UnityEngine;
using System;

public class RighteousShield : Ability
{

	public RighteousShield (Unit u) : base (u)
	{
		shield = 50;
		manaCost = 20;
		maxCooldown = 3;
		duration = 1;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Ally;
	}

	public override void UseAbility (Unit target, TileMap map)
	{
		base.UseAbility (target, map);
		target.shield += shield;

		target.ApplyEffect (new MovespeedMod ("Righteous Shield", duration, 1));
	}
}


