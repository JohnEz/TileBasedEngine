using UnityEngine;
using System;

public class TripleShot : Ability
{
	public TripleShot (Unit u, TileMap m, VisualEffectLibrary el) : base(u, m , el)
	{
		damage = 10;
		range = 5;
		maxCooldown = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		int dmg = (int)(damage * myCaster.damageDealtMod);

		myCaster.AddRemoveMana (15);

		target.TakeDamage (dmg);
		target.TakeDamage (dmg);
		target.TakeDamage (dmg);

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateEffect ("Triple Shot", pos).GetComponent<EffectController> ());
	}
}


