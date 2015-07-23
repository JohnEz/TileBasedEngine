using UnityEngine;
using System.Collections.Generic;

public enum AreaType {
	Single,
	AOE,
	Line,
	Cone,
	Self,
	SelfAOE,
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
	public int manaCost;
	public int shield = 0;
	int cooldown;
	public AreaType area = AreaType.Single;
	public TargetType targets = TargetType.Enemy;
	public int range;
	public int AOERange = 2;

    public Unit myCaster = null;
	public Unit myTarget = null;

	public bool AbilityFinished = true;

    public Ability(Unit u)
    {
        myCaster = u;
    }

	public virtual void UseAbility(Unit target, TileMap map) {
		cooldown = maxCooldown;
		AbilityFinished = false;
		myTarget = target;

	}

	public virtual void UseAbility(List<Node> targetSquares, TileMap map) {
		/*foreach (Node n in targetSquares) {
			if (n.myUnit != null) {
				UseAbility(n.myUnit);
			}
		}*/
		cooldown = maxCooldown;
		AbilityFinished = false;
	}

	public virtual void UpdateAbility() {
		AbilityFinished = true;
	}

	public void ReduceCooldown(int i) {
		if (cooldown > 0) {
			cooldown -= i;
		}
	}
	
}
