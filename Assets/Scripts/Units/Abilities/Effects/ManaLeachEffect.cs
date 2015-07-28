using UnityEngine;
using System;
public class ManaLeachEffect : Effect
{

	Unit myCaster = null;
	int manaGive = 0;
	int manaTake = 0;


	public ManaLeachEffect (string n, int dur, Unit caster, int mGive, int gTake) : base(n, dur, 1)
	{
		myCaster = caster;
		manaGive = mGive;
		manaTake = manaTake;
	}

	public override void RunEffect (Unit u)
	{
		base.RunEffect (u);

		u.AddRemoveMana (-manaTake);
		myCaster.AddRemoveMana (manaGive);
	}

}


