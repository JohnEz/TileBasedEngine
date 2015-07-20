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
	public int cooldown = 1;
	public AreaType area = AreaType.Single;
	public TargetType target = TargetType.Enemy;
	public int range;
	//effect - this will be another class and used for buffs / debuffs
	
}
