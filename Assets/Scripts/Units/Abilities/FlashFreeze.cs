using UnityEngine;
using System;
using System.Collections.Generic;

public class FlashFreeze : Ability
{
	public FlashFreeze (Unit u) : base(u)
	{
		damage = 20;
		manaCost = 20;
		duration = 1;
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
		target.ApplyEffect (new Stun ("Frozen", duration));
		
	}
	
	public override void UseAbility (List<Node> targetSquares, TileMap map)
	{
		base.UseAbility (targetSquares, map);
		
		foreach (Node n in targetSquares) {
			if (n.myUnit != null) {
				UseAbility(n.myUnit, map);
			}
		}
	}
}


