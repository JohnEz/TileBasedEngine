using UnityEngine;
using System;
public class ShadowStep : Ability
{
	
	int bleedDmg = 10;
	EffectController wwEffect;
	bool createdEffects = false;
	public ShadowStep (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Shadow Step";
		duration = 3;
		range = 6;
		area = AreaType.Floor;
		targets = TargetType.Enemy;
		maxCooldown = 2;
		stacks = 5;
		
		description = "Cooldown: " + maxCooldown.ToString () +
			"\nTeleports to the target location and makes all enemies around bleed for " + bleedDmg.ToString() + " damage each turn for " + duration.ToString() + 
				" turns. Stacks " + stacks + " times.";
	}
	
	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		createdEffects = false;

		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Dash", pos, false, false).GetComponent<EffectController> ());

		pos = map.TileCoordToWorldCoord (n.x, n.y);
		wwEffect = effectLib.CreateVisualEffect ("Whirlwind", pos).GetComponent<EffectController> ();
		myVisualEffects.Add (wwEffect);

		//change the position of the caster and clear old node
		map.GetNode (myCaster.tileX, myCaster.tileY).myUnit = null;
		myCaster.tileX = n.x;
		myCaster.tileY = n.y;
		n.myUnit = myCaster;
		myCaster.transform.position = pos;
	}

	public override void UpdateAbility() {
		AbilityFinished = true;

		//once the wirlwind effect has finished, create slashes
		if (!createdEffects && wwEffect.animFinished) {
			createdEffects = true;

			targetNode.reachableNodes = map.FindReachableTiles(targetNode.x, targetNode.y, 1, true);

			foreach(Node n in targetNode.reachableNodes) {
				if (n.myUnit != null && n.myUnit.team != myCaster.team) {
					n.myUnit.ApplyEffect(new Dot ("Lacerate", duration, bleedDmg, stacks, effectLib.getIcon("Lacerate").sprite));

					Vector3 pos = map.TileCoordToWorldCoord (n.myUnit.tileX, n.myUnit.tileY);
					myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash2", pos).GetComponent<EffectController> ());
				}
			}

		}

		foreach (EffectController ec in myVisualEffects) {
			if (!ec.animFinished) {
				AbilityFinished = false;
			}
		}

	}
}


