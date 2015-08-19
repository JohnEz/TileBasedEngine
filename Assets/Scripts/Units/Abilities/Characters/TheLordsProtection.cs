using UnityEngine;
using System;
public class TheLordsProtection : Ability
{
	float damageMod = 0.5f;
	public TheLordsProtection (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "The Lords Protection";
		healing = 10;
		manaCost = 30;
		maxCooldown = 15;
		duration = 2;
		area = AreaType.All;
		targets = TargetType.Ally;
		stacks = 1;

		int percentageMod = (int)(damageMod * 100);

		description = "Cooldown: " + maxCooldown.ToString () + " Mana: " + manaCost.ToString () +
			"\nReduces the damage all allies take by " + percentageMod.ToString () + 
				"% for " + duration.ToString() + " turns and heals them for " + healing.ToString() + " health.";
	}

	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		//loop through all the targetable nodes
		foreach (Node t in n.reachableNodes) {
			t.myUnit.ApplyEffect(new DamageRecievedEffect("The Lords Protection", duration, -damageMod, 1));
			t.myUnit.TakeHealing(healing);

			Vector3 pos = map.TileCoordToWorldCoord (t.x, t.y);
			myVisualEffects.Add (effectLib.CreateVisualEffect ("The Lords Protection", pos).GetComponent<EffectController> ());
		}

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Word of Healing"));
	}
	
}



