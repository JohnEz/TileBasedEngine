using UnityEngine;
using System.Collections.Generic;
using System;

public class Fireball : Ability
{
	int dotDamage = 10;

	public Fireball (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Fireball";
		damage = 30;
		manaCost = 15;
		duration = 3;
		maxCooldown = 2;
		range = 6;
		AOERange = 2;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		stacks = 4;

		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and applies a DOT that lasts " + duration.ToString() + " turns and deals " + dotDamage.ToString() + 
				" damage. Stacks " + stacks.ToString() + " times.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Fireball Cast"));
		myProjectiles.Add(effectLib.CreateProjectile("Fireball", myCaster.transform.position, target.transform.position, 10).GetComponent<ProjectileController>());
	}


	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();

		int dmg = (int)(damage * myCaster.damageDealtMod);

		//deal damage, if not dodged, apply effect
		if (myTarget.TakeDamage (dmg, effectLib.getSoundEffect ("Fireball Hit"), true, myCaster) != -1) {
			myTarget.ApplyEffect (new Dot ("Burning", duration, dotDamage, stacks));
			myTarget.ShowCombatText ("Burning", myTarget.statusCombatText);
		}

		Vector3 pos = map.TileCoordToWorldCoord (myTarget.tileX, myTarget.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Fireball Explosion", pos).GetComponent<EffectController> ());
	}
}


