using UnityEngine;
using System.Collections;

/* Auteur : Jimmy STOIKOVITCH */

public class MainMenu : MonoBehaviour {

	// IP et Port par défault affiché au départ. On sélectionne localhost pour faciliter la démo.
    public string matchIP = "127.0.0.1";
    public string matchPort = "8000";

    //GUI
    public int labelW = 80;
    public int fieldW = 100;
    public int elementsH = 35;
    public int fieldX = 80;

    

    void OnGUI() {
        //Regarde si on est déconnecté, le serveur, un client connecté, ou bien en train de se connecter
        switch (Network.peerType) {
            //ce qui est affiché lorsqu'on démarre le jeu, ou lorsqu'on est déconnecté
            case NetworkPeerType.Disconnected:
                MainMenu_GUI();
                break;
            //ce qui est affiché lorsqu'on est connecté au serveur en tant que Client
            case NetworkPeerType.Client:
                Client_GUI();
                break;
            //ce qui est affiché lorsqu'on est celui qui a initialisé le serveur
            case NetworkPeerType.Server:
                Server_GUI();
                StartServer();
                break;
            //ce qui est affiché lorsqu'on est entrain de se connecter au serveur.
            case NetworkPeerType.Connecting:
                Connecting_GUI();
                break;
        }
    }

    //creation des champs d'entrée
    void MainMenu_GUI() {
        //IP
        GUI.Label(new Rect(10, 10, labelW, elementsH), "Server IP");
        matchIP = GUI.TextField(new Rect(fieldX, 10, fieldW, elementsH), matchIP);
        
        //Port
        GUI.Label(new Rect(10, 50, labelW, elementsH), "Server Port");
        matchPort = GUI.TextField(new Rect(fieldX, 50, fieldW, elementsH), matchPort);
		
        //convertion de string en int pour les éléments nécessaires
        int connectPort = int.Parse(matchPort);
		
        //Bouton de connexion
        if (GUI.Button(new Rect(10, 120, 150, 30), "Connect to server")) {
            StartGame(matchIP , connectPort);
        }
		
        //Bouton d'initialisation du serveur
        if (GUI.Button(new Rect(10, 150, 150, 30), "Start a server")) {
            StartServer();
        }
    }
    
    void Client_GUI() {
        GUI.Label(new Rect(10, 10, 500, 100), "Connecte au serveur :" + matchIP);
    }

    void Server_GUI() {
        GUI.Label(new Rect(10, 10, 500, 100), "Nombre de clients : " + Network.connections.Length);
    }
    
    void Connecting_GUI() {
        GUI.Label(new Rect(10, 10, 500, 100), "Connexion au serveur...");
    }


    void StartGame(string matchIP , int connectPort) {
		// Connection au serveur   
      	Network.Connect(matchIP, connectPort);
		
		// On charge la scène de Jeu.
       	Application.LoadLevel("Jeu");
    }

    void StartServer() {
        // Initialise le serveur
        Network.InitializeServer(16, 8000, !Network.HavePublicAddress());
		
		// Lance la scène de jeu. (version serveur, juste la caméra).
        Application.LoadLevel("Jeu");
    }
}