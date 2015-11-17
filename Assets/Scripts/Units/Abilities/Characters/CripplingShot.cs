using UnityEngine;
using System.Collections.Generic;
using System;

public class CripplingShot : Ability
{
	public CripplingShot (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Crippling Shot";
		damage = 20;
		manaCost = 20;
		duration = 2;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		range = 5;
		maxCooldown = 1;
		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and applies a snare for " + duration.ToString() + " turns.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		myTarget = target;

		myProjectiles.Add(effectLib.CreateProjectile("Crippling Shot", myCaster.transform.position, target.transform.position, 10).GetComponent<ProjectileController>());
		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Exploit Weakness Fire"));
	}

	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();
		Vector3 pos = map.TileCoordToWorldCoord (myTarget.tileX, myTarget.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Hit1", pos).GetComponent<EffectController> ());
		
		int dmg = (int)(damage * myCaster.damageDealtMod);

		// deal damage, if not dodged apply cripple
		if (myTarget.TakeDamage (dmg, effectLib.getSoundEffect ("Crippling Shot Hit"), true, myCaster) != -1) {
			myTarget.ApplyEffect (new Snare ("Crippled", duration, effectLib.getIcon("Crippling Shot").sprite));
			myTarget.ShowCombatText ("Snared", myTarget.statusCombatText);
		}

	}
}


