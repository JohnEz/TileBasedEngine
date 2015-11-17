using UnityEngine;
using System;
public class Lacerate : Ability
{
	
	int bleedDmg = 10;
	public Lacerate (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Lacerate";
		damage = 30;
		duration = 3;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
		stacks = 5;

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and makes them bleed for " + bleedDmg.ToString() + " damage each turn for " + duration.ToString() + 
				" turns. Stacks " + stacks + " times.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		int dmg = (int)(damage * myCaster.damageDealtMod);

		//deal damage, if not dodged, apply effect
		if (target.TakeDamage (dmg, effectLib.getSoundEffect ("Lacerate"), true, myCaster) != -1) {
			target.ApplyEffect (new Dot ("Lacerate", duration, bleedDmg, stacks, effectLib.getIcon("Lacerate").sprite));
		}

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());
	}
}


