using UnityEngine;
using System;

public class ShieldSlam : Ability
{
	public ShieldSlam(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Shield Slam";
		damage = 20;
		duration = 1;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 3;
		usesGuard = true;

		description = "Cooldown: " + maxCooldown.ToString () + "  REQUIRES GUARD POINTS" +
			"\nSlams the target for " + damage.ToString () + 
				" damage times the number of Guard Points and stuns for half the amount.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		int combo = myCaster.UseGuardPoints ();
		int dmg = (int)(damage * myCaster.damageDealtMod);

		if (target.TakeDamage(dmg * combo, effectLib.getSoundEffect ("Shield Slam"), true, myCaster) != -1) {
			target.ApplyEffect(new Stun("Shield Slammed", (int)(combo / 2), effectLib.getIcon("Shield Slam").sprite));
			target.ShowCombatText ("Stunned", target.statusCombatText);
		}


	}

	public override void UpdateAbility ()
	{
		if (!myTarget.moving || myTarget.isDead) {
			AbilityFinished = true;
		}
	}


}

