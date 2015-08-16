using UnityEngine;
using System.Collections.Generic;
using System;

public class CracklingArrow : Ability
{
	Node targetNode = null;

	public CracklingArrow (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Crackling Arrow";
		damage = 15;
		manaCost = 25;
		duration = 3;
		area = AreaType.LineAOE;
		AOERange = 1;
		targets = TargetType.Enemy;
		range = 7;
		maxCooldown = 1;
	}
	
	public override void UseAbility (Node n)
	{
		base.UseAbility (n);
		
		targetNode = n;

		Vector3 targetPos = map.TileCoordToWorldCoord (n.x, n.y);

		myProjectiles.Add(effectLib.CreateProjectile("Crackling Arrow", myCaster.transform.position, targetPos, 10).GetComponent<ProjectileController>());
		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Exploit Weakness Fire"));
	}
	
	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();

		int dmg = (int)(damage * myCaster.damageDealtMod);

		Vector3 pos = map.TileCoordToWorldCoord (targetNode.x, targetNode.y);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Crackle", pos).GetComponent<EffectController> ());

		// deal damage to target square
		if (targetNode.myUnit != null && targetNode.myUnit.team != myCaster.team) {
			targetNode.myUnit.TakeDamage(dmg);
			if (targetNode.myUnit != null && targetNode.myUnit.team != myCaster.team) {
				targetNode.myUnit.AddTrigger(new CracklingArrowTrigger(myCaster, duration, effectLib));
			}
		}

		//create crackle effects on neighbour nodes
		foreach (Node n in targetNode.neighbours) {
			//if its a targetable tile
			if (targetNode.reachableNodes.Contains(n)) {
				pos = map.TileCoordToWorldCoord (n.x, n.y);
				myVisualEffects.Add (effectLib.CreateVisualEffect ("Crackle", pos).GetComponent<EffectController> ());
				if (n.myUnit != null && n.myUnit.team != myCaster.team) {
					n.myUnit.AddTrigger(new CracklingArrowTrigger(myCaster, duration, effectLib));
				}
			}
		}

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Crackling Arrow Hit"));
		
	}
}


