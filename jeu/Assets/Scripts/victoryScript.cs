using UnityEngine;
using System.Collections;

public class victoryScript : MonoBehaviour {
	
	public bool isVictorious = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter() {
        isVictorious = true;
		Debug.Log ("plop");
    }
	
	void OnGUI() {
		if(isVictorious) {
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 50, 20), "VICTORY !");
		}
	}
}
