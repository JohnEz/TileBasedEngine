using UnityEngine;
using System;

public class Clobber : Ability
{
	public Clobber (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 35;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
	}
	
	public override void UseAbility(Node target)
	{
		base.UseAbility(target);
		int dmg = (int)(damage * myCaster.damageDealtMod);
		
		target.myUnit.TakeDamage(dmg, effectLib.getSoundEffect ("Blunt2"), true, myCaster);
		
		Vector3 pos = map.TileCoordToWorldCoord (target.x, target.y);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Hit1", pos).GetComponent<EffectController> ());
	}

}


