using UnityEngine;
using System;

public class ArcaneEmpowerment : Ability
{
	public ArcaneEmpowerment (Unit u) : base(u)
	{
		manaCost = 15;
		maxCooldown = 3;
		duration = 2;
		range = 5;
		area = AreaType.Single;
		targets = TargetType.Ally;
	}

	public override void UseAbility (Unit target, TileMap map)
	{
		base.UseAbility (target, map);

		target.ApplyEffect(new DamageDealtEffect("Arcane Empowerment", duration, 0.30f));
	}
}


