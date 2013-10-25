using UnityEngine;
using System.Collections;

public class DestructionCaisseScript : MonoBehaviour
{



    #region variable

    private bool _estTermine = false;
    private bool _estRamasse = false;
    private CaisseManager _CaisseManager;

    #endregion


    // Use this for initialization
	void Start () {
        _CaisseManager = CaisseManager.GetCaisseManagerInstance();
	}
	
	// Update is called once per frame
	void Update () {
	    
        
	}


    void OnCollisionEnter(Collision collision)
    {
        
        if ( _estTermine && !_estRamasse && collision.gameObject.layer == LayerMask.NameToLayer("Joueur") )//Ramassage de la caisse 
        {
            _CaisseManager.AddCaisse(this.gameObject);
            _estRamasse = true;
        }

    }


    public void FaireExploser()
    {
        if (!_estTermine)//Destruction de la caisse 
        {
            //Permet de savoir si il faut jouer l'animation de destruction de la caisse ou bien de la faire disparaitre pour le donner au joueur
            this.animation.PlayQueued("DestructionCubeAnimation");
            _estTermine = true;
        }
    }


    /// <summary>
    /// Permet de remettre des propriété du script à zéro 
    /// </summary>
    public void InitProperty()
    {
        _estRamasse = false;
        _estTermine = false;
    }

}
