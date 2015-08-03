using UnityEngine;
using System;

public class ShieldSlam : Ability
{
	public ShieldSlam(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 20;
		duration = 1;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 2;
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

			if (map.UnitCanEnterTile(myCaster.tileX+(diffX*2), myCaster.tileY+(diffY*2))) {
				target.SlideToTile(myCaster.tileX+(diffX*2), myCaster.tileY+(diffY*2));
			}

			target.TakeDamage(dmg);
			target.ApplyEffect(new Stun("Shield Slammed", duration));
			target.ShowCombatText ("Stunned", target.statusCombatText);
		} else {
			int dmg = (int)(damage * myCaster.damageDealtMod);

			target.TakeDamage(dmg*3);
		}

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Shield Slam"));
	}

	public override void UpdateAbility ()
	{
		if (!myTarget.moving) {
			AbilityFinished = true;
		}
	}


}

