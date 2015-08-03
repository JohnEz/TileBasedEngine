using UnityEngine;
using System.Collections.Generic;
using System;

public class Fireball : Ability
{
	public Fireball (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 30;
		manaCost = 15;
		duration = 3;
		maxCooldown = 2;
		range = 6;
		AOERange = 2;
		area = AreaType.Single;
		targets = TargetType.All;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Fireball Cast"));
		myProjectiles.Add(effectLib.CreateProjectile("Fireball", myCaster.transform.position, target.transform.position, 8).GetComponent<ProjectileController>());
	}


	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();

		int dmg = (int)(damage * myCaster.damageDealtMod);

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Fireball Hit"));

		myTarget.TakeDamage (dmg);
		myTarget.ApplyEffect (new Dot ("Burning", duration, 10));

		Vector3 pos = map.TileCoordToWorldCoord (myTarget.tileX, myTarget.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Fireball Explosion", pos).GetComponent<EffectController> ());
	}
}


