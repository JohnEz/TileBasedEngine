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

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nSlams the target for " + damage.ToString () + 
				" damage, pushes them back 1 square and stuns them for 1 turn. If the target is larger than the caster, it is not pushed back or stunned but takes 3x damage.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		if (target.mySize <= UnitSize.Normal) {
			int dmg = (int)(damage * myCaster.damageDealtMod);
			int diffX = target.tileX - myCaster.tileX;
			int diffY = target.tileY - myCaster.tileY;

			Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
			myVisualEffects.Add (effectLib.CreateVisualEffect ("Hit1", pos).GetComponent<EffectController> ());


			//deal damage, if not dodged, apply effect
			if (target.TakeDamage(dmg, effectLib.getSoundEffect ("Shield Slam"), true, myCaster) != -1) {
				target.ApplyEffect(new Stun("Shield Slammed", duration));
				target.ShowCombatText ("Stunned", target.statusCombatText);
				if (map.UnitCanEnterTile(myCaster.tileX+(diffX*2), myCaster.tileY+(diffY*2))) {
					target.SlideToTile(myCaster.tileX+(diffX*2), myCaster.tileY+(diffY*2));
				}
			}
		} else {
			int dmg = (int)(damage * myCaster.damageDealtMod);

			target.TakeDamage(dmg*3, effectLib.getSoundEffect ("Shield Slam"), true, myCaster);
			myCaster.ApplyEffect(new BlockEffect("Shield Slam", 2, 15));
		}

	}

	public override void UpdateAbility ()
	{
		if (!myTarget.moving || myTarget.isDead) {
			AbilityFinished = true;
		}
	}


}

