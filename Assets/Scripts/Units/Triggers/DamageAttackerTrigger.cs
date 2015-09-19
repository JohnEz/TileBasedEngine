using UnityEngine;
using System;


public class DamageAttackerTrigger : Trigger
{
	int damage = 0;
	public DamageAttackerTrigger (string name, Unit caster, int dur, PrefabLibrary el, int dmg, TriggerType TT) : base(name, TT, caster, el, dur)
	{
		maxTriggers = -1;
		damage = dmg;
	}
	
	public override void RunTrigger (Unit host, Unit attacker = null)
	{
		if (attacker != null) {
			base.RunTrigger (host, attacker);

			int dmg = (int)(damage * myCaster.damageDealtMod);

			attacker.TakeDamage (dmg, effectLib.getSoundEffect ("Sword1"), false, myCaster);

			Vector3 pos = attacker.transform.position;
			effectLib.CreateVisualEffect ("Slash1", pos);
		}
	}
}


