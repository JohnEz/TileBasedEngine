using UnityEngine;
using System;


public class DivineSacrificeTrigger : Trigger
{


	public DivineSacrificeTrigger (Unit caster, int dur, PrefabLibrary el) : base(TriggerType.Death, caster, el)
	{
		maxTriggers = 1;
		maxDuration = dur;
	}

	public override void RunTrigger (Unit host, Unit attacker = null)
	{
		base.RunTrigger (host, attacker);

		myCaster.AddRemoveMana (20);
	}
}


