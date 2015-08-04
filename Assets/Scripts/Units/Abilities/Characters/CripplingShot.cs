using UnityEngine;
using System.Collections.Generic;
using System;

public class CripplingShot : Ability
{
	public CripplingShot (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Crippling Shot";
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

		myTarget = target;

		myProjectiles.Add(effectLib.CreateProjectile("Crippling Shot", myCaster.transform.position, target.transform.position, 10).GetComponent<ProjectileController>());

	}

	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();
		Vector3 pos = map.TileCoordToWorldCoord (myTarget.tileX, myTarget.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Hit1", pos).GetComponent<EffectController> ());
		
		int dmg = (int)(damage * myCaster.damageDealtMod);
		
		myTarget.ApplyEffect (new Snare ("Crippled", duration));
		myTarget.TakeDamage (dmg);
	}
}


