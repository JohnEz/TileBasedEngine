using UnityEngine;
using System;

public class ManaTrapTrigger : Trigger
{
	public ManaTrapTrigger (Unit caster) : base(TriggerType.Floor, caster)
	{
		maxTriggers = 1;
		myTargets = TargetType.All;
	}
	
	public override void RunTrigger (Unit host)
	{
		base.RunTrigger (host);
		
		host.ApplyEffect(new ManaLeachEffect("Mana Trap", 2, myCaster, 10, 0));
		host.ApplyEffect (new Snare ("Mana Trap", 2));
	}

}


