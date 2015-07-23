using UnityEngine;
using System.Collections.Generic;

public class DamageDealtEffect : Effect
{

    float damageDealtMod = 0;

    public DamageDealtEffect(string n, int dur, float mod)
        : base(n, dur)
    {
        damageDealtMod = 1 + mod;
    }

    public override void RunEffect(Unit u)
    {
        base.RunEffect(u);
        u.damageDealtMod *= damageDealtMod;
    }

}
