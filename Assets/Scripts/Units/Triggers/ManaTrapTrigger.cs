using UnityEngine;
using System;

public class ManaTrapTrigger : Trigger
{
	public ManaTrapTrigger (Unit caster, PrefabLibrary el) : base(TriggerType.Floor, caster, el)
	{
		maxTriggers = 1;
	}
	
	public override void RunTrigger (Unit host, Unit attacker = null)
	{
		base.RunTrigger (host, attacker);
		
		host.ApplyEffect(new ManaLeachEffect("Mana Leach", 2, myCaster, 10, 0));
		host.ApplyEffect (new Snare ("Mana Trap", 2));
		host.ShowCombatText ("Snared", host.statusCombatText);
	}

}


