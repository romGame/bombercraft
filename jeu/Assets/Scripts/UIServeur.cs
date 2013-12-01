using UnityEngine;
using System.Collections;

public class UIServeur : MonoBehaviour {


    //Pour utilisé les informations du serveur (nombre de joueur, ip etc ....)
    [SerializeField]
    private NetworkManager _manager = null;

    void OnGUI()
    {
        GUI.Label(new Rect(80, 20, 150, 30), "Serveur actif");
        //GUI.Label(new Rect(10, 120, 150, 30), "Nombre de joueur : " + _manager.NombreJoueur);
    }






}
