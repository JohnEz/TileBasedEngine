using UnityEngine;
using System;

public class MovespeedMod : Effect
{
	int speedMod = 0;

	public MovespeedMod (string n, int dur, int mod) : base(n, dur)
	{
		speedMod = mod;
	}

	public override void RunEffect (Unit u)
	{
		base.RunEffect (u);
		u.movespeed += speedMod;
	}
}


