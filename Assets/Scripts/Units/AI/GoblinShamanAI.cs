using UnityEngine;
using System;
using System.Collections.Generic;

public class GoblinShamanAI : AIBehaviours
{
	public List<Unit> totems = new List<Unit>();

	public GoblinShamanAI () : base()
	{

	}

	public bool HasTotem(string n) {
		foreach (Unit u in totems) {
			if (u.name.Contains (n) && !u.isDead) {
				return true;
			}
		}

		return false;
	}

	public override void FSM ()
	{
		bool has4Totems = false;

		if (myUnit.actionPoints < 1 && myUnit.remainingMove < 1) {
			turnPlanned = true;
			return;
		}

		if (HasTotem ("TotemEarth") && HasTotem ("TotemFire") && HasTotem ("TotemWater") && HasTotem ("TotemWind")) {
			has4Totems = true;
		}

		if (CanUseAbility (myUnit.myAbilities [3]) && !has4Totems) {

			List<Node> targetable = myMap.FindSingleRangedTargets(myUnit.myAbilities [3], myUnit, false);
			List<Node> removeNodes = new List<Node>();

			//remove nodes that have units
			foreach(Node n in targetable) {
				if (n.myUnit != null) {
					removeNodes.Add(n);
				}
			}

			foreach (Node n in removeNodes) {
				targetable.Remove(n);
			}

			if (targetable.Count < 1) {
				base.FSM ();
				return;
			}

			int roll = UnityEngine.Random.Range (0, targetable.Count - 1);
			selectedAbility = 3;

			myUnit.currentPath = new List<Node>();

			target = targetable [roll];

			turnPlanned = true;

			DumbRanged ();

		} else {
			base.FSM ();
		}
	}
}


