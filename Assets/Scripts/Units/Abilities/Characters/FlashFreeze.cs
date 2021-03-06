using UnityEngine;
using System;
using System.Collections.Generic;

public class FlashFreeze : Ability
{
	public FlashFreeze (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Flash Freeze";
		damage = 20;
		manaCost = 30;
		duration = 1;
		maxCooldown = 3;
		range = 5;
		AOERange = 2;
		area = AreaType.AOE;
		targets = TargetType.All;
		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nDeals " + damage.ToString () + " damage to both allies and enemies and stuns them for " + duration.ToString() + " turns.";
	}
	
	public override void UseAbility (Unit target)
	{
		cooldown = maxCooldown;
		AbilityFinished = false;
		myTarget = target;

		int dmg = (int)(damage * myCaster.damageDealtMod);

		//deal damage, if not dodged, apply effect
		if (target.TakeDamage (dmg, null, true, myCaster) != -1) {
			if (target.ApplyEffect (new Stun ("Frozen", duration, effectLib.getIcon("Flash Freeze").sprite))) {
				target.ShowCombatText ("Stunned", target.statusCombatText);
			}
		}
		
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


