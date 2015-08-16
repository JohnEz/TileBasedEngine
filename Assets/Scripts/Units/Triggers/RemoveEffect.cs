using UnityEngine;
using System;


public class RemoveEffect : Trigger
{

	Effect effectToRemove = null;
	
	public RemoveEffect (Unit caster, TriggerType trig, Effect eff, PrefabLibrary el) : base(trig, caster, el)
	{
		maxTriggers = 1;
		effectToRemove = eff;
	}
	
	public override void RunTrigger (Unit host, Unit attacker = null)
	{
		base.RunTrigger (host, attacker);
		
		host.RemoveEffect(effectToRemove);
	}
}


