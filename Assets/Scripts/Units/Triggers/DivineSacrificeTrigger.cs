using UnityEngine;
using System;


public class DivineSacrificeTrigger : Trigger
{

	int manaGain = 0;
	public DivineSacrificeTrigger (string name, Unit caster, int dur, PrefabLibrary el, int mGain) : base(name, TriggerType.Death, caster, el, dur)
	{
		maxTriggers = 1;
		manaGain = mGain;
	}

	public override void RunTrigger (Unit host, Unit attacker = null)
	{
		base.RunTrigger (host, attacker);

		myCaster.AddRemoveMana (manaGain);
	}
}


