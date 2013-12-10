using UnityEngine;
using System.Collections;

/* Auteur : Jimmy STOIKOVITCH */

public class ToggleExplosion : MonoBehaviour {

    MeshRenderer meshExplosion = null;

	// Use this for initialization
	void Start () {
	    meshExplosion = this.GetComponent<MeshRenderer>();
        meshExplosion.gameObject.renderer.enabled = false;
	}

    public void Show() {
        if (meshExplosion != null) {
            meshExplosion.gameObject.renderer.enabled = true;
        }
    }

    public void Hide() {
        if (meshExplosion != null) {
            meshExplosion.gameObject.renderer.enabled = false;
        }
    }
}
