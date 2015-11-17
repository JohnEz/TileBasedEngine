using UnityEngine;
using System.Collections.Generic;

public class Dot : Effect
{
	int baseDamage = 0;
    int damage = 0;

	public Dot(string n, int dur, int dmg, int stacks = 1, Sprite icon = null)
        : base(n, dur, stacks, icon)
    {
		baseDamage = dmg;
        damage = dmg;
		description = "Deals " + damage.ToString() + " damage each turn.";
    }

	public override void RunEffect(Unit u, bool reapply = false)
    {
 	    base.RunEffect(u, reapply);

        //deal damage 
		if (!reapply) {
			u.TakeDamage (damage, null, false);
		}
    }

	public override void AddStack ()
	{
		base.AddStack ();
		if (stack < maxStack) {
			++stack;
			damage += baseDamage;
		}
		description = "Deals " + damage.ToString() + " damage each turn.";
	}
}

