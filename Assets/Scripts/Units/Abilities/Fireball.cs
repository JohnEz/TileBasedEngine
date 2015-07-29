using UnityEngine;
using System.Collections.Generic;
using System;

public class Fireball : Ability
{
	public Fireball (Unit u, TileMap m, VisualEffectLibrary el) : base(u, m , el)
	{
		damage = 35;
		manaCost = 20;
		duration = 2;
		maxCooldown = 3;
		range = 5;
		AOERange = 2;
		area = AreaType.AOE;
		targets = TargetType.All;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		target.TakeDamage (dmg);
		target.ApplyEffect (new Dot ("Burning", duration, 10));

	}

	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		foreach (Node no in n.reachableNodes) {
			if (no.myUnit != null) {
				UseAbility(no.myUnit);
			}
		}
	}
}


