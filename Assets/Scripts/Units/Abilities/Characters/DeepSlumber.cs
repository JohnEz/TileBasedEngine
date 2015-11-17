using UnityEngine;
using System;

public class DeepSlumber : Ability
{
	float damageMod = 0.25f;

	public DeepSlumber (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Deep Slumber";
		manaCost = 20;
		damage = 0;
		duration = 4;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 3;

		int percentageMod = (int)(damageMod * 100);

		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nPuts the target to sleep for " + duration.ToString() + " turns. The damage of the next attack against the unit is increased by " + percentageMod.ToString() + 
				"% however this awakens the unit.";
	}
	
	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		Effect eff1 = new Sleep ("Deep Slumber (sleep)", duration, effectLib.getIcon("Deep Slumber").sprite);
		Effect eff2 = new DamageRecievedEffect ("Deep Slumber (dmg)", duration, damageMod, 1, effectLib.getIcon("Deep Slumber").sprite); 

		target.ApplyEffect(eff1);
		target.ApplyEffect(eff2);

		target.AddTrigger (new RemoveEffect ("Deep Slumber (sleep)", myCaster, TriggerType.Hit, eff1, effectLib, duration, effectLib.getIcon("Deep Slumber").sprite));
		target.AddTrigger (new RemoveEffect ("Deep Slumber (dmg)", myCaster, TriggerType.Hit, eff2, effectLib, duration, effectLib.getIcon("Deep Slumber").sprite));

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Deep Slumber"));
	}
}


