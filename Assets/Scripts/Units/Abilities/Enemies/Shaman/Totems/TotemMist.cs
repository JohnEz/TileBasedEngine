using UnityEngine;
using System.Collections.Generic;

public class TotemMist : Ability
{
	int losMod = 4;
	public TotemMist(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Smoke Bomb";
		damage = 0;
		duration = 1;
		range = 1;
		area = AreaType.Self;
		targets = TargetType.Ally;
		maxCooldown = 4;

		AIPriority = 100;
		AIRanged = true;
		AISupportsAlly = true;
	}
	
	public override void UseAbility(Node target)
	{
		base.UseAbility(target);

		foreach (Node t in target.reachableNodes) {

			//create the smoke effect
			Vector3 pos = map.TileCoordToWorldCoord (t.x, t.y);
			t.myUnit.ApplyEffect(new DestroyGOEffect("Destroy Smoke", duration, effectLib.CreateParticleEffect ("Smoke Bomb", pos)));

			List<Node> targetNodes = new List<Node> ();

			targetNodes.Add (t);
		
			foreach (Node n in t.neighbours) {
				targetNodes.Add (n);
			}
		
			targetNodes.Add (map.GetNode (t.x + 1, t.y + 1));
			targetNodes.Add (map.GetNode (t.x - 1, t.y + 1));
			targetNodes.Add (map.GetNode (t.x + 1, t.y - 1));
			targetNodes.Add (map.GetNode (t.x - 1, t.y - 1));
		
			//change their los and apply the effect
			foreach (Node n in targetNodes) {
				n.LOSMod += losMod;
			}
		
			t.myUnit.ApplyEffect (new SmokeBombTileEffect ("Mist", duration, targetNodes, losMod));
		
			t.myUnit.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Smoke Bomb"));
		}
		
	}
	
	
}
