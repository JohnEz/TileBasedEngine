using UnityEngine;
using System;

public class TripleShot : Ability
{
	bool firstHit = true;

	public TripleShot (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		damage = 10;
		range = 7;
		maxCooldown = 1;
		area = AreaType.Single;
		targets = TargetType.Enemy;
	}

	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);
		firstHit = true;

		myTarget = target;

		Vector3 offset = new Vector3 (0.25f, 0.25f, 0);

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("TripleShot"));
		myProjectiles.Add(effectLib.CreateProjectile("Arrow1", myCaster.transform.position, target.transform.position, 10).GetComponent<ProjectileController>());
		myProjectiles.Add(effectLib.CreateProjectile("Arrow1", myCaster.transform.position + offset, target.transform.position, 9.5f).GetComponent<ProjectileController>());
		myProjectiles.Add(effectLib.CreateProjectile("Arrow1", myCaster.transform.position - offset, target.transform.position, 9).GetComponent<ProjectileController>());
	
	}

	public override void AbilityOnHit ()
	{
		base.AbilityOnHit ();

		int dmg = (int)(damage * myCaster.damageDealtMod);

		myCaster.AddRemoveMana (5);
		
		myTarget.TakeDamage (dmg);
		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("TripleShot Hit"));

		if (firstHit) {
			Vector3 pos = map.TileCoordToWorldCoord (myTarget.tileX, myTarget.tileY);
			myVisualEffects.Add (effectLib.CreateVisualEffect ("Triple Shot", pos).GetComponent<EffectController> ());

			firstHit = false;
		}
	}
}


