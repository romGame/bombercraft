using UnityEngine;
using System.Collections;

/* Auteur : Romain SPENATO */

public class guiCountersScript : MonoBehaviour {
	
	// La texture du bloc
	[SerializeField]
		public Texture blocTexture;
	
	// La texture de la mort
	[SerializeField]
		public Texture blocDeath;
	
	// Les compteurs
	public string blocCounterText = "X ";
	public int nbBlocs = 0;
	
	public string blocDeathText = "X ";
	public int nbDeaths = 0;
	
	void OnGUI() {
		// Compteur de Blocs. (Texture + Label)
		GUI.DrawTexture(new Rect(5, 5, 60, 60), blocTexture, ScaleMode.ScaleToFit, true, 0.0f);
		GUI.Label(new Rect(75, 25, 50, 20), blocCounterText + nbBlocs);
			
		// Compteur de Morts. (Texture + Label)
		GUI.DrawTexture(new Rect(5, 85, 60, 60), blocDeath, ScaleMode.ScaleToFit, true, 0.0f);
		GUI.Label(new Rect(75, 105, 50, 20), blocDeathText + nbDeaths);
	}
}
