using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public bool pausedGame = false;
    public bool showMenu = false;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void TogglePause()
    {
        pausedGame = !pausedGame;

        if (pausedGame)
        {
            Time.timeScale = 0;
            pausedGame = true;
            showMenu = true;
        } else
        {
            Time.timeScale = 1;
            pausedGame = false;
            showMenu = false;
        }

        if (showMenu)
        {
            gameObject.SetActive(true);
        } else
        {
            gameObject.SetActive(false);
        }
    }

    public void LoadMainMenu()
    {
        Application.LoadLevel("_MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
