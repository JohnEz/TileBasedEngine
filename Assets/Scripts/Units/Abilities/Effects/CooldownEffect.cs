using UnityEngine;
using System.Collections.Generic;

public class CooldownEffect : Effect
{
	
	int baseCooldownMod = 0;
	int cooldownMod = 0;
	
	public CooldownEffect(string n, int dur, int mod, int stacks = 1, Sprite icon = null)
		: base(n, dur, stacks, icon)
	{
		baseCooldownMod = mod;
		cooldownMod = 1 + mod;
		description = "Cooldown speed effect " + cooldownMod.ToString();
	}
	
	public override void RunEffect(Unit u, bool reapply = false)
	{
		base.RunEffect(u, reapply);
		u.cooldownSpeed += cooldownMod;
	}
	
	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			cooldownMod += baseCooldownMod;
		}
		description = "Cooldown speed effect " + cooldownMod.ToString();
	}
	
}
