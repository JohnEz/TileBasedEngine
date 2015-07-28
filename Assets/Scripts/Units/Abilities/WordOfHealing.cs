using UnityEngine;
using System;
public class WordOfHealing : Ability
{
	public WordOfHealing (Unit u) : base (u)
	{
		healing = 30;
		manaCost = 10;
		maxCooldown = 1;
		duration = 2;
		range = 4;
		area = AreaType.Single;
		targets = TargetType.Ally;
		stacks = 3;

	}

	public override void UseAbility (Unit target, TileMap map)
	{
		base.UseAbility (target, map);

		target.TakeHealing (healing);
		target.ApplyEffect (new DamageRecievedEffect ("Word of Healing", duration, -0.05f, stacks));
	}
}



