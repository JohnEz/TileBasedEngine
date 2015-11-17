using UnityEngine;
using System;

public class FlamingAxe : Ability
{
	int dotDamage = 5;

	public FlamingAxe(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Flaming Axe";
		damage = 50;
		duration = 1;
		range = 4;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 3;
		stacks = 9;

		AIPriority = 15;
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		
		
		int dmg = (int)(damage * myCaster.damageDealtMod);

		Vector3 pos = map.TileCoordToWorldCoord (target.x, target.y);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Slash1", pos).GetComponent<EffectController> ());

		//deal damage, if not dodged, apply effect
		if (target.myUnit.TakeDamage (dmg, effectLib.getSoundEffect ("Fireball Hit"), true, myCaster) != -1) {
			target.myUnit.ApplyEffect (new Dot ("Potent Fire", duration, dotDamage, stacks));
			target.myUnit.ShowCombatText ("Burning", target.myUnit.statusCombatText);
		}


		
	}
	
	
}

