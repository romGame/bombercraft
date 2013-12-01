using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoueurLogic : MonoBehaviour
{

    #region variable

    #region Paramétrable
    [SerializeField]
    private float _VitesseDeplacement = 3f;
    [SerializeField]
    private float _VitesseSaut = 5f;
    [SerializeField]
    private float _VitesseRotationCamera = 150f;
    [SerializeField]
    private float _DelaisRafreshServeur = 0.05f;

    //Permet de créer un copie de cet objet quand un joueur se connecte au server 
    //[SerializeField]
    //private static GameObject _joueurskin = null;


    
    #endregion

    #region Vecteurs d'initialisations
    private Vector3 _MoveDirection = Vector3.zero;
    private Vector3 _AxeRotationJoueur = new Vector3(0, 1, 0);
    #endregion


    //Le joueur
    private Transform _Controller = null;
    //Le viseur du joueur
    private Transform _Viseur = null;
    private Rigidbody _Personnage = null;
    //Pour la gestion des caisses
    //public CaisseManager _CaisseManager;
    //Pour la gestion des bombes
    //public BombManager _BombManager;
    //Pour la synchronisazion des joueurs 
    public NetworkView _NetworkView;
    public NetworkManager _NetworkManager;

    public NetworkViewID IdJoueur;

    //Gestion des sauts
    private float _directionalJumpFactor = 0.7f;
    private bool _isJumping = false ;

    public bool isLocalPlayer = false;
    public NetworkPlayer netPlayer;


    private float _lastTimeSave;

    //L'animation de déplacement du joueur
    private Animation _DeplacementAnimation = null;

    #endregion

	void Start () {
        _Controller = GetComponent<Transform>();
        _Personnage = GetComponent<Rigidbody>();
        //_CaisseManager = CaisseManager.GetCaisseManagerInstance();
        //_BombManager = BombManager.GetBombManagerInstance();
        _NetworkView =  GetComponent<NetworkView>();
        _lastTimeSave = 0;
       


        //On cherche et récupère le viseur du joueur parmit tous les Transforms
        Transform[] listeTransform = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < listeTransform.Length; i++)
        {
            if (listeTransform[i].gameObject.layer == LayerMask.NameToLayer("ViseurJoueur"))
            {
                _Viseur = (Transform)listeTransform[i];
            }

            if (listeTransform[i].gameObject.layer == LayerMask.NameToLayer("JoueurGraphic"))
            {
                _DeplacementAnimation = ((Transform)listeTransform[i]).GetComponent<Animation>();
            }

        }


	}

    //Permet de gérer le faite que le personnage soit le joueur actuel ou bien un joueur autre
    public void InitialisePlayer()
    {
        if (!isLocalPlayer)
        {
            //On désactive les caméras si ce n'est pas le joueur
            Camera[] TabCamera = this.GetComponents<Camera>();
            foreach (Camera cam in TabCamera)
            {
                cam.enabled = false;
            }
        }
    }

	void Update () {

        #region Gestion des entrées utilisateur
        if (isLocalPlayer)
        {

            
            
        }
        #endregion
    }

    void FixedUpdate()
    {

        if (isLocalPlayer)
        {

            //Déplacement du joueur
            _MoveDirection.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            //Pour jouer les animations de déplacement
            if (_MoveDirection != Vector3.zero && !_DeplacementAnimation.isPlaying)
            {
                _DeplacementAnimation.PlayQueued("deplacement-creeper");
            }
            else if (_DeplacementAnimation.isPlaying && _MoveDirection == Vector3.zero)
            {
                _DeplacementAnimation.Stop();
            }
            
            //Faire déplacer le joueur
            _Controller.Translate(_MoveDirection * _VitesseDeplacement * Time.deltaTime);
            //Faire une rotation sur l'axe des Y du personnage en fonction du mouvement de la souris
            _Controller.Rotate(_AxeRotationJoueur, Input.GetAxis("Mouse X") * _VitesseRotationCamera * Time.deltaTime);
            

            //Gestion du saut
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!_isJumping)
                {
                    _Personnage.AddForce(_VitesseSaut * _Personnage.transform.up + _Personnage.velocity.normalized * _directionalJumpFactor, ForceMode.VelocityChange);
                    _isJumping = true;
                }
            }


            //Placement d'une caisse
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _NetworkView.RPC("CaissePoseJoueur",
                        RPCMode.Server,
                        IdJoueur,
                        _Viseur.position);

            }

            //Placement d'une bombe
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                _NetworkView.RPC("BombPoseJoueur",
                        RPCMode.Server,
                    IdJoueur);
            }

            if (Time.time - _lastTimeSave > _DelaisRafreshServeur)//En fonction du délais de rafraichissement
            {

                Debug.Log("RPC Translate " + _NetworkView.viewID);
                _lastTimeSave = Time.time;
                _NetworkView.RPC("TranslateJoueur",
                        RPCMode.Server,
                        IdJoueur,
                        transform.position,
                        transform.rotation);
            }

        }
    }



    #region RPC vide (nécéssaire pout fonctionner)
    /*[RPC]
    void DeplacementJoueur(NetworkViewID playerID, Vector3 translation)
    {
    }

    [RPC]
    void RotationJoueur(NetworkViewID playerID, Vector3 axe, float angle)
    {
    }*/

    [RPC]
    void TranslateJoueur(NetworkViewID playerID, Vector3 pos, Quaternion rot)
    {

        //_NetworkManager.TranslateJoueur(playerID, pos, rot);
    }

    [RPC]
    void CaissePoseJoueur(NetworkViewID playerID, Vector3 pos)
    {
    }

    [RPC]
    void BombPoseJoueur(NetworkViewID playerID)
    {
    }
    #endregion

    void OnCollisionEnter(Collision collision)
    {

        //Gestion du saut
        if (_isJumping && (collision.gameObject.layer == LayerMask.NameToLayer("Sol") || collision.gameObject.layer == LayerMask.NameToLayer("CaisseDestructible")))
        {
            _isJumping = false;
        }
    }

}
