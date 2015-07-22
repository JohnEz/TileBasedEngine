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

[System.Serializable]
public class Ability{
	public string Name;
	public int damage = 0;
	public int healing = 0;
	public int duration = 0;
	public int maxCooldown = 1;
	int cooldown;
	public AreaType area = AreaType.Single;
	public TargetType targets = TargetType.Enemy;
	public int range;
	public int AOERange = 2;
	//effect - this will be another class and used for buffs / debuffs

	public void UseAbility(Unit target) {
		float dmgmod = 1 - ((float)target.damageReduction / 100);
		int dmg = (int)(damage * dmgmod);
		target.HP -= dmg;
		target.ShowDamage (dmg, target.gameObject.transform.localPosition.x, target.gameObject.transform.localPosition.y);
		target.HP += healing;
		cooldown = maxCooldown;


	}

	public void UseAbility(List<Node> targetSquares) {
		foreach (Node n in targetSquares) {
			if (n.myUnit != null) {
				UseAbility(n.myUnit);
			}
		}
	}

	public void ReduceCooldown(int i) {
		if (cooldown > 0) {
			cooldown -= i;
		}
	}
	
}
