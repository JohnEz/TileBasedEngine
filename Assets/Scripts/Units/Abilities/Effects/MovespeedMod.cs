using UnityEngine;
using System;

public class MovespeedMod : Effect
{
	int baseMod = 0;
	int speedMod = 0;

	public MovespeedMod (string n, int dur, int mod, int stacks = 1) : base(n, dur, stacks)
	{
		baseMod = mod;
		speedMod = mod;
	}

	public override void RunEffect (Unit u)
	{
		base.RunEffect (u);
		u.movespeed += speedMod;
	}

	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			speedMod += baseMod;
		}
	}
}


