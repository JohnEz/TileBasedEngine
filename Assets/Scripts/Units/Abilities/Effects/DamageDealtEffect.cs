using UnityEngine;
using System.Collections.Generic;

public class DamageDealtEffect : Effect
{

	float baseDamageDealtMod = 0;
    float damageDealtMod = 0;

	public DamageDealtEffect(string n, int dur, float mod, int stacks = 1, Sprite icon = null)
        : base(n, dur, stacks, icon)
    {
		baseDamageDealtMod = mod;
        damageDealtMod = mod;
		int percent = (int)(damageDealtMod * 100);
		description = "Damage dealt effect " + percent.ToString() + "%";
    }

	public override void RunEffect(Unit u, bool reapply = false)
    {
        base.RunEffect(u, reapply);
        u.damageDealtMod += damageDealtMod;
    }

	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			damageDealtMod += baseDamageDealtMod;
		}
		int percent = (int)(damageDealtMod * 100);
		description = "Damage dealt effect " + percent.ToString() + "%";
	}

}
