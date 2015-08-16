using UnityEngine;
using System;

public class DeepSlumber : Ability
{
	public DeepSlumber (Unit u, TileMap m, PrefabLibrary el) : base(u, m , el)
	{
		Name = "Deep Slumber";
		damage = 0;
		duration = 4;
		range = 6;
		area = AreaType.Single;
		targets = TargetType.Enemy;
		maxCooldown = 3;
	}
	
	public override void UseAbility (Unit target)
	{
		base.UseAbility (target);

		Effect eff1 = new Sleep ("Deep Slumber", duration);
		Effect eff2 = new DamageRecievedEffect ("Deep Slumber", duration, 0.25f); 

		target.ApplyEffect(eff1);
		target.ApplyEffect(eff2);

		target.AddTrigger (new RemoveEffect (myCaster, TriggerType.Hit, eff1, effectLib));
		target.AddTrigger (new RemoveEffect (myCaster, TriggerType.Hit, eff2, effectLib));

		myCaster.GetComponent<AudioSource> ().PlayOneShot (effectLib.getSoundEffect ("Deep Slumber"));
	}
}


