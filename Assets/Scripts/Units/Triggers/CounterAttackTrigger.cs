using UnityEngine;
using System;


public class CounterAttackTrigger : Trigger
{
	int damage = 0;
	public CounterAttackTrigger (string name, Unit caster, int dur, PrefabLibrary el, int dmg) : base(name, TriggerType.Block, caster, el, dur)
	{
		maxTriggers = -1;
		damage = dmg;
	}
	
	public override void RunTrigger (Unit host, Unit attacker = null)
	{
		base.RunTrigger (host, attacker);

		int dmg = (int)(damage * myCaster.damageDealtMod);

		attacker.TakeDamage (dmg, effectLib.getSoundEffect("Sword1"), false, myCaster);

		Vector3 pos = attacker.transform.position;
		effectLib.CreateVisualEffect ("Slash1", pos);
	}
}


