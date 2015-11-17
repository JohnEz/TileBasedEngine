using UnityEngine;
using System.Collections.Generic;
using System;

public class ArcaneSpark : Ability
{
	int dotDamage = 10;
	
	public ArcaneSpark (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Arcane Spark";
		damage = 120;
		manaCost = 0;
		maxCooldown = 2;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		
		description = "Cooldown: " + maxCooldown.ToString () + " Mana: 100%" +
			"\nDeals " + damage.ToString () + " damage to the target multiplied by your remaining mana percentage. Cannot be dodged or blocked.";
	}
	
	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		
		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Fireball Cast"));
		myProjectiles.Add(effectLib.CreateProjectile("Fireball", myCaster.transform.position, target.transform.position, 20).GetComponent<ProjectileController>());

	}
	
	
	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();

		float percentage = (float)myCaster.mana / (float)myCaster.maxMana;

		int dmg = (int)((damage * myCaster.damageDealtMod) * percentage);
		
		//deal damage, if not dodged, apply effect
		myTarget.TakeDamage (dmg, effectLib.getSoundEffect ("Fireball Hit"), false, myCaster);
		
		Vector3 pos = map.TileCoordToWorldCoord (myTarget.tileX, myTarget.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Fireball Explosion", pos).GetComponent<EffectController> ());

		myCaster.AddRemoveMana (-myCaster.mana);
	}
}


