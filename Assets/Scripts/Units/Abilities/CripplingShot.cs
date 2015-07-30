using UnityEngine;
using System;

public class CripplingShot : Ability
{
	public CripplingShot (Unit u, TileMap m, VisualEffectLibrary el) : base(u, m , el)
	{
		damage = 10;
		manaCost = 20;
		duration = 2;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		range = 5;
		maxCooldown = 1;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		int dmg = (int)(damage * myCaster.damageDealtMod);

		target.ApplyEffect (new Snare ("Crippled", duration));
		target.TakeDamage (dmg);

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateEffect ("Hit1", pos).GetComponent<EffectController> ());
	}
}


