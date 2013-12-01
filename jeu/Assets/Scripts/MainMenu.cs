using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{


    public string matchIP = "192.168.1.3";
    public string matchPort = "8000";

    //GUI
    public int labelW = 80;
    public int fieldW = 100;
    public int elementsH = 35;
    public int fieldX = 80;

    

    void OnGUI()
    {
        //Regarde si on est déconnecté, le serveur, un client connecté, ou bien en train de se connecter
        switch (Network.peerType)
        {
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
    void MainMenu_GUI()
    {
        //IP
        GUI.Label(new Rect(10, 10, labelW, elementsH), "Server IP");
        matchIP = GUI.TextField(new Rect(fieldX, 10, fieldW, elementsH), matchIP);
        
        //Port
        GUI.Label(new Rect(10, 50, labelW, elementsH), "Server Port");
        matchPort = GUI.TextField(new Rect(fieldX, 50, fieldW, elementsH), matchPort);
        //conversion de string en int pour les éléments nécessaires
        int connectPort = int.Parse(matchPort);
        //Bouton de connexion
        if (GUI.Button(new Rect(10, 120, 150, 30), "Connect to server"))
        {
            StartGame(matchIP , connectPort);
        }
        //Bouton d'initialisation du serveur
        if (GUI.Button(new Rect(10, 150, 150, 30), "Start a server"))
        {
            StartServer();
        }
    }
    
    void Client_GUI()
    {
        GUI.Label(new Rect(10, 10, 500, 100), "Connecte au serveur :" + matchIP);
    }

    void Server_GUI()
    {
        GUI.Label(new Rect(10, 10, 500, 100), "Nombre de clients : " + Network.connections.Length);
    }
    
    void Connecting_GUI()
    {
        GUI.Label(new Rect(10, 10, 500, 100), "Connexion au serveur...");
    }


    void StartGame(string matchIP , int connectPort)
    {
       Network.Connect(matchIP, connectPort);
       //Network.SetLevelPrefix(1);
       Application.LoadLevel("Jeu");
    }

    void StartServer()
    {
        Debug.Log("Lancement du serveur.");
        //Network.SetLevelPrefix(1);
        Network.InitializeServer(16, 8000, !Network.HavePublicAddress());
        Application.LoadLevel("Jeu");
    }

    /*
    void LoadLevel()
    {
        string level = "Jeu";
        int levelPrefix = 1;
        Debug.Log("Loading level " + level + " with prefix " + levelPrefix);
 
        // There is no reason to send any more data over the network on the default channel,
        // because we are about to load the level, thus all those objects will get deleted anyway
        Network.SetSendingEnabled(0, false);   
 
        // We need to stop receiving because first the level must be loaded.
        // Once the level is loaded, RPC's and other state update attached to objects in the level are allowed to fire
        Network.isMessageQueueRunning = false;
 
        // All network views loaded from a level will get a prefix into their NetworkViewID.
        // This will prevent old updates from clients leaking into a newly created scene.
        Network.SetLevelPrefix(levelPrefix);
        Application.LoadLevel(level);
 
        // Allow receiving data again
        Network.isMessageQueueRunning = true;
        // Now the level has been loaded and we can start sending out data
        Network.SetSendingEnabled(0, true);
  
    }*/


}