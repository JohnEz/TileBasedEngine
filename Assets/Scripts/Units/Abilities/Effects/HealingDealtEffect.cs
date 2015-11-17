using UnityEngine;
using System.Collections.Generic;

public class HealingDealtEffect : Effect
{
	
	float baseHealingMod = 0;
	float healingMod = 0;
	
	public HealingDealtEffect(string n, int dur, float mod, int stacks = 1, Sprite icon = null)
		: base(n, dur, stacks, icon)
	{
		baseHealingMod = mod;
		healingMod = mod;
		int percent = (int)(healingMod * 100);
		description = "Healing dealt effect " + percent.ToString() + "%";
	}
	
	public override void RunEffect(Unit u, bool reapply = false)
	{
		base.RunEffect(u, reapply);
		u.healingDealtMod += healingMod;
	}
	
	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			healingMod += baseHealingMod;
		}
		int percent = (int)(healingMod * 100);
		description = "Healing dealt effect " + percent.ToString() + "%";
	}
	
}
