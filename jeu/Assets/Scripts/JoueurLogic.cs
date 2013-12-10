using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Auteur : Jimmy SOIKOVITCH
  * Amélioré par Florent LEFEBVRE. 
*/

public class JoueurLogic : MonoBehaviour
{

    #region variable

    #region Paramétrable
    [SerializeField]
    private float _VitesseDeplacement = 3f;
    [SerializeField]
    private float _VitesseSaut = 8f;
    [SerializeField]
    private float _VitesseRotationCamera = 150f;
    [SerializeField]
    private float _DelaisRafreshServeur = 0.05f;
	
	[SerializeField]
    private BoxCollider _ViseurCollider = null;
	
	[SerializeField]
	private bool _airCollision = false ;

    
    #endregion

    #region Vecteurs d'initialisations
    private Vector3 _MoveDirection = Vector3.zero;
    private Vector3 _AxeRotationJoueur = new Vector3(0, 1, 0);
    #endregion


    //Le joueur
    private Transform _Controller;
    //Le viseur du joueur
    private Transform _Viseur;
    //Pour effectuer les sauts 
    private Rigidbody _Personnage;
    //L'animation de déplacement du joueur
    private Animation _DeplacementAnimation;

    public BombManager _BombManager;
    public CaisseManager _CaisseManager;

    [SerializeField]
    private guiCountersScript counter;

    public NetworkView _NetworkView;
    //public NetworkManager _NetworkManager;

    [SerializeField]
    private GameObject playerPrefab = null;
    [SerializeField]
    private GameObject playerSkinPrefab = null;


    //Gestion des sauts
    private float _directionalJumpFactor = 0.7f;
    private bool _isJumping = false ;

    public bool isLocalPlayer = false;
    //Pour la synchronisazion des joueurs 
    public NetworkPlayer netPlayer;

    private float _lastTimeSave;

    #region Gestion reseau
    private Dictionary<NetworkPlayer, GameObject> ListPlayerConnect = new Dictionary<NetworkPlayer, GameObject>();
    #endregion
    #endregion

	void Start () {
        Application.runInBackground = true;
        _NetworkView = GetComponent<NetworkView>();
        _Controller = GetComponent<Transform>();
        _Personnage = GetComponent<Rigidbody>();
        _CaisseManager = CaisseManager.GetCaisseManagerInstance();
        if (Network.isServer) {
            _BombManager = BombManager.GetBombManagerInstance();
        }

        _lastTimeSave = 0;
        this.tag = "Player";

        if (Network.isServer && ListPlayerConnect.Count == 0) {
            JoinPlayer(_NetworkView.viewID, new Vector3(0, 0, -22), Network.player);
        }

        //On cherche et récupère le viseur du joueur parmit tous les Transforms
        Transform[] listeTransform = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < listeTransform.Length; i++) {
            if (listeTransform[i].gameObject.layer == LayerMask.NameToLayer("ViseurJoueur")) {
                _Viseur = (Transform)listeTransform[i];
            }

            if (listeTransform[i].gameObject.layer == LayerMask.NameToLayer("JoueurGraphic")) {
                _DeplacementAnimation = ((Transform)listeTransform[i]).GetComponent<Animation>();
            }
        }
	}

	void Update () {

        #region Gestion des entrées utilisateur
        if (isLocalPlayer) {
			RaycastHit hit;
            
            //Placement d'une caisse
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
				//Détection de collision pour placer la caisse
				if( Physics.Raycast(transform.position , transform.forward, out hit, 3)) {
					
					float angle = Mathf.Atan2 (hit.point.x -transform.position.x, hit.point.z -transform.position.z );
					
					float DistanceFin = Vector3.Distance(transform.position, hit.point) - 1 ;
					if( DistanceFin >= 1) {
						Vector3 finalPos = transform.position + new Vector3(Mathf.Sin(angle )*DistanceFin, hit.point.y, Mathf.Cos(angle )*DistanceFin );

                        _CaisseManager.GetCaisse(finalPos);
                        networkView.RPC("CaissePoseJoueur",
                                RPCMode.Server,
                                netPlayer,
                                finalPos);
					}
				}
				else {
                    networkView.RPC("CaissePoseJoueur",
                            RPCMode.Server,
                            netPlayer,
                            _Viseur.position);
                    _CaisseManager.GetCaisse(_Viseur.position);
				}
            }

            //Placement d'une bombe
            if (Input.GetKeyDown(KeyCode.Mouse2)) {
                GameObject bomb = _BombManager.GetBomb();
                bomb.transform.position = this.transform.position;
                networkView.RPC("BombPoseJoueur",
                        RPCMode.Server,
                    netPlayer);
            }
			
			_airCollision = false;
			
			//Detection d'un "sol"
			if (Physics.Raycast (transform.position, -Vector3.up, out hit, 1f ) ) {
				_isJumping = false;

			}
			else {
				_isJumping = true;
				//Detection d'une collision dans les airs
				if (Physics.Raycast (transform.position, -Vector3.left, out hit, 0.5f ) || Physics.Raycast (transform.position, Vector3.left, out hit, 0.5f )
					||Physics.Raycast (transform.position, -Vector3.forward, out hit, 0.5f ) || Physics.Raycast (transform.position, Vector3.forward, out hit, 0.5f )) {
					
					_airCollision = true;
				}
			}
			
            //Gestion du saut
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                if (!_isJumping) {
                    _Personnage.AddForce(_VitesseSaut * _Personnage.transform.up + _Personnage.velocity.normalized * _directionalJumpFactor, ForceMode.VelocityChange);
                }
            }
			
        }
        #endregion
    }

    void FixedUpdate() {
        if (isLocalPlayer) {
            counter.nbBlocs = _CaisseManager.Count();
			
            //Déplacement du joueur
			if(!_airCollision) {
            	_MoveDirection.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			}
			else {
				_MoveDirection.Set (0,0,0);
			}

            //Pour jouer les animations de déplacement
            if (_MoveDirection != Vector3.zero && !_DeplacementAnimation.isPlaying) {
                _DeplacementAnimation.PlayQueued("deplacement-creeper");
            }
            else if (_DeplacementAnimation.isPlaying && _MoveDirection == Vector3.zero) {
                _DeplacementAnimation.Stop();
            }
            
            //Faire déplacer le joueur
            _Controller.Translate(_MoveDirection * _VitesseDeplacement * Time.deltaTime);
            //Faire une rotation sur l'axe des Y du personnage en fonction du mouvement de la souris
            _Controller.Rotate(_AxeRotationJoueur, Input.GetAxis("Mouse X") * _VitesseRotationCamera * Time.deltaTime);
			
			//En fonction du délais de rafraichissement
            if (Time.time - _lastTimeSave > _DelaisRafreshServeur) {
                _lastTimeSave = Time.time;
                networkView.RPC("TranslateJoueur",
                        RPCMode.Others,
                        netPlayer,
                        transform.position,
                        transform.rotation);
            }
        }
    }

    //Un joueur arrive sur le serveur
    void OnPlayerConnected(NetworkPlayer p) {
        if (Network.isServer) {
            NetworkViewID newViewID = Network.AllocateViewID();
            networkView.RPC("JoinPlayer", RPCMode.All, newViewID, new Vector3(0, 0, -22), p);
        }
    }


    void OnConnectedToServer() {
        if (isLocalPlayer) {
            networkView.RPC("SendAllPlayers", RPCMode.Server);
        }
    }

    [RPC]
    void SendAllPlayers(NetworkMessageInfo info) {
        if (Network.isServer) {
            //Gestion des skins
            //On retourne au nouveau joueur tous les joueurs déjà connecté
            foreach (KeyValuePair<NetworkPlayer, GameObject> pair in ListPlayerConnect) {
                if (info.sender != pair.Key) {
                    ListPlayerConnect[info.sender].networkView.RPC("CreationSkinAutreJoueur",
                        info.sender,
                        pair.Value.transform.position,
                        pair.Value.transform.rotation,
                        pair.Key);
                }
            }
        }
    }

    //Ajouter un joueur à la scene
    [RPC]
    void JoinPlayer(NetworkViewID newPlayerView, Vector3 pos, NetworkPlayer p) {
        if (isLocalPlayer) {
            GameObject newPlayer;
			
			//Le joueur actuel
            if (p == Network.player) {
                newPlayer = this.gameObject; 
                newPlayer.GetComponent<NetworkView>().viewID = newPlayerView;
                newPlayer.GetComponent<JoueurLogic>().netPlayer = p;
                newPlayer.GetComponent<JoueurLogic>().tag = "Player";
                ListPlayerConnect.Add(p, newPlayer);
            }
            else {
                newPlayer = Instantiate(playerSkinPrefab, pos, Quaternion.identity) as GameObject;
                newPlayer.GetComponent<NetworkView>().viewID = newPlayerView;
                newPlayer.GetComponent<JoueurLogic>().netPlayer = p;
                ListPlayerConnect.Add(p, newPlayer);
            }


            //GameObject Bomb = Instantiate(BombPrefab, new Vector3 (1000,1000,1000), Quaternion.identity) as GameObject;
            //TODO gestion des bombes

            if (Network.isServer) {
                foreach (NetworkPlayer np in Network.connections) {
                    networkView.SetScope(np, true);

                }
            }
        }
    }

    [RPC]
    void CreationSkinAutreJoueur(Vector3 position, Quaternion rotation, NetworkPlayer p) {
        if (isLocalPlayer) {
            GameObject playerSkin = Instantiate(playerSkinPrefab, position, rotation) as GameObject;
            playerSkin.GetComponent<JoueurLogic>().isLocalPlayer = false;
            ListPlayerConnect.Add(p, playerSkin);
        }
    }

    [RPC]
    void TranslateJoueur(NetworkPlayer playerID, Vector3 pos, Quaternion rot) {
        if (isLocalPlayer) {
            GameObject player = ListPlayerConnect[playerID];
            player.transform.position = pos;
            player.transform.rotation = rot;

        }
    }

    [RPC]
    void CaissePoseJoueur(NetworkPlayer playerID, Vector3 pos) {
        if (Network.isServer) {
            GameObject player = ListPlayerConnect[playerID];
            _CaisseManager.GetCaisse(pos);
        }
    }

    [RPC]
    void BombPoseJoueur(NetworkPlayer playerID) {
        if (Network.isServer) {
            GameObject player = ListPlayerConnect[playerID];
            GameObject bomb = _BombManager.GetBomb();
            bomb.transform.position = player.transform.position;
            
			//TODO Ajouter une bombe
            //GameObject bomb =  player.GetComponent<JoueurLogic>()._BombManager.GetBomb();
            //bomb.transform.position = player.transform.position;
        }
    }
}