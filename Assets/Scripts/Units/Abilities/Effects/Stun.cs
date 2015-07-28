using UnityEngine;
using System.Collections.Generic;



public class Stun : Effect
{

    public Stun(string n, int dur)
        : base(n, dur, 1)
    {
		description = "Stun";
    }

    public override void RunEffect(Unit u)
    {
        base.RunEffect(u);
        u.movespeed = 0;
        u.maxAP = 0;
		u.ShowCombatText ("Stunned", u.statusText);
    }

}