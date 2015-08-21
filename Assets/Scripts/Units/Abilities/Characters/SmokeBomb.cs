using UnityEngine;
using System.Collections.Generic;

public class SmokeBomb : Ability
{
	int losMod = 4;
	float damageMod = 0.25f;
	public SmokeBomb(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Smoke Bomb";
		damage = 0;
		duration = 3;
		range = 1;
		area = AreaType.Self;
		targets = TargetType.Ally;
		maxCooldown = 10;

		int percentageMod = (int)(damageMod * 100);

		description = "Cooldown: " + maxCooldown.ToString () +
			"\nDrops a smoke bomb that reduces vision in the area and reduces the damage allies take by " + percentageMod.ToString () + "% lasting " + duration.ToString() + " turns.";
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

			Effect eff = new DamageRecievedEffect("Smokey", 0, -damageMod);
			eff.targets = myCaster.team;

			n.myEffects.Add(eff); 
		}

		myCaster.ApplyEffect (new SmokeBombTileEffect ("Smoke Bomb", duration, targetNodes, losMod));

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Smoke Bomb"));

	}
	
	
}
