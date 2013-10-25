using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BombManager  {

    #region variable

    //Contiendra toutes les caisses destructible du jeu
    private Queue<GameObject> _listBomb;


    private static BombManager _instance = null;

    static readonly object instanceLock = new object();

    private Vector3 _PositionHide = new Vector3(1000, 1000, 1000);
    #endregion




    private BombManager()
    {
        _listBomb = new Queue<GameObject>();
    }


    /// <summary>
    /// Permet de récupérer l'instance du singleton
    /// </summary>
    /// <returns></returns>
    public static BombManager GetBombManagerInstance()
    {

        if (_instance == null)
        {
            lock (instanceLock)
            {
                if (_instance == null) //on vérifie encore, au cas où l'instance aurait été créée entretemps.
                {
                    _instance = new BombManager();
                }
            }
        }
        return _instance;
    }




    /// <summary>
    /// Permet de faire disparaitre la caisse de la map et de l'affiler au joueur
    /// </summary>
    /// <param name="caisse"></param>
    public void AddBomb(GameObject bomb)
    {
        _listBomb.Enqueue(bomb);
        //caisse.transform.renderer.enabled = false;
        bomb.transform.position = _PositionHide;
    }



    /// <summary>
    /// Permet de récupérer la prochaine caisse dans la queue. Retourne null si plus d'objet
    /// </summary>
    /// <returns></returns>
    public GameObject GetBomb()
    {
        GameObject bomb;
        try
        {
            if (_listBomb.Count > 0)
            {
                bomb = _listBomb.Dequeue();

                //Initialisation des variables du script bombExplosion pour rejouer l'animation.
                bomb.GetComponent<bombExplosionScript>().EnableBomb();
            }
            else
            {
                bomb = null;
            }
        }
        catch (Exception)
        {

            bomb = null;
        }
        return bomb;
    }



}
