using UnityEngine;
using System;
public class TheLordsProtection : Ability
{
	public TheLordsProtection (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "The Lords Protection";
		healing = 10;
		manaCost = 25;
		maxCooldown = 10;
		duration = 2;
		area = AreaType.All;
		targets = TargetType.Ally;
		stacks = 1;
	}

	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		//loop through all the targetable nodes
		foreach (Node t in n.reachableNodes) {
			t.myUnit.ApplyEffect(new DamageRecievedEffect("The Lords Protection", duration, -0.5f, 1));
			t.myUnit.TakeHealing(healing);

			Vector3 pos = map.TileCoordToWorldCoord (t.x, t.y);
			myVisualEffects.Add (effectLib.CreateVisualEffect ("The Lords Protection", pos).GetComponent<EffectController> ());
		}

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Word of Healing"));
	}
	
}



