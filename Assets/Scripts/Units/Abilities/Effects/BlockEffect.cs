using UnityEngine;
using System.Collections.Generic;

public class BlockEffect : Effect
{
	
	int baseBlockMod = 0;
	int blockMod = 0;

	public BlockEffect(string n, int dur, int mod, int stacks = 1)
		: base(n, dur, stacks)
	{
		baseBlockMod = mod;
		blockMod = 1 + mod;
	}
	
	public override void RunEffect(Unit u, bool reapply = false)
	{
		base.RunEffect(u, reapply);
		u.blockChance += blockMod;
	}
	
	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			blockMod += baseBlockMod;
		}
	}
	
}
