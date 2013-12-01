using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour {



    private NetworkView _myNetworkView;

    [SerializeField]
    private GameObject playerPrefab = null;
    [SerializeField]
    private GameObject playerSkinPrefab = null;
    [SerializeField]
    private GameObject BombPrefab = null;

    [SerializeField]
    private Camera CameraServeur = null;

    public BombManager _BombManager;
    public CaisseManager _CaisseManager;

    NetworkPlayer LocalPlayer;

    //Liste des joueurs connecté au serveur
    private Dictionary<NetworkViewID, NetworkPlayer> PlayersConnect = new Dictionary<NetworkViewID, NetworkPlayer>();
    private Dictionary<NetworkViewID ,GameObject> ListJoueurGameObject = new Dictionary<NetworkViewID, GameObject>();


    private string LocalAddress = "127.0.0.1";
    private string ServerAddress = "127.0.0.1";



    void Start()
    {
        Application.runInBackground = true;
        _myNetworkView = networkView; /*this.GetComponent<NetworkView>();*/

        LocalAddress = GetLocalIPAddress();
        if (CameraServeur != null) CameraServeur.enabled = false;
        if (Network.isServer)
        {
            _BombManager = BombManager.GetBombManagerInstance();
            _CaisseManager = CaisseManager.GetCaisseManagerInstance();
            //Afficher la carte en vue de dessus pour voir la partie en cours 
            if (CameraServeur != null)
            {
                CameraServeur.enabled = true;
            }
        }


    }


    //Un joueur arrive sur le serveur
    void OnPlayerConnected(NetworkPlayer p)
    {  
        
        if (Network.isServer)
        {
            _myNetworkView.RPC("SaveNetworkPlayer", p, p);
        }
    }


    //On sauvegarde le networkplayer sur le client
    [RPC]
    void SaveNetworkPlayer(NetworkPlayer p)
    {
        LocalPlayer = p;
    }

    [RPC]
    void SaveNetworkViewId(NetworkViewID id)
    {
        _myNetworkView.viewID = id;
    }

    [RPC]
    void JoinPlayer(NetworkViewID infoIViewId, NetworkViewID newPlayerView, Vector3 pos, NetworkPlayer p)
    {
        GameObject newPlayer;
        //if (infoIViewId == _myNetworkView.viewID && Network.isClient)//Le joueur actuel
        if (LocalPlayer == p && Network.isClient)
        {
            newPlayer = Instantiate(playerPrefab, pos, Quaternion.identity) as GameObject;
            newPlayer.GetComponent<NetworkView>().viewID = infoIViewId;
            newPlayer.GetComponent<JoueurLogic>().netPlayer = p;
            newPlayer.GetComponent<JoueurLogic>().IdJoueur = newPlayerView;
            newPlayer.GetComponent<JoueurLogic>()._NetworkManager = this;
            newPlayer.tag = "Player";
            newPlayer.GetComponent<JoueurLogic>().isLocalPlayer = true;
            networkView.RPC("MiseAJourClient", RPCMode.Server, p, infoIViewId, newPlayerView);

        }
        else
        {
            newPlayer = Instantiate(playerSkinPrefab, pos, Quaternion.identity) as GameObject;
            newPlayer.GetComponent<JoueurLogic>().IdJoueur = newPlayerView;
            newPlayer.GetComponent<NetworkView>().viewID = infoIViewId;
            //newPlayer.GetComponent<JoueurLogic>()._NetworkManager = this;
        }





        //GameObject Bomb = Instantiate(BombPrefab, new Vector3 (1000,1000,1000), Quaternion.identity) as GameObject;
        //TODO gestion des bombes
        PlayersConnect.Add(newPlayerView, p);
        ListJoueurGameObject.Add(newPlayerView ,newPlayer);

    }


    [RPC]
    void MiseAJourClient(NetworkPlayer p,NetworkViewID infoIViewId, NetworkViewID newPlayerView)
    {


        //Gestion des skins
        foreach (KeyValuePair<NetworkViewID, GameObject> pair in ListJoueurGameObject)
        {
            if (newPlayerView != pair.Key)
            {
                networkView.RPC("CreationSkinAutreJoueur", 
                    p, 
                    pair.Value.transform.position, 
                    pair.Value.transform.rotation, 
                    pair.Value.GetComponent<JoueurLogic>().IdJoueur,
                    pair.Value.GetComponent<NetworkView>().viewID,
                    PlayersConnect[pair.Key]);
            }

        }

    }

    [RPC]
    void CreationSkinAutreJoueur(Vector3 position, Quaternion rotation, NetworkViewID idjoueur , NetworkViewID networkviewid, NetworkPlayer p)
    {
        GameObject playerSkin = Instantiate(playerSkinPrefab, position, rotation) as GameObject;
        playerSkin.GetComponent<JoueurLogic>().IdJoueur = idjoueur;
        playerSkin.GetComponent<NetworkView>().viewID = networkviewid;
        Debug.Log("[CreationSkinAutreJoueur] - idView : " + networkviewid);
        Debug.Log("[CreationSkinAutreJoueur] - idJoueur : " + idjoueur);
        PlayersConnect.Add(networkviewid, p);
        ListJoueurGameObject.Add(networkviewid, playerSkin);
    }


    [RPC]
    void SendAllPlayers(NetworkMessageInfo info)
    {
        

        Debug.Log("Nouveau joueur, info : " + info.networkView.viewID);
        if (Network.isServer)
        {

            //networkView.RPC("SaveNetworkViewId", info.sender, newID);
            networkView.RPC("JoinPlayer", RPCMode.All, info.networkView.viewID, Network.AllocateViewID(), new Vector3(0, 0, -22), info.sender);
        }
    }




    void OnConnectedToServer()
    {
        
        //networkView.viewID = Network.AllocateViewID();
        networkView.RPC("SendAllPlayers", RPCMode.Server);
    }

    /*
    [RPC]
    void DeplacementJoueur(NetworkViewID playerID, Vector3 translation)
    {
        GameObject player = ListJoueurGameObject[playerID];
        player.transform.Translate(translation);
    }

    [RPC]
    void RotationJoueur(NetworkViewID playerID, Vector3 axe, float angle)
    {
        GameObject player = ListJoueurGameObject[playerID];
        player.transform.Rotate(axe, angle);
    }
    */

    [RPC]
    void TranslateJoueur(NetworkViewID playerID, Vector3 pos, Quaternion rot)
    {
        Debug.Log("[TranslateJoueur] - playerID " + playerID);
        if (Network.isServer)
        {
            GameObject player = ListJoueurGameObject[playerID];
            player.transform.position = pos;
            player.transform.rotation = rot;
        }
    }

    [RPC]
    void CaissePoseJoueur(NetworkViewID playerID, Vector3 pos)
    {
        if (Network.isServer)
        {
            GameObject player = ListJoueurGameObject[playerID];
            _CaisseManager.GetCaisse(pos);
        }
    }

    [RPC]
    void BombPoseJoueur(NetworkViewID playerID)
    {
        if (Network.isServer)
        {
            GameObject player = ListJoueurGameObject[playerID];
            GameObject bomb =  _BombManager.GetBomb();
            bomb.transform.position = player.transform.position;
            //TODO Ajouter une bombe
            //GameObject bomb =  player.GetComponent<JoueurLogic>()._BombManager.GetBomb();
            //bomb.transform.position = player.transform.position;
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        if (Network.isServer)
        {
            networkView.RPC("DisconnectPlayer", RPCMode.All, player);
        }
    }

    [RPC]
    void DisconnectPlayer(NetworkPlayer player)
    {
        if (Network.isClient)
        {
            Debug.Log("Player Disconnected: " + player.ToString());
        }

        // now we have to do the reverse lookup from
        // the NetworkPlayer --> GameObject
        // this is easy with the Hashtable

        if (PlayersConnect.ContainsValue(player))
        {

            /*NetworkViewID tmp = PlayersConnect[

            // we check to see if the gameobject exists
            // or not first just as a safety measure
            // trying to destory a gameObject that
            // doesn't exist causes a runtime error

            if ((GameObject)PlayersConnect[player])
            {
                Destroy((GameObject)PlayersConnect[player]);
            }

            // we also have to remove the Hashtable entry

            PlayersConnect.Remove(player);*/
        }
    }

    public string GetLocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }
        }
        return localIP;
    }
}
