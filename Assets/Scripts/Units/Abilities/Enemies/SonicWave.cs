using UnityEngine;
using System;

public class SonicWave : Ability
{
	public SonicWave (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 20;
		manaGain = 15;
		range = 6;
		duration = 2;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 4;
		
		AIPriority = 5;
		AIRanged = true;
		AISupportsAlly = false;
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		
		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Drum1", pos).GetComponent<EffectController> ());
		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Drum Double"));

		target.myUnit.TakeDamage (damage, null, true, myCaster);
		myCaster.AddRemoveMana (manaGain);

	}
}


