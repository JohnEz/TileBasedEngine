using UnityEngine;
using System.Collections.Generic;

public class SmokeBombTileEffect : Effect
{
	List<Node> tiles;
	int LOSMod = 0;

	public SmokeBombTileEffect(string n, int dur, List<Node> targets, int mod, int stacks = 1, Sprite icon = null)
		: base(n, dur, 1, icon)
	{
		tiles = targets;
		LOSMod = mod;
		visible = false;
	}
	
	public override void OnExpire ()
	{
		base.OnExpire ();

		foreach (Node n in tiles) {
			n.LOSMod -= LOSMod;

			Effect removeEffect = null;

			foreach(Effect eff in n.myEffects) {
				if (eff.name.Contains("Smokey")) {
					removeEffect = eff;
				}
			}

			n.myEffects.Remove(removeEffect);

		}
	}
}

