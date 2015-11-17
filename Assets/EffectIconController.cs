using UnityEngine;
using System.Collections;

public class EffectIconController : MonoBehaviour {

	public Effect myEffect = null;
	public bool showingMyEffect = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTooltip() {
		if (!showingMyEffect) {
			GetComponentInParent<UIManager> ().ShowTooltip (this);
			showingMyEffect = true;
		}
	}

	public void HideTooltip() {
		if (showingMyEffect) {
			GetComponentInParent<UIManager> ().HideTooltip ();
			showingMyEffect = false;
		}
	}

	public string GetEffectName() {
		return myEffect.name;
	}

	public string GetEffectDesc() {
		return myEffect.description;
	}

	public int GetEffectDuration() {
		return myEffect.duration;
	}

	public int GetEffectStacks() {
		return myEffect.stack;
	}
}
