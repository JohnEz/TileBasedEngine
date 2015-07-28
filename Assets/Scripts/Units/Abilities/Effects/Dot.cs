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

    public override void RunEffect(Unit u)
    {
 	    base.RunEffect(u);
        //deal damage 
        u.TakeDamage(damage);
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

