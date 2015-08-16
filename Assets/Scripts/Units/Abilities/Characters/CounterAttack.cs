using UnityEngine;
using System;

public class CounterAttack : Ability
{
	public CounterAttack (Unit u, TileMap m, PrefabLibrary el)  : base(u, m , el)
	{
		Name = "Counter Attack";
		damage = 15;
		duration = 2;
		area = AreaType.Self;
		targets = TargetType.Enemy;
		maxCooldown = 6;
	}

	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Counter Attack", pos).GetComponent<EffectController> ());

		myCaster.ApplyEffect(new BlockEffect("Counter Attack", duration, 100, 1));
		myCaster.AddTrigger (new CounterAttackTrigger (myCaster, duration, effectLib));

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Counter Attack"));
	}

}


