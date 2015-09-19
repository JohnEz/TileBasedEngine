using UnityEngine;
using System;

public class AxeThrow : Ability
{
	public AxeThrow (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 30;
		range = 5;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;

		AIRanged = true;
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		
		myTarget = target.myUnit;
		int dirX = myCaster.tileX - target.x;
		int dirY = myCaster.tileY - target.y;

		//create effect facing target
		if (dirX < 0 || dirY > 0) {
			myProjectiles.Add (effectLib.CreateProjectile ("Axe", myCaster.transform.position, target.myUnit.transform.position, 10).GetComponent<ProjectileController> ());
		} else {
			myProjectiles.Add (effectLib.CreateProjectile ("Axe", myCaster.transform.position, target.myUnit.transform.position, 10, true).GetComponent<ProjectileController> ());
		}
	}

	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();
		Vector3 pos = map.TileCoordToWorldCoord (myTarget.tileX, myTarget.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());
		
		int dmg = (int)(damage * myCaster.damageDealtMod);
		
		// deal damage, if not dodged apply cripple
		if (myTarget.TakeDamage (dmg, effectLib.getSoundEffect ("Blunt1"), true, myCaster) != -1) {
			myTarget.ApplyEffect (new Dot("Axe Bleed", 2, 5, 3));
			myTarget.ShowCombatText ("Bleeding", myTarget.statusCombatText);
		}
		
	}
}


