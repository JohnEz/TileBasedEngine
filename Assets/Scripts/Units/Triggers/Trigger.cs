using UnityEngine;
using System;

public enum TriggerType {
	Death,
	Hit,
	Healed,
	Dodge,
	Block,
	Use_Ability,
	Move,
	Floor
}

public class Trigger
{
	public string triggerName;
	public TriggerType myTrigger;
	public int maxTriggers = 1;
	public int triggerCount = 0;
	public Unit myCaster = null;
	public int maxDuration = 1;
	public int duration = 0;
	public TargetType myTargets = TargetType.All;
	public PrefabLibrary effectLib = null;


	public Trigger (string name, TriggerType tt, Unit caster, PrefabLibrary el, int dur)
	{
		triggerName = name;
		myTrigger = tt;
		myCaster = caster;
		effectLib = el;
		maxDuration = dur;
		duration = maxDuration;
	}

	public void CheckTrigger(TriggerType tt, Unit host, Unit attacker = null) {
		// if the trigger type is same as mine
		if (myTrigger == tt && (triggerCount < maxTriggers || maxTriggers == -1)) {
			//TRIGGERED O.O
			RunTrigger(host, attacker);
		}
	}

	public virtual void RunTrigger(Unit host, Unit attacker = null) {
		++triggerCount;
	}
}


