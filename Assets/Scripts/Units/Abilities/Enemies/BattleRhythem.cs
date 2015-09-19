using UnityEngine;
using System;

public class BattleRhythem : Ability
{
	int moveMod = 1;
	int cooldownMod = 1;
	
	public BattleRhythem (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		range = 6;
		duration = 2;
		manaCost = 30;
		manaGain = 30;
		healing = 10;
		area = AreaType.Self;
		targets = TargetType.Ally;
		maxCooldown = 4;
		
		AIPriority = 10;
		AIRanged = true;
		AISupportsAlly = true;
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		
		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Drum1", pos).GetComponent<EffectController> ());
		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Drum Double"));
		
		foreach (Node n in target.reachableNodes) {
			Unit sUnit = n.myUnit;

			//if the unit is in combat
			if (sUnit.isActive && sUnit != myCaster) {
				sUnit.ApplyEffect( new MovespeedMod("Battle Rhythem", duration, moveMod)); 
				sUnit.ApplyEffect( new CooldownEffect("Battle Rhythem", duration, cooldownMod));
				sUnit.AddRemoveMana(manaGain);
			}
		}
	}
}


