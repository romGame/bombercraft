using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CaisseManager 
{

    #region variable

    //Contiendra toutes les caisses destructible du jeu
    private Queue<GameObject> _listCaisseDestructible;

    //Gestion du singleton
    private static CaisseManager _instance = null;
    static readonly object instanceLock = new object();

    //La position à affecter aux caisses pour les "cacher" 
    private Vector3 _PositionHide = new Vector3(1020, 1020, 1020);
    #endregion


    private CaisseManager()
    {
        _listCaisseDestructible = new Queue<GameObject>();
    }

    /// <summary>
    /// Permet de récupérer l'instance du singleton
    /// </summary>
    /// <returns></returns>
    public static CaisseManager GetCaisseManagerInstance()
    {
        
        if (_instance == null)
        {
            lock (instanceLock)
            {
                if (_instance == null) //on vérifie encore, au cas où l'instance aurait été créée entretemps.
                {
                    _instance = new CaisseManager();
                }
            }
        }
        return _instance;
    }

    /// <summary>
    /// Permet de faire disparaitre la caisse de la map et de l'affiler au joueur
    /// </summary>
    /// <param name="caisse"></param>
    public void AddCaisse(GameObject caisse)
    {
        _listCaisseDestructible.Enqueue(caisse);
        caisse.transform.position = _PositionHide;
    }


    /// <summary>
    /// Permet de récupérer la prochaine caisse dans la queue. Retourne null si plus d'objet
    /// </summary>
    /// <returns></returns>
    public GameObject GetCaisse(Vector3 PositionCaisse)
    {
        GameObject caisse = null;
        try
        {

            if (_listCaisseDestructible.Count > 0)
            {
                caisse = _listCaisseDestructible.Dequeue();
                //Initialisation des variables du script de destruction des casse pour rejouer l'animation.
                caisse.GetComponent<DestructionCaisseScript>().InitProperty(PositionCaisse);
				//caisse.GetComponent<DestructionCaisseScript>().InitProperty();
            }
        }
        catch (Exception)
        {
        }
        return caisse;
    }

}
