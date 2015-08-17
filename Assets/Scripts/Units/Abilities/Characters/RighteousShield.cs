using UnityEngine;
using System;

public class RighteousShield : Ability
{
	int shieldDuration = 3;

	public RighteousShield (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Righteous Shield";
		shield = 50;
		manaCost = 20;
		maxCooldown = 2;
		duration = 3;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Ally;

		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nShields the target against " + shield.ToString () + 
				" points of damage and increases movespeed by 1 square, lasting " + duration.ToString() + " turns.";
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		target.ApplyEffect (new MovespeedMod ("Righteous Speed", duration, 1));
		target.ApplyEffect (new ShieldEffect ("Righteous Shield", shieldDuration, shield));

		target.ShowCombatText (shield.ToString (), target.statusCombatText);
		target.UpdateHealthBar ();

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Righteous Shield", pos).GetComponent<EffectController> ());

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Righteous Shield"));
	}
}


