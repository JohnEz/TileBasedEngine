using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void StartGame()
    {
        // should be changed to the in game menu
        Application.LoadLevel("_Level");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
