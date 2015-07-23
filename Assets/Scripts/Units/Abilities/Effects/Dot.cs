using UnityEngine;
using System.Collections.Generic;

public class Dot : Effect
{
    int damage = 0;

    public Dot(string n, int dur, int dmg)
        : base(n, dur)
    {
        damage = dmg;
    }

    public override void RunEffect(Unit u)
    {
 	    base.RunEffect(u);
        //deal damage 
        u.TakeDamage(damage);
    }
}

