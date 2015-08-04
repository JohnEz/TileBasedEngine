using UnityEngine;
using System;
public class Lacerate : Ability
{
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
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		int dmg = (int)(damage * myCaster.damageDealtMod);

		myCaster.AddComboPoints (2);
		//deal damage, if not dodged, apply effect
		if (target.TakeDamage (dmg, effectLib.getSoundEffect ("Lacerate")) != -1) {
			target.ApplyEffect (new DamageRecievedEffect ("Lacerate", duration, 0.1f, stacks));
		}

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());
	}
}


