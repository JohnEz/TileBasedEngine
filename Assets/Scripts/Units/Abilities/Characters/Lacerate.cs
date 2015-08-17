using UnityEngine;
using System;
public class Lacerate : Ability
{

	float damageMod = 0.1f;
	public Lacerate (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Lacerate";
		damage = 40;
		duration = 2;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
		stacks = 3;

		int percentageMod = (int)(damageMod * 100);

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and increases the damage they take by " + percentageMod.ToString() + "% for " + duration.ToString() + 
				" turns. Stacks " + stacks + " times.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		int dmg = (int)(damage * myCaster.damageDealtMod);

		myCaster.AddComboPoints (2);
		//deal damage, if not dodged, apply effect
		if (target.TakeDamage (dmg, effectLib.getSoundEffect ("Lacerate"), true, myCaster) != -1) {
			target.ApplyEffect (new DamageRecievedEffect ("Lacerate", duration, damageMod, stacks));
		}

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());
	}
}


