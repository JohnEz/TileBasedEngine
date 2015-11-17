using UnityEngine;
using System.Collections.Generic;

public class DodgeEffect : Effect
{
	
	int baseDodgeMod = 0;
	int dodgeMod = 0;
	
	public DodgeEffect(string n, int dur, int mod, int stacks = 1, Sprite icon = null)
		: base(n, dur, stacks, icon)
	{
		baseDodgeMod = mod;
		dodgeMod = mod;
		description = "Dodge chance effect " + dodgeMod.ToString() + "%";
	}
	
	public override void RunEffect(Unit u, bool reapply = false)
	{
		base.RunEffect(u, reapply);
		u.dodgeChance += dodgeMod;
	}
	
	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			dodgeMod += baseDodgeMod;
		}
		description = "Dodge chance effect " + dodgeMod.ToString() + "%";
	}
	
}
