using UnityEngine;
using System.Collections;

public class DestructionCaisseScript : MonoBehaviour
{



    #region variable

    private bool _estTermine = false;
    private bool _estRamasse = false;

    //Singleton du Caisse manager
    private CaisseManager _CaisseManager;

    //Permet de reseter les caisse 
    private Vector3 _ResetScale = new Vector3(1, 1, 1);
    private Quaternion _ResetRotation = new Quaternion(0, 0, 0, 0);
    #endregion



	void Start () {
        _CaisseManager = CaisseManager.GetCaisseManagerInstance();
	}
	

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
    /// Permet de remettre des propriété du script à zéro et de placer la caisse à l'endroit voulu
    /// </summary>
    public void InitProperty(Vector3 Position)
    {
        _estRamasse = false;
        _estTermine = false;
        this.transform.position = Position;
        this.transform.localScale = _ResetScale;
        this.transform.localRotation = _ResetRotation;
    }

}
