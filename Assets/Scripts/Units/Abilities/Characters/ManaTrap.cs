using UnityEngine;
using System;

public class ManaTrap : Ability
{
	public ManaTrap (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		manaCost = 0;
		duration = 2;
		maxCooldown = 3;
		range = 6;
		area = AreaType.Floor;
		targets = TargetType.Enemy;
	}

	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		n.myTrigger = new ManaTrapTrigger ("Mana Trap", myCaster, effectLib, duration);
		n.myTrigger.myTargets = targets;
	}
}


