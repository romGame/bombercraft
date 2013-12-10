using UnityEngine;
using System.Collections;

/* Auteur : Romain SPENATO */

public class victoryScript : MonoBehaviour {
	
	public bool isVictorious = false;
	
	/* NE RENTRE JAMAIS DEDANS AU CONTACT DU DRAPEAU */
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
