using UnityEngine;
using System.Collections.Generic;

public class DestroyGOEffect : Effect
{
	GameObject myGameObject;
	
	public DestroyGOEffect(string n, int dur, GameObject go, int stacks = 1, Sprite icon = null)
		: base(n, dur, 1, icon)
	{
		myGameObject = go;
		description = "Destroys a game object";
		visible = false;
	}
	
	public override void OnExpire ()
	{
		base.OnExpire ();
		GameObject.Destroy (myGameObject, 0);
	}
}

