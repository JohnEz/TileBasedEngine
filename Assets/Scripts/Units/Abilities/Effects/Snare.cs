using UnityEngine;
using System.Collections.Generic;

public class Snare : Effect
{

    public Snare(string n, int dur)
        : base(n, dur)
    {
		description = "Snare";
    }

    public override void RunEffect(Unit u)
    {
        base.RunEffect(u);
        u.movespeed = 0;
    }

}
