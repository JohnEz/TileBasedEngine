using UnityEngine;
using System;
using System.Collections.Generic;

public class FlashFreeze : Ability
{
	public FlashFreeze (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 20;
		manaCost = 20;
		duration = 1;
		maxCooldown = 3;
		range = 5;
		AOERange = 2;
		area = AreaType.AOE;
		targets = TargetType.All;
	}
	
	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		int dmg = (int)(damage * myCaster.damageDealtMod);
		
		target.TakeDamage (dmg);
		target.ApplyEffect (new Stun ("Frozen", duration));
		target.ShowCombatText ("Stunned", target.statusCombatText);
		
	}
	
	public override void UseAbility (Node n)
	{
		List<Node> targetSquares = n.reachableNodes;
		base.UseAbility (n);
		
		foreach (Node no in targetSquares) {
			if (no.myUnit != null) {
				UseAbility(no.myUnit);
			}
		}

		Vector3 pos = map.TileCoordToWorldCoord (n.x, n.y);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Flash Freeze", pos).GetComponent<EffectController> ());

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Flash Freeze"));

		
	}

}


