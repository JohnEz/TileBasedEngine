using UnityEngine;
using System;


public class RemoveEffect : Trigger
{

	Effect effectToRemove = null;
	
	public RemoveEffect (Unit caster, TriggerType trig, Effect eff) : base(trig, caster)
	{
		maxTriggers = 1;
		effectToRemove = eff;
	}
	
	public override void RunTrigger (Unit host)
	{
		base.RunTrigger (host);
		
		host.RemoveEffect(effectToRemove);
	}
}


