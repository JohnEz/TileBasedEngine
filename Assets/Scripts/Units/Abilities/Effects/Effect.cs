using UnityEngine;
using System.Collections.Generic;


public class Effect
{

    int maxDuration = 0;
    public int duration = 0;
    public int maxStack = 1;
	public int stack = 1;
    public string name = "NAME THIS EFFECT SILLY";
	public string description = "description";

    public Effect(string n, int dur, int mStack)
    {
        name = n;
        maxDuration = dur;
        duration = dur;
		maxStack = mStack;
    }

	public virtual void RunEffect(Unit u, bool reapply = false) {
        if (!reapply) {
			--duration;
		}
    }

    public void RefreshEffect()
    {
        duration = maxDuration;
    }

    public void RefreshEffect(int dur)
    {
        if (dur > maxDuration)
        {
            maxDuration = dur;
        }
        duration = dur;
    }

	public virtual void AddStack() {
		RefreshEffect ();
	}

}

