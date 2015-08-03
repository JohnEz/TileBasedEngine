using UnityEngine;
using System;
public class ManaLeachEffect : Effect
{

	Unit myCaster = null;
	int manaGive = 0;
	int manaTake = 0;


	public ManaLeachEffect (string n, int dur, Unit caster, int mGive, int mTake) : base(n, dur, 1)
	{
		myCaster = caster;
		manaGive = mGive;
		manaTake = mTake;
	}

	public override void RunEffect (Unit u, bool reapply = false)
	{
		base.RunEffect (u, reapply);

		if (!reapply) {
			u.AddRemoveMana (-manaTake);
			myCaster.AddRemoveMana (manaGive);
		}
	}

}


