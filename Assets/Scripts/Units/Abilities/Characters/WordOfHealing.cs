using UnityEngine;
using System;
public class WordOfHealing : Ability
{
	public WordOfHealing (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Word Of Healing";
		healing = 30;
		manaCost = 10;
		maxCooldown = 1;
		duration = 2;
		range = 4;
		area = AreaType.Single;
		targets = TargetType.Ally;
		stacks = 3;

	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		target.TakeHealing (healing);
		target.ApplyEffect (new DamageRecievedEffect ("Word of Healing", duration, -0.05f, stacks));

		Vector3 pos = map.TileCoordToWorldCoord (target.tileX, target.tileY);
		myVisualEffects.Add (effectLib.CreateVisualEffect ("Word of Healing", pos).GetComponent<EffectController> ());

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Word of Healing"));
	}
}



