using UnityEngine;
using System.Collections.Generic;

public class Dot : Effect
{
	int baseDamage = 0;
    int damage = 0;

    public Dot(string n, int dur, int dmg, int stacks = 1)
        : base(n, dur, stacks)
    {
		baseDamage = dmg;
        damage = dmg;
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
	}
}

