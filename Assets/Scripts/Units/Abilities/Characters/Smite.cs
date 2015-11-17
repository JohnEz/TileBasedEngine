using UnityEngine;
using System.Collections.Generic;

public class Smite : Ability
{
	float healingMod = 0.33f;
	public Smite(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Smite";
		damage = 20;
		duration = 3;
		range = 5;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
		stacks = 3;
		
		/*description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and reduces their damage dealt by " + percentageMod.ToString() + "% for " + duration.ToString() + 
				" turns. Stacks " + stacks + " times.";*/
		int percent = (int)(healingMod * 100);

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and increases your next heal by " + percent + "%. Stacks " + stacks.ToString() + " times lasting " + 
				duration.ToString() + " turns.";
	}
	
	public override void UseAbility(Unit target)
	{
		base.UseAbility(target);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		HealingDealtEffect healEffect = new HealingDealtEffect ("Smite", duration, healingMod, stacks, effectLib.getIcon("Smite").sprite);

		//deal damage, if not dodged, apply effect
		target.TakeDamage (dmg, effectLib.getSoundEffect ("Crippling Strike"), true, myCaster);
		myCaster.ApplyEffect (healEffect);
		myCaster.AddTrigger(new RemoveEffect ("smite remove", myCaster, TriggerType.Heal, healEffect, effectLib, duration, effectLib.getIcon("Smite").sprite));

		
		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());
	}
	
	
}
