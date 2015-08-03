using UnityEngine;
using System.Collections.Generic;

public class DamageRecievedEffect : Effect
{
	float baseDamageRecievedMod = 0;
    float damageRecievedMod = 0;

    public DamageRecievedEffect(string n, int dur, float mod, int stacks = 1) : base(n, dur, stacks)
    {
		baseDamageRecievedMod = mod;
		damageRecievedMod = 1 + mod;
    }

	public override void RunEffect(Unit u, bool reapply = false)
    {
        base.RunEffect(u, reapply);
        u.damageRecievedMod *= damageRecievedMod;
    }

	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			damageRecievedMod += baseDamageRecievedMod;
		}
	}
}
