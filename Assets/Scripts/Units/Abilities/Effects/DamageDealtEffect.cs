using UnityEngine;
using System.Collections.Generic;

public class DamageDealtEffect : Effect
{

	float baseDamageDealtMod = 0;
    float damageDealtMod = 0;

    public DamageDealtEffect(string n, int dur, float mod, int stacks = 1)
        : base(n, dur, stacks)
    {
		baseDamageDealtMod = mod;
        damageDealtMod = 1 + mod;
    }

    public override void RunEffect(Unit u)
    {
        base.RunEffect(u);
        u.damageDealtMod *= damageDealtMod;
    }

	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			damageDealtMod += baseDamageDealtMod;
		}
	}

}
