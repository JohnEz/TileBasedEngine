using UnityEngine;
using System.Collections.Generic;

public class CripplingStrike : Ability
{

    public CripplingStrike(Unit u)
        : base(u)
    {
        damage = 30;
        duration = 2;
        range = 1;
        area = AreaType.Single;
        targets = TargetType.Enemy;
        maxCooldown = 1;
    }

    public override void UseAbility(Unit target, TileMap map)
    {
        base.UseAbility(target, map);
        int dmg = (int)(damage * myCaster.damageDealtMod);

        target.TakeDamage(dmg);
        target.ApplyEffect(new DamageDealtEffect("Cripple", duration, -0.1f));
    }


}
