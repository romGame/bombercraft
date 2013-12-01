using UnityEngine;
using System.Collections;

public class ToggleExplosion : MonoBehaviour {

    MeshRenderer meshExplosion = null;


	// Use this for initialization
	void Start () {

	    meshExplosion = this.GetComponent<MeshRenderer>();
        meshExplosion.gameObject.renderer.enabled = false;
        //Hide();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void Show()
    {
        
        //this.GetComponent<MeshFilter>().mesh = meshExplosion;
        //meshExplosion.enabled = true;
        //meshExplosion.transform.position = new Vector3(0, 0, 0);
        if (meshExplosion != null)
        {
            meshExplosion.gameObject.renderer.enabled = true;
        }
    }

    public void Hide()
    {
        
        //this.GetComponent<MeshFilter>().mesh = null;
        //meshExplosion.enabled = false;
        //meshExplosion.transform.position = new Vector3(500, 500, 500);
        if (meshExplosion != null)
        {
            meshExplosion.gameObject.renderer.enabled = false;
        }
    }



}
