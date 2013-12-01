using UnityEngine;
using System.Collections;

public class mainMenuOptionsScript : MonoBehaviour {
	
	// booleens de validation de sélection
	private bool playIsSelected = false;
	private bool exitIsSelected = false;
	
	// Vector3 représentant la position de la flèche du menu en fonction du choix sélectionné.
	private Vector3 arrowOnPlay = new Vector3(0.68f, 0.26f, 3.5f);
	private Vector3 arrowOnExit = new Vector3(0.68f, 0.10f, 3.5f);
	
	// Couleurs de Sélection/Deselection (Blanc et Noir)
	private Color colorSelected = new Color(255,255,255);
	private Color colorNotSelected = new Color(0,0,0);
	
	// Valeurs que l'on aura besoin de manipuler
	[SerializeField]
		private Transform arrowY;
	
	[SerializeField]
		private TextMesh playOptionText;
	
	[SerializeField]
		private TextMesh exitOptionText;

	// Use this for initialization
	void Start () {
		// Au début, on choisit l'option "PLay" par défault.
		playIsSelected = true;
		optionSelection(1);
	}
	
	// Update is called once per frame
	void Update () {
		// Le joueur sélectionne l'option "PLAY"
		if (Input.GetKeyDown(KeyCode.UpArrow) && !playIsSelected)
        {
			playIsSelected = true;
			exitIsSelected = false;
			optionSelection(1);
		}
		
		// Le joueur sélectionne l'option "EXIT"
		if (Input.GetKeyDown(KeyCode.DownArrow) && !exitIsSelected)
        {
			exitIsSelected = true;
			playIsSelected = false;
			optionSelection(2);
		}
		
		// Le joueur appuie sur la touche 'ENTER'
		if (Input.GetKeyDown(KeyCode.Return))
        {
			if(playIsSelected) {
				Application.LoadLevel("Menu");
			}
			else {
				Application.Quit();
			}
		}
	}
	
	// Fonction utilisée pour mettre à jour l'affichage en fonction de l'option choisie
	void optionSelection(int id) {
		switch( id ) {
			case 1 :
				// On déplace la flèche en face de l'option en utilisant le bon Vector3
				arrowY.position = arrowOnPlay;
			
				// On met à jour les couleurs de sélections
				playOptionText.color = colorSelected;
				exitOptionText.color = colorNotSelected;
			break;
			case 2 :
				// On déplace la flèche en face de l'option en utilisant le bon Vector3
				arrowY.position = arrowOnExit;
			
				// On met à jour les couleurs de sélections
				exitOptionText.color = colorSelected;
				playOptionText.color = colorNotSelected;
			break;
		}
	}
}
