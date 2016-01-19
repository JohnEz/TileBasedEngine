using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject UI;
    public GameObject PauseMenu;
    public Camera cam;

    // Use this for initialization
    void Start () {
		UI.GetComponent<UIManager>().Initialise ();
		GetComponent<PrefabLibrary> ().Initialise ();
		UI.GetComponent<UIManager>().prefabs = GetComponent<PrefabLibrary> ();
		GetComponent<TileMap> ().Initialise ();
		GetComponent<UnitManager> ().Initialise ();
		cam.GetComponent<CameraController> ().Initialise (GetComponent<TileMap> ());
	}

	void Update() {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.GetComponent<PauseMenu>().TogglePause();
        }

        if (!PauseMenu.GetComponent<PauseMenu>().pausedGame)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GetComponent<UnitManager>().ShowAbility(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GetComponent<UnitManager>().ShowAbility(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GetComponent<UnitManager>().ShowAbility(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                GetComponent<UnitManager>().ShowAbility(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                GetComponent<UnitManager>().ShowAbility(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                GetComponent<UnitManager>().ShowAbility(5);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape))
            {
                //Show movement
                GetComponent<UnitManager>().ShowMovement();
                //unhighlight icons
                GetComponentInChildren<UIManager>().HighlightIcon(-1);
            }
        }


    }
}
