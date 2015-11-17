using UnityEngine;
using System;
using System.Collections.Generic;

public class Amplify : Ability
{
	float mod = 1.0f;
	public Amplify (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Amplify";
		manaCost = 30;
		duration = 2;
		maxCooldown = 3;
		range = 5;
		AOERange = 2;
		area = AreaType.AOE;
		targets = TargetType.All;
		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nSpeads out a 100% modifier accross allies and enemies, increasing ally damage and reducing enemies for " + duration.ToString() + " turns.";
	}
	
	public override void UseAbility (Unit target)
	{
		AbilityFinished = false;

		if (target.team == myCaster.team) {
			target.ApplyEffect(new DamageDealtEffect("Amplify", duration, mod, 1, effectLib.getIcon("Arcane Pulse").sprite));
		} else {
			target.ApplyEffect(new DamageDealtEffect("Amplify", duration, -mod, 1, effectLib.getIcon("Arcane Pulse").sprite));
		}
		
	}
	
	public override void UseAbility (Node n)
	{
		List<Node> targetSquares = n.reachableNodes;
		base.UseAbility (n);
		int count = 0;
		foreach (Node no in targetSquares) {
			if (no.myUnit != null) {
				count++;
			}
		}

		if (count > 1) {
			mod = 1.0f / count;
		} else {
			mod = 1.0f;
		}

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


