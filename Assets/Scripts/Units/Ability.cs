using UnityEngine;
using System.Collections;

public enum AreaType {
	Single,
	SquareAOE,
	Line,
	Cone,
	Self,
	All
}

public enum TargetType {
	Enemy,
	Ally,
	All
}

public class Ability{
	int damage = 0;
	int healing = 0;
	int duration = 0;
	int cooldown = 1;
	AreaType area = AreaType.Single;
	TargetType target = TargetType.Enemy;
	//effect - this will be another class and used for buffs / debuffs


}
