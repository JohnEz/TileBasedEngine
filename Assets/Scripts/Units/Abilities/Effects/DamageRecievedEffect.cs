using UnityEngine;
using System.Collections.Generic;

public class DamageRecievedEffect : Effect
{
	float baseDamageRecievedMod = 0;
    float damageRecievedMod = 0;

	public DamageRecievedEffect(string n, int dur, float mod, int stacks = 1, Sprite icon = null)
		: base(n, dur, stacks, icon)
    {
		baseDamageRecievedMod = mod;
		damageRecievedMod = mod;

		int percent = (int)(damageRecievedMod * 100);
		description = "Damage Recieved Mod " + percent.ToString() + "%";
    }

	public override void RunEffect(Unit u, bool reapply = false)
    {
        base.RunEffect(u, reapply);
        u.damageRecievedMod += damageRecievedMod;
    }

	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			damageRecievedMod += baseDamageRecievedMod;
		}
		int percent = (int)(damageRecievedMod * 100);
		description = "Damage Recieved Mod " + percent.ToString() + "%";
	}
}
