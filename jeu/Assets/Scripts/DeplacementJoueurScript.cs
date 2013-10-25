using UnityEngine;
using System.Collections;

public class DeplacementJoueurScript : MonoBehaviour
{

    #region variable

    #region Paramétrable
    [SerializeField]
    private float _VitesseDeplacement = 3f;
    public float VitesseDeplacement
    {
        get { return _VitesseDeplacement; }
        set { _VitesseDeplacement = value; }
    }

    [SerializeField]
    private float _Gravity = 20f;
    public float Gravity
    {
        get { return _Gravity; }
        set { _Gravity = value; }
    }

    [SerializeField]
    private float _JumpSpeed = 1f;
    public float JumpSpeed
    {
        get { return _JumpSpeed; }
        set { _JumpSpeed = value; }
    }

    [SerializeField]
    private float _VitesseRotationCamera = 150f;

    public float VitesseRotationCamera
    {
        get { return _VitesseRotationCamera; }
        set { _VitesseRotationCamera = value; }
    }


    #endregion


    #region Vecteurs d'initialisations
    private Vector3 _PositionPlacementCaisse = new Vector3(2, 0, 0);
    private Vector3 _ResetScale = new Vector3(1, 1, 1);
    private Quaternion _ResetRotation = new Quaternion(0, 0, 0, 0);
    private Vector3 _MoveDirection = Vector3.zero;
    private Vector3 _AxeRotationJoueur = new Vector3(0, 1, 0);
    #endregion


    private Transform _Controller = null;
    private Transform _Viseur = null;
    private Rigidbody _Personnage = null;
    private CaisseManager _CaisseManager;
    private BombManager _BombManager;
    private float _directionalJumpFactor = 0.7f;
    private bool _isJumping = false ;


    #endregion

    // Use this for initialization
	void Start () {
        //_Controller = (CharacterController)CharacterController.FindObjectOfType(typeof(CharacterController));
        _Controller = GetComponent<Transform>();
        _Personnage = GetComponent<Rigidbody>();
        _CaisseManager = CaisseManager.GetCaisseManagerInstance();
        _BombManager = BombManager.GetBombManagerInstance();

        //On récupère le viseur du joueur 
        Transform[] listeTransform = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < listeTransform.Length; i++)
        {
            if (listeTransform[i].gameObject.layer == LayerMask.NameToLayer("ViseurJoueur"))
            {
                _Viseur = (Transform)listeTransform[i];
                break;
            }

        }

	}
	
	// Update is called once per frame
	void Update () {


        _MoveDirection.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //_MoveDirection = transform.TransformDirection(_MoveDirection) * _VitesseDeplacement * Time.deltaTime;
        _Controller.Translate(_MoveDirection * _VitesseDeplacement * Time.deltaTime);
        //Faire une rotation sur l'axe des Y du personne en fonction du mouvement de la souris
        _Controller.Rotate(_AxeRotationJoueur, Input.GetAxis("Mouse X") * _VitesseRotationCamera * Time.deltaTime);
        

        //Placement d'une caisse
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject caisse = _CaisseManager.GetCaisse();
            if (caisse != null)
            {
                caisse.transform.localScale = _ResetScale;
                caisse.transform.localRotation = _ResetRotation;
                caisse.transform.position = _Viseur.position /* _Controller.position + _PositionPlacementCaisse*/;
            }
        }

        //Placement d'une bombe
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            GameObject bomb = _BombManager.GetBomb();
            
            if (bomb != null)
            {
                bomb.transform.position = _Controller.position;
            }

        }

        //Gestion du saut
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if(!_isJumping)
            {
                _Personnage.AddForce(_JumpSpeed * _Personnage.transform.up + _Personnage.velocity.normalized * _directionalJumpFactor, ForceMode.VelocityChange);
                _isJumping = true;
            }
        }






	}

    void FixedUpdate()
    {


    }


   


    void OnCollisionEnter(Collision collision)
    {

        //Gestion du saut
        if (_isJumping && (
            collision.gameObject.layer == LayerMask.NameToLayer("Sol") || 
            collision.gameObject.layer == LayerMask.NameToLayer("CaisseDestructible")))
        {
            _isJumping = false;
        }

    }


}
