using UnityEngine;
using System.Collections.Generic;

public enum AreaType {
	Single,
	SquareAOE,
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
	public int damage = 0;
	public int healing = 0;
	public int duration = 0;
	public int maxCooldown = 1;
	int cooldown;
	public AreaType area = AreaType.Single;
	public TargetType targets = TargetType.Enemy;
	public int range;
	//effect - this will be another class and used for buffs / debuffs

	public void UseAbility(Unit target) {
		target.HP -= damage;
		target.HP += healing;
		cooldown = maxCooldown;
	}
	
}
