using UnityEngine;
using System;

public class ManaTrap : Ability
{
	public ManaTrap (Unit u, TileMap m, VisualEffectLibrary el) : base(u, m , el)
	{
		manaCost = 5;
		duration = 2;
		maxCooldown = 2;
		range = 6;
		area = AreaType.Floor;
		targets = TargetType.All;
	}

	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		n.myTrigger = new ManaTrapTrigger (myCaster);
	}
}


