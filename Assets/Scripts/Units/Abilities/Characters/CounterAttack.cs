using UnityEngine;
using System;

public class CounterAttack : Ability
{
	int blockIncrease = 75;
	public CounterAttack (Unit u, TileMap m, PrefabLibrary el)  : base(u, m , el)
	{
		Name = "Counter Attack";
		damage = 15;
		duration = 2;
		area = AreaType.Self;
		targets = TargetType.Enemy;
		maxCooldown = 6;
		description = "Cooldown: " + maxCooldown.ToString () +
			"\nIncreases block chance by " + blockIncrease.ToString() + "% for "  + duration.ToString() + " turns and deals " + damage.ToString() + " damage to attackers after blocking.";
	}

	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Counter Attack", pos).GetComponent<EffectController> ());

		myCaster.ApplyEffect(new BlockEffect("Counter Attack", duration, blockIncrease, 1));
		myCaster.AddTrigger (new DamageAttackerTrigger ("Counter Attack", myCaster, duration, effectLib, damage, TriggerType.Block));

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Counter Attack"));
	}

}


