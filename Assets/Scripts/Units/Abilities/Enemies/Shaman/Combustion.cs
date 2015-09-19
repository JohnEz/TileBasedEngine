using UnityEngine;
using System;

public class Combustion : Ability
{
	public Combustion(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Combustion";
		damage = 20;
		duration = 1;
		range = 3;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 3;

		AIPriority = 4;
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		
		
		int dmg = (int)(damage * myCaster.damageDealtMod);

		int count = 0;

		//check if it has fire debuff
		foreach (Effect eff in target.myEffects) {
			if (eff.name.Contains("Potent Fire")) {
				count = eff.stack;
			}
		}

		++count;

		//deal damage, if not dodged, apply effect
		target.myUnit.TakeDamage (dmg * count, effectLib.getSoundEffect ("Fireball Hit"), true, myCaster);
		
		Vector3 pos = map.TileCoordToWorldCoord (target.x, target.y);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Fireball Explosion", pos).GetComponent<EffectController> ());
		
	}
	
	
}

