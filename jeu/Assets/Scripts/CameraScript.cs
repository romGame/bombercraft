using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {


    private bool wasLocked = false;

    public bool WasLocked
    {
        get { return wasLocked; }
        set { wasLocked = value; }
    }

	// Use this for initialization
	void Start () {
        Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () {
        //Pour bloquer la souris 
        if (!Screen.lockCursor && wasLocked)
        {
            wasLocked = false;
            DidUnlockCursor();
        }
        else if (Screen.lockCursor && !wasLocked)
        {
            wasLocked = true;
            DidLockCursor();
        }
	}


    #region Blocage de la souris
    void DidLockCursor()
    {
        guiTexture.enabled = false;
    }
    void DidUnlockCursor()
    {

        guiTexture.enabled = true;
    }

    void OnMouseDown()
    {
        Screen.lockCursor = true;
    }
    #endregion

}
