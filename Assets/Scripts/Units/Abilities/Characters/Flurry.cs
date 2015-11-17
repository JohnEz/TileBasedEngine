using UnityEngine;
using System;
public class Flurry : Ability
{

	public Flurry (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Flurry";
		damage = 20;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 3;
		
		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage multiplied by the number of Lacerates on the target.";
	}
	
	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		int count = 0;
		
		//check if it has fire debuff
		foreach (Effect eff in target.myEffects) {
			if (eff.name.Contains("Lacerate")) {
				count = eff.stack;
			}
		}

		int dmg = (int)((damage * count) * myCaster.damageDealtMod);
		
		//deal damage
		target.TakeDamage (dmg, effectLib.getSoundEffect ("Lacerate"), true, myCaster);
		
		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash2", pos).GetComponent<EffectController> ());
	}
}


