using UnityEngine;
using System;
using System.Collections.Generic;

public class TotemSnare : Ability
{	
	
	public TotemSnare (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		range = 1;
		duration = 2;
		area = AreaType.SelfAOE;
		targets = TargetType.Enemy;
		maxCooldown = 1;
		AOERange = 3;
		
		AIPriority = 1;
		AIRanged = false;
		AISupportsAlly = false;
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		
		foreach (Node no in target.reachableNodes) {
			if (no.myUnit != null && no.myUnit.team != myCaster.team) {
				no.myUnit.ApplyEffect (new Snare("Snare", duration));
				no.myUnit.ShowCombatText ("Snared", no.myUnit.statusCombatText);
			}
		}
		
		Vector3 pos = map.TileCoordToWorldCoord (target.x, target.y);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Drum1", pos).GetComponent<EffectController> ());
		//myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Flash Freeze"));

	}
	
}


