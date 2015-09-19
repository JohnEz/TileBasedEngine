using UnityEngine;
using System;

public class TotemHeal : Ability
{	

	public TotemHeal (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		range = 6;
		duration = 1;
		area = AreaType.Self;
		targets = TargetType.Ally;
		maxCooldown = 1;
		healing = 30;
		
		AIPriority = 10;
		AIRanged = true;
		AISupportsAlly = true;
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		
		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Drum1", pos).GetComponent<EffectController> ());
		//myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Drum Double"));
		
		foreach (Node n in target.reachableNodes) {
			Unit sUnit = n.myUnit;
			
			//if the unit is in combat
			if (sUnit.isActive) {
				sUnit.TakeHealing(healing);
			}
		}
	}
}


