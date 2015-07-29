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
	All
}

public enum TargetType {
	Enemy,
	Ally,
	All
}

public class Ability{
	public string Name;
    public int spellID;

	public int damage = 0;
	public int healing = 0;
	public int duration = 0;
	public int maxCooldown = 1;
	public int manaCost = 0;
	public int shield = 0;
	int cooldown;
	public AreaType area = AreaType.Single;
	public TargetType targets = TargetType.Enemy;
	public int range;
	public int AOERange = 2;

    public Unit myCaster = null;
	public Unit myTarget = null;

	public VisualEffectLibrary effectLib;
	public TileMap map;

	public bool AbilityFinished = true;

	public int stacks = 1;

	public List<EffectController> myVisualEffects = new List<EffectController> ();

    public Ability(Unit u, TileMap m, VisualEffectLibrary el)
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
	}

	public void ReduceCooldown(int i) {
		if (cooldown > 0) {
			cooldown -= i;
		}
	}
	
}
