using UnityEngine;
using System.Collections.Generic;



public class Stun : Effect
{

	public Stun(string n, int dur, Sprite icon = null)
        : base(n, dur, 1, icon)
    {
		description = "Stunned";
    }

	public override void RunEffect(Unit u, bool reapply = false)
    {
        base.RunEffect(u, reapply);

        u.movespeed = -100;
        u.maxAP = -100;
		if (!reapply) {
			u.ShowCombatText ("Stunned " + duration.ToString(), u.statusCombatText);
		}
    }

}