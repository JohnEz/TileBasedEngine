using UnityEngine;
using System;

public class HammerSlam : Ability
{
	public HammerSlam(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Hammer Slam";
		damage = 50;
		duration = 2;
		range = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 3;
		
		AIPriority = 10;
	}
	
	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		

		int dmg = (int)(damage * myCaster.damageDealtMod);

		Vector3 pos = map.TileCoordToWorldCoord (target.x, target.y);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Hit1", pos).GetComponent<EffectController> ());


		//deal damage, if not dodged, apply effect
		if (target.myUnit.TakeDamage(dmg, effectLib.getSoundEffect ("Blunt2"), true, myCaster) != -1) {
			target.myUnit.ApplyEffect(new Stun("Hammer Slammed", duration));
			target.myUnit.ShowCombatText ("Stunned", target.myUnit.statusCombatText);
			target.myUnit.ApplyEffect(new DamageRecievedEffect("Broken Armour", duration, 0.5f));
		}
		
	}
	
	
}

