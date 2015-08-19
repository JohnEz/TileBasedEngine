using UnityEngine;
using System.Collections.Generic;

public enum AreaType {
	Single,
	AOE,
	Line,
	Cone,
	Self,
	SelfAOE,
	Floor,
	LineAOE,
	All
}

public enum TargetType {
	Enemy,
	Ally,
	All
}

public class Ability{
	public string Name = "";
	public string description = "I was too lazy to write this ability description for some reason ¯|_(ツ)_|¯ ";
    public int spellID;

	public int damage = 0;
	public int healing = 0;
	public int duration = 0;
	public int maxCooldown = 1;
	public int manaCost = 0;
	public int manaGain = 0;
	public int shield = 0;
	public int cooldown;
	public AreaType area = AreaType.Single;
	public TargetType targets = TargetType.Enemy;
	public int range;
	public int AOERange = 2;
	public bool usesCombo = false;
	public bool canTargetSelf = true;

    public Unit myCaster = null;
	public Unit myTarget = null;

	public PrefabLibrary effectLib;
	public TileMap map;

	public bool AbilityFinished = true;

	public int stacks = 1;

	public List<EffectController> myVisualEffects = new List<EffectController> ();
	public List<ProjectileController> myProjectiles = new List<ProjectileController> ();

	//ai information
	public int AIPriority = 5;
	public bool AIRanged = false;
	public bool AISupportsAlly = false;



    public Ability(Unit u, TileMap m, PrefabLibrary el)
    {
        myCaster = u;
		effectLib = el;
		map = m;
    }

	public virtual void UseAbility(Unit target) {
		cooldown = maxCooldown;
		AbilityFinished = false;
		myTarget = target;

		myCaster.AddRemoveMana (-manaCost);
	}

	public virtual void AbilityOnHit() {

	}

	public virtual void UseAbility(Node n) {
		cooldown = maxCooldown;
		AbilityFinished = false;
		myCaster.AddRemoveMana (-manaCost);
		myVisualEffects = new List<EffectController> ();
	}

	public virtual void UpdateAbility() {
		AbilityFinished = true;

		foreach (EffectController ec in myVisualEffects) {
			if (!ec.animFinished) {
				AbilityFinished = false;
			}
		}

		List<ProjectileController> destroyProjectiles = new List<ProjectileController> ();
		
		foreach (ProjectileController proj in myProjectiles) {
			if (proj.reachedTarget) {
				
				AbilityOnHit();
				
				destroyProjectiles.Add(proj);
			} else {
				AbilityFinished = false;
			}
		}
		
		//remove finished projectiles
		foreach (ProjectileController proj in destroyProjectiles) {
			myProjectiles.Remove(proj);
			GameObject.Destroy(proj.gameObject, 0);
		}
	}

	public void ReduceCooldown(int i) {
		if (cooldown > 0) {
			cooldown -= i;
		}
	}
	
}
