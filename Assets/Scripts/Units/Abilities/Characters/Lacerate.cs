using UnityEngine;
using System;
public class Lacerate : Ability
{
	public Lacerate (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
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
		target.TakeDamage(dmg);
		target.ApplyEffect(new DamageRecievedEffect("Lacerate", duration, 0.1f, stacks));

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Lacerate"));
	}
}


