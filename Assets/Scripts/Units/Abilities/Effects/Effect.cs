using UnityEngine;
using System.Collections.Generic;


public class Effect
{

    int maxDuration = 0;
    int duration = 0;
    public int stackable = 1;
    string name = "NAME THIS EFFECT SILLY";

    public Effect(string n, int dur)
    {
        name = n;
        maxDuration = dur;
        duration = dur;
    }

    public virtual void RunEffect(Unit u) {
        --duration;
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

}

