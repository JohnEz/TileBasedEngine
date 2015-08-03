using UnityEngine;
using System.Collections.Generic;

public class Snare : Effect
{

    public Snare(string n, int dur)
        : base(n, dur, 1)
    {
		description = "Snare";
    }

	public override void RunEffect(Unit u, bool reapply = false)
    {
        base.RunEffect(u, reapply);

        u.movespeed = 0;
		u.remainingMove = 0;
		if (!reapply) {
			u.ShowCombatText ("Snared", u.statusCombatText);
		}
    }

}
