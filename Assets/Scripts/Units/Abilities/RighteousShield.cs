using UnityEngine;
using System;

public class RighteousShield : Ability
{

	public RighteousShield (Unit u, TileMap m, VisualEffectLibrary el) : base(u, m , el)
	{
		shield = 50;
		manaCost = 20;
		maxCooldown = 3;
		duration = 1;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Ally;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		target.shield += shield;

		target.ApplyEffect (new MovespeedMod ("Righteous Shield", duration, 1));

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateEffect ("Righteous Shield", pos).GetComponent<EffectController> ());
	}
}


