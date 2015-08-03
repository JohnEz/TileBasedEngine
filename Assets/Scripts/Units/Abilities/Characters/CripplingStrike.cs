using UnityEngine;
using System.Collections.Generic;

public class CripplingStrike : Ability
{

	public CripplingStrike(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
    {
        damage = 30;
        duration = 2;
        range = 1;
        area = AreaType.Single;
        targets = TargetType.Enemy;
        maxCooldown = 1;
    }

    public override void UseAbility(Unit target)
    {
        base.UseAbility(target);
        int dmg = (int)(damage * myCaster.damageDealtMod);

        target.TakeDamage(dmg);
        target.ApplyEffect(new DamageDealtEffect("Cripple", duration, -0.1f));

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Crippling Strike"));
    }


}
