using UnityEngine;
using System;

public class ShieldEffect : Effect
{
	int maxShield;
	int shieldAmount;

	public ShieldEffect (string n, int dur, int shield) : base(n, dur, 1)
	{
		maxShield = shield;
		shieldAmount = maxShield;
		description = "Damage Shield";
	}

	public override void RunEffect(Unit u, bool reapply = false)
	{
		base.RunEffect(u, reapply);

		//give shield
		u.shield += shieldAmount;
	}

	public int TakeDamage(int d) {
		int dmg = d - shieldAmount;
		shieldAmount -= d;

		return dmg;
	}

	public override void AddStack ()
	{
		base.AddStack ();
		shieldAmount = maxShield;
	}

	public bool ShieldIsDestroyed() {
		if (shieldAmount <= 0) {
			return true;
		}
		return false;
	}
}


