using UnityEngine;
using System;

public class ManaTrap : Ability
{
	public ManaTrap (Unit caster) : base(caster)
	{
		manaCost = 5;
		duration = 2;
		maxCooldown = 2;
		range = 6;
		area = AreaType.Floor;
		targets = TargetType.All;
	}

	public override void UseAbility (Node n, TileMap map)
	{
		base.UseAbility (n, map);

		n.myTrigger = new ManaTrapTrigger (myCaster);
	}
}


