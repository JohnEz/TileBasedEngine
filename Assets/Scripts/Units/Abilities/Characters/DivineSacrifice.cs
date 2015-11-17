using UnityEngine;
using System;

public class DivineSacrifice : Ability
{
	public DivineSacrifice (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Divine Sacrifice";
		damage = 10;
		manaGain = 4;
		duration = 4;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and each time the target takes damage, the caster gains " + manaGain.ToString() +
				" mana. Lasts " + duration.ToString() + ".";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		//deal damage, if not dodged, apply effect
		if (target.TakeDamage (dmg, effectLib.getSoundEffect ("Divine Sacrifice"), true, myCaster) != -1) {
			//apply an on death trigger
			target.AddTrigger (new DivineSacrificeTrigger ("Divine Sacrifice", myCaster, duration, effectLib, manaGain, effectLib.getIcon("Divine Sacrifice").sprite));
		}

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Divine Sacrifice", pos).GetComponent<EffectController> ());
	}
}


