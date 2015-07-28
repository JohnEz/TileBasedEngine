using UnityEngine;
using System;

public enum TriggerType {
	Death,
	Hit,
	Healed,
	Dodge,
	Use_Ability,
	Move,
	Floor
}

public class Trigger
{
	public TriggerType myTrigger;
	public int maxTriggers = 1;
	public int triggerCount = 0;
	public Unit myCaster = null;
	public int maxDuration = 1;
	public int duration = 0;
	public TargetType myTargets = TargetType.All;


	public Trigger (TriggerType tt, Unit caster)
	{
		myTrigger = tt;
		myCaster = caster;
	}

	public void CheckTrigger(TriggerType tt, Unit host) {
		// if the trigger type is same as mine
		if (myTrigger == tt && triggerCount < maxTriggers) {
			//TRIGGERED O.O
			RunTrigger(host);
		}
	}

	public virtual void RunTrigger(Unit host) {
		++triggerCount;
	}
}


