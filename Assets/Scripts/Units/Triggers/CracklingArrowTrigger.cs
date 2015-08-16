using UnityEngine;
using System;


public class CracklingArrowTrigger : Trigger
{
	int damage = 15;
	public CracklingArrowTrigger (Unit caster, int dur, PrefabLibrary el) : base(TriggerType.Hit, caster, el)
	{
		maxTriggers = 1;
		maxDuration = dur;

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


