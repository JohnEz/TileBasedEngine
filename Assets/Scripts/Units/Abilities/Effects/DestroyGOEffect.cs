using UnityEngine;
using System.Collections.Generic;

public class DestroyGOEffect : Effect
{
	GameObject myGameObject;
	
	public DestroyGOEffect(string n, int dur, GameObject go, int stacks = 1)
		: base(n, dur, 1)
	{
		myGameObject = go;
	}
	
	public override void OnExpire ()
	{
		base.OnExpire ();
		GameObject.Destroy (myGameObject, 0);
	}
}

