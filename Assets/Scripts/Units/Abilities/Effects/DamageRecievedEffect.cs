using UnityEngine;
using System.Collections.Generic;

public class DamageRecievedEffect : Effect
{
    float damageRecievedMod = 0;

    public DamageRecievedEffect(string n, int dur, float mod) : base(n, dur)
    {
        damageRecievedMod = 1 + mod;
    }

    public override void RunEffect(Unit u)
    {
        base.RunEffect(u);
        u.damageRecievedMod *= damageRecievedMod;
    }
}
