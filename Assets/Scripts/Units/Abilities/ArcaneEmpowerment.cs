using UnityEngine;
using System;

public class ArcaneEmpowerment : Ability
{
	public ArcaneEmpowerment (Unit u, TileMap m, VisualEffectLibrary el) : base(u, m , el)
	{
		manaCost = 15;
		maxCooldown = 3;
		duration = 2;
		range = 5;
		area = AreaType.Single;
		targets = TargetType.Ally;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		target.ApplyEffect(new DamageDealtEffect("Arcane Empowerment", duration, 0.30f));
	}
}


