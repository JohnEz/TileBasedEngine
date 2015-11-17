using UnityEngine;
using System.Collections.Generic;
using System;

public class TotemFireball : Ability
{
	int dotDamage = 5;
	
	public TotemFireball (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Fireball";
		damage = 10;
		manaCost = 0;
		duration = 3;
		maxCooldown = 1;
		range = 8;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		stacks = 9;

		AIRanged = true;
		
		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and applies a DOT that lasts " + duration.ToString() + " turns and deals " + dotDamage.ToString() + 
				" damage. Stacks " + stacks.ToString() + " times.";
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);

		myTarget = target.myUnit;

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Fireball Cast"));
		myProjectiles.Add(effectLib.CreateProjectile("Totem Fireball", myCaster.transform.position, target.myUnit.transform.position, 10).GetComponent<ProjectileController>());
	}
	
	
	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();
		
		int dmg = (int)(damage * myCaster.damageDealtMod);
		
		//deal damage, if not dodged, apply effect
		if (myTarget.TakeDamage (dmg, effectLib.getSoundEffect ("Fireball Hit"), true, myCaster) != -1) {
			myTarget.ApplyEffect (new Dot ("Potent Fire", duration, dotDamage, stacks));
			myTarget.ShowCombatText ("Burning", myTarget.statusCombatText);
		}
		
		Vector3 pos = map.TileCoordToWorldCoord (myTarget.tileX, myTarget.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Fireball Explosion", pos).GetComponent<EffectController> ());
	}
}


