using UnityEngine;
using System;

public class MovespeedMod : Effect
{
	int baseMod = 0;
	int speedMod = 0;

	public MovespeedMod (string n, int dur, int mod, int stacks = 1, Sprite icon = null)
		: base(n, dur, stacks, icon)
	{
		baseMod = mod;
		speedMod = mod;
		description = "Move speed modifier " + speedMod.ToString();
	}

	public override void RunEffect (Unit u, bool reapply = false)
	{
		base.RunEffect (u, reapply);
		u.movespeed += speedMod;
	}

	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			speedMod += baseMod;
		}

		description = "Move speed modifier " + speedMod.ToString();
	}
}


