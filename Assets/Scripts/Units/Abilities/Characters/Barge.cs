using UnityEngine;
using System;

public class Barge : Ability
{
	public Barge(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Barge";
		damage = 25;
		duration = 2;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 2;
		
		description = "Cooldown: " + maxCooldown.ToString () +
			"\nBarges the target for " + damage.ToString () + 
				" damage, pushes them back 1 square and snares them. If the target is larger than the caster, it is not pushed back. Gain 2 Guard Points";
	}
	
	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Hit1", pos).GetComponent<EffectController> ());
		int dmg = (int)(damage * myCaster.damageDealtMod);
		myCaster.AddGuardPoints (2);

		// if the target is normal or smaller size
		if (target.mySize <= UnitSize.Normal) {
			int diffX = target.tileX - myCaster.tileX;
			int diffY = target.tileY - myCaster.tileY;
			

			
			
			//deal damage, if not dodged, apply effect
			if (target.TakeDamage(dmg, effectLib.getSoundEffect ("Shield Slam"), true, myCaster) != -1) {
				target.ApplyEffect (new Snare ("Barged", duration, effectLib.getIcon("Barge").sprite));
				target.ShowCombatText ("Snared", myTarget.statusCombatText);
				if (map.UnitCanEnterTile(myCaster.tileX+(diffX*2), myCaster.tileY+(diffY*2))) {
					target.SlideToTile(myCaster.tileX+(diffX*2), myCaster.tileY+(diffY*2));
					myCaster.SlideToTile(myCaster.tileX+(diffX), myCaster.tileY+(diffY));
				}
			}
		} else {
			if (target.TakeDamage(dmg, effectLib.getSoundEffect ("Shield Slam"), true, myCaster) != -1) {
				target.ApplyEffect (new Snare ("Barged", duration, effectLib.getIcon("Barge").sprite));
				target.ShowCombatText ("Snared", myTarget.statusCombatText);
			}
		}
		
	}
	
	public override void UpdateAbility ()
	{
		if (!myTarget.moving || myTarget.isDead) {
			AbilityFinished = true;
		}
	}
	
	
}

