using UnityEngine;
using System.Collections.Generic;

public class CripplingStrike : Ability
{
	float damageMod = 0.1f;
	public CripplingStrike(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
    {
		Name = "Crippling Strike";
        damage = 30;
        duration = 2;
        range = 1;
        area = AreaType.Single;
        targets = TargetType.Enemy;
        maxCooldown = 1;
		stacks = 3;

		int percentageMod = (int)(damageMod * 100);

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDeals " + damage.ToString () + " damage to the target and reduces their damage dealt by " + percentageMod.ToString() + "% for " + duration.ToString() + 
				" turns. Stacks " + stacks + " times.";
    }

    public override void UseAbility(Unit target)
    {
        base.UseAbility(target);
        int dmg = (int)(damage * myCaster.damageDealtMod);

		//deal damage, if not dodged, apply effect
		if (target.TakeDamage (dmg, effectLib.getSoundEffect ("Crippling Strike"), true, myCaster) != -1) {
			target.ApplyEffect (new DamageDealtEffect ("Cripple", duration, -damageMod));
		}

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());
    }


}
