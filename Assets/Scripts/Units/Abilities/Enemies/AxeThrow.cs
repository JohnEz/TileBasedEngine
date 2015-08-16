using UnityEngine;
using System;

public class AxeThrow : Ability
{
	public AxeThrow (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 20;
		range = 5;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		
		myTarget = target;
		int dirX = myCaster.tileX - target.tileX;
		int dirY = myCaster.tileY - target.tileY;

		//create effect facing target
		if (dirX < 0 || dirY > 0) {
			myProjectiles.Add (effectLib.CreateProjectile ("Axe", myCaster.transform.position, target.transform.position, 10).GetComponent<ProjectileController> ());
		} else {
			myProjectiles.Add (effectLib.CreateProjectile ("Axe", myCaster.transform.position, target.transform.position, 10, true).GetComponent<ProjectileController> ());
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


