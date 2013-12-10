using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

/* Auteur : Jimmy STOIKOVITCH */

public class NetworkManager : MonoBehaviour {

    [SerializeField]
    private GameObject playerPrefab = null;
    [SerializeField]
    private GameObject playerSkinPrefab = null;
    
    void Start() {
        Application.runInBackground = true;
        Instantiate(playerPrefab, new Vector3(0, 0, -22), Quaternion.identity);
    }
}
