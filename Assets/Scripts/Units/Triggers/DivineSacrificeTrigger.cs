using UnityEngine;
using System;


public class DivineSacrificeTrigger : Trigger
{


	public DivineSacrificeTrigger (Unit caster) : base(TriggerType.Death, caster)
	{
		maxTriggers = 1;
	}

	public override void RunTrigger (Unit host)
	{
		base.RunTrigger (host);

		myCaster.AddRemoveMana (20);
	}
}


