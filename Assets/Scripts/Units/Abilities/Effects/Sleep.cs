using UnityEngine;
using System.Collections.Generic;



public class Sleep : Effect
{
	
	public Sleep(string n, int dur)
		: base(n, dur, 1)
	{
		description = "Sleep";
	}
	
	public override void RunEffect(Unit u, bool reapply = false)
	{
		base.RunEffect(u, reapply);
		
		u.movespeed = 0;
		u.maxAP = 0;
		if (!reapply) {
			u.ShowCombatText ("Asleep " + duration.ToString(), u.statusCombatText);
		}
	}
	
}