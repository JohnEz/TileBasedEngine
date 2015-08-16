using UnityEngine;
using System;


public class CounterAttackTrigger : Trigger
{
	
	public CounterAttackTrigger (Unit caster, int dur, PrefabLibrary el) : base(TriggerType.Block, caster, el)
	{
		maxTriggers = 50;
		maxDuration = dur;
	}
	
	public override void RunTrigger (Unit host, Unit attacker = null)
	{
		base.RunTrigger (host, attacker);

		int dmg = (int)(20 * myCaster.damageDealtMod);

		attacker.TakeDamage (dmg, effectLib.getSoundEffect("Sword1"), false, myCaster);

		Vector3 pos = attacker.transform.position;
		effectLib.CreateVisualEffect ("Slash1", pos);
	}
}


