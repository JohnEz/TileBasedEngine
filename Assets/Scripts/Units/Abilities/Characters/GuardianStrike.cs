using UnityEngine;
using System.Collections.Generic;

public class GuardianStrike : Ability
{
	public GuardianStrike(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
    {
		Name = "Guardian Strike";
        damage = 30;
        duration = 2;
        range = 1;
        area = AreaType.Single;
        targets = TargetType.Enemy;
        maxCooldown = 1;
		stacks = 3;


		/*description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and reduces their damage dealt by " + percentageMod.ToString() + "% for " + duration.ToString() + 
				" turns. Stacks " + stacks + " times.";*/

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and you gain 1 Guard Point. Guard Points give a 4% increase to block chance.";
    }

    public override void UseAbility(Unit target)
    {
        base.UseAbility(target);
        int dmg = (int)(damage * myCaster.damageDealtMod);


		//deal damage, if not dodged, apply effect
		if (target.TakeDamage (dmg, effectLib.getSoundEffect ("Crippling Strike"), true, myCaster) != -1) {
			myCaster.AddGuardPoints (1);
		}

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());
    }


}
