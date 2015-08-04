using UnityEngine;
using System;
public class ArcanePulse : Ability
{
	public ArcanePulse (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Arcane Pulse";
		manaCost = -20;
		maxCooldown = 1;
		damage = 25;
		range = 5;
		area = AreaType.Single;
		targets = TargetType.Enemy;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		
		int dmg = (int)(damage * myCaster.damageDealtMod);

		float mod = 1;

		// if its a mana user
		if (target.maxMana != 0) {
			//calculate the modifier
			mod = 2 - ((float)target.mana / (float)target.maxMana)*2;
		}
 
		dmg = (int)(dmg * mod);

		target.TakeDamage (dmg, effectLib.getSoundEffect ("Arcane Pulse"));
		
		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Arcane Pulse", pos).GetComponent<EffectController> ());
	}

}


