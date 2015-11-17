using UnityEngine;
using System.Collections.Generic;

public class SmokeBomb : Ability
{
	int losMod = 1;
	int dodgeMod = 25;
	public SmokeBomb(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Smoke Bomb";
		damage = 0;
		duration = 3;
		range = 1;
		area = AreaType.Self;
		targets = TargetType.Ally;
		maxCooldown = 10;

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDrops a smoke bomb that slightly reduces vision in the area and increase the dodge chance of allies in it by " + dodgeMod.ToString () + "% lasting " + duration.ToString() + " turns.";
	}
	
	public override void UseAbility(Node target)
	{
		base.UseAbility(target);

		List<Node> targetNodes = new List<Node> ();

		//create the smoke effect
		Vector3 pos = map.TileCoordToWorldCoord (myCaster.tileX, myCaster.tileY);
		myCaster.ApplyEffect(new DestroyGOEffect("Destroy Smoke", duration, effectLib.CreateParticleEffect ("Smoke Bomb", pos)));

		targetNodes.Add (target);

		foreach (Node n in target.neighbours) {
			targetNodes.Add (n);
		}

		targetNodes.Add (map.GetNode (target.x + 1, target.y + 1));
		targetNodes.Add (map.GetNode (target.x - 1, target.y + 1));
		targetNodes.Add (map.GetNode (target.x + 1, target.y - 1));
		targetNodes.Add (map.GetNode (target.x - 1, target.y - 1));

		//change their los and apply the effect
		foreach (Node n in targetNodes) {
			n.LOSMod += losMod;

			Effect eff = new DodgeEffect("Smokey", 0, dodgeMod, 1, effectLib.getIcon("Smoke Bomb").sprite);
			eff.targets = myCaster.team;

			n.myEffects.Add(eff); 

			if (n.myUnit && n.myUnit.team == myCaster.team) {
				n.myUnit.ApplyEffect(eff);
			}
		}

		myCaster.ApplyEffect (new SmokeBombTileEffect ("Smoke Bomb", duration, targetNodes, losMod, 1));

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Smoke Bomb"));

	}
	
	
}
