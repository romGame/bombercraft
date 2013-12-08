using UnityEngine;
using System.Collections;

public class guiCountersScript : MonoBehaviour {
	
	[SerializeField]
		public Texture blocTexture;
	
	[SerializeField]
		public Texture blocDeath;
	
	public string blocCounterText = "X ";
	public int nbBlocs = 0;
	
	public string blocDeathText = "X ";
	public int nbDeaths = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		if (!Network.isServer)
        {
			GUI.DrawTexture(new Rect(5, 5, 60, 60), blocTexture, ScaleMode.ScaleToFit, true, 0.0f);
			GUI.Label(new Rect(75, 25, 50, 20), blocCounterText + nbBlocs);
			
			GUI.DrawTexture(new Rect(5, 85, 60, 60), blocDeath, ScaleMode.ScaleToFit, true, 0.0f);
			GUI.Label(new Rect(75, 105, 50, 20), blocDeathText + nbDeaths);
		}
	}
}
