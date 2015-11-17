using UnityEngine;
using System;


public class CracklingArrowTrigger : Trigger
{
	int damage = 0;
	public CracklingArrowTrigger (string name, Unit caster, int dur, PrefabLibrary el, int dmg, Sprite icon = null)
		: base(name, TriggerType.Hit, caster, el, dur, icon)
	{
		maxTriggers = 1;
		damage = dmg;
	}
	
	public override void RunTrigger (Unit host, Unit attacker = null)
	{
		base.RunTrigger (host, null);

		int dmg = (int)(damage * myCaster.damageDealtMod);

		host.TakeDamage (dmg, null, false, null);
		host.ApplyEffect (new Stun ("Crackling Arrow", 1));

		Vector3 pos = host.transform.position;
		effectLib.CreateVisualEffect ("Hit1", pos);

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Crackling Arrow Pop"));
	}
}


