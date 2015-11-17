using UnityEngine;
using System;
using System.Collections.Generic;

public class SpawnTotem : Ability
{
	public SpawnTotem(Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Spawn Totem";
		duration = 2;
		range = 4;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 4;
	

		AIPriority = 0;
	}
	
	public override void UseAbility (Node n)
	{
		base.UseAbility (n);

		List<EnemyClass> availableTotems = new List<EnemyClass> ();

		if (!myCaster.GetComponent<GoblinShamanAI> ().HasTotem("TotemEarth")) {
			availableTotems.Add(EnemyClass.EarthTotem);
		}

		if (!myCaster.GetComponent<GoblinShamanAI> ().HasTotem("TotemFire")) {
			availableTotems.Add(EnemyClass.FireTotem);
		}

		if (!myCaster.GetComponent<GoblinShamanAI> ().HasTotem("TotemWater")) {
			availableTotems.Add(EnemyClass.WaterTotem);
		}

		if (!myCaster.GetComponent<GoblinShamanAI> ().HasTotem("TotemWind")) {
			availableTotems.Add(EnemyClass.WindTotem);
		}

		int roll = UnityEngine.Random.Range (0, availableTotems.Count-1);

		myCaster.GetComponent<GoblinShamanAI>().totems.Add(myCaster.uManager.spawnEnemy (n.x, n.y, availableTotems [roll], true));
	}
	
	
}

