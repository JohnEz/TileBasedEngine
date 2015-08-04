using UnityEngine;
using System;

public class DivineSacrifice : Ability
{
	public DivineSacrifice (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Divine Sacrifice";
		damage = 10;
		duration = 4;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		//deal damage, if not dodged, apply effect
		if (target.TakeDamage (dmg, effectLib.getSoundEffect ("Divine Sacrifice")) != -1) {
			//apply an on death trigger
			target.AddTrigger (new DivineSacrificeTrigger (myCaster));
		}

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Divine Sacrifice", pos).GetComponent<EffectController> ());
	}
}


