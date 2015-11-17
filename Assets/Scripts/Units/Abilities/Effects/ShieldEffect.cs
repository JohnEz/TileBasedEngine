using UnityEngine;
using System;

public class ShieldEffect : Effect
{
	int maxShield;
	int shieldAmount;

	public ShieldEffect (string n, int dur, int shield, Sprite icon = null)
		: base(n, dur, 1, icon)
	{
		maxShield = shield;
		shieldAmount = maxShield;
		description = "Damage Shield " + shieldAmount.ToString();
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

		description = "Damage Shield " + shieldAmount.ToString();

		return dmg;
	}

	public override void AddStack ()
	{
		base.AddStack ();
		shieldAmount = maxShield;
		description = "Damage Shield " + shieldAmount.ToString();
	}

	public bool ShieldIsDestroyed() {
		if (shieldAmount <= 0) {
			return true;
		}
		return false;
	}
}


