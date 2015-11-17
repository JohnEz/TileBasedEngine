using UnityEngine;
using System;
public class WordOfHealing : Ability
{
	float damageMod = 0.05f;
	public WordOfHealing (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Word Of Healing";
		healing = 40;
		manaCost = 10;
		maxCooldown = 1;
		duration = 2;
		range = 4;
		area = AreaType.Single;
		targets = TargetType.Ally;
		stacks = 3;

		int percentageMod = (int)(damageMod * 100);

		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nHeals the target for " + healing.ToString () + " health and reduces the damage the target takes by " + percentageMod.ToString() +
				"% for " + duration.ToString() + " turns. Stacks " + stacks.ToString() + " times.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		target.TakeHealing ((int)(healing * myCaster.healingDealtMod), myCaster);
		target.ApplyEffect (new DamageRecievedEffect ("Word of Healing", duration, -damageMod, stacks, effectLib.getIcon("Word of Healing").sprite));

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Word of Healing", pos).GetComponent<EffectController> ());

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Word of Healing"));
	}
}



