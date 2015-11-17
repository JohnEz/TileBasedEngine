using UnityEngine;
using System.Collections.Generic;

public class Snare : Effect
{

	public Snare(string n, int dur, Sprite icon = null)
        : base(n, dur, 1, icon)
    {
		description = "Snared";
    }

	public override void RunEffect(Unit u, bool reapply = false)
    {
        base.RunEffect(u, reapply);

        u.movespeed = -100;
		u.remainingMove = -100;
		if (!reapply) {
			u.ShowCombatText ("Snared", u.statusCombatText);
		}
    }

}
