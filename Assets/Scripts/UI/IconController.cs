using UnityEngine;
using System.Collections;

public class IconController : MonoBehaviour {

	public int slot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//when the icon is clicked
	void OnMouseUp() {
		GetComponentInParent<UnitManager>().GetComponent<UnitManager> ().ShowAbility(slot);
	}
}
