using UnityEngine;
using System;

public class Fireball : Ability
{
	public Fireball (Unit u) : base(u)
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

	public override void UseAbility (Unit target, TileMap map)
	{
		base.UseAbility (target, map);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		target.TakeDamage (dmg);
		target.ApplyEffect (new Dot ("Burning", duration, 10));

	}

	public override void UseAbility (System.Collections.Generic.List<Node> targetSquares, TileMap map)
	{
		base.UseAbility (targetSquares, map);

		foreach (Node n in targetSquares) {
			if (n.myUnit != null) {
				UseAbility(n.myUnit, map);
			}
		}
	}
}


