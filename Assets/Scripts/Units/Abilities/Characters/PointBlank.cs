using UnityEngine;
using System;

public class PointBlank : Ability
{
	public PointBlank (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Point Blank";
		damage = 20;
		duration = 0;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 1;
		usesGuard = true;

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nUses all the combo points and deals " + damage.ToString () + 
				" damage, per combo point, to the target. The caster is pushed back equal to the number of combo points.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		int dmg = (int)(damage * myCaster.damageDealtMod);
		int combo = myCaster.UseGuardPoints ();

		int dirX = myCaster.tileX - target.tileX;
		int dirY = myCaster.tileY - target.tileY;

		int currX = myCaster.tileX + dirX;
		int currY = myCaster.tileY + dirY;

		//create effect facing target
		if (dirX < 0 || dirY > 0) {
			Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY) + new Vector3(0.3f, 0, 0);
			myVisualEffects.Add (effectLib.CreateVisualEffect ("Point Blank", pos).GetComponent<EffectController> ());
		} else {
			Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY) + new Vector3(0.3f * -1, 0, 0);
			myVisualEffects.Add (effectLib.CreateVisualEffect ("Point Blank", pos, true).GetComponent<EffectController> ());
		}

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Point Blank"));

		int count = 0;

		while (map.UnitCanEnterTile(currX, currY) && count < combo) {
			currX += dirX;
			currY += dirY;
			++count;
		}

		currX -= dirX;
		currY -= dirY;

		if (currX != myCaster.tileX || currY != myCaster.tileY) {
			myCaster.SlideToTile (currX, currY);
		}

		//cant dodge or block this finisher
		target.TakeDamage (dmg * combo, null, false, myCaster);


	}

	public override void UpdateAbility ()
	{
		if (!myCaster.moving) {
			AbilityFinished = true;
		}
	}
}


