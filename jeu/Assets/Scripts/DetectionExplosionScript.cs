using UnityEngine;
using System.Collections;

/* Auteur : Jimmy STOIKOVITCH */

public class DetectionExplosionScript : MonoBehaviour {
    //Vérification du trigger (détection des caisses)
    void OnTriggerEnter(Collider other)
    {
        //Si une caisse se trouve dans le trigger alors on la détruit
        other.GetComponent<DestructionCaisseScript>().FaireExploser();
    }
}
