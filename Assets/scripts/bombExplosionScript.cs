using UnityEngine;
using System.Collections;

public class bombExplosionScript : MonoBehaviour {
	
	// Attributs
	float timeBeforeExplode = 3f;
	float timeInWait = 0f;
	
	Animation bombTickingAnimation = null;
	public Animation BombTicking
    {
        get { return bombTickingAnimation; }
        set { bombTickingAnimation = value; }
    }
	
	AudioSource bombExplosionSound = null;
	public AudioSource BombExplosionSound
    {
        get { return bombExplosionSound; }
        set { bombExplosionSound = value; }
    }
	
	ParticleSystem bombExplosionParticle = null;
	public ParticleSystem BombExplosionParticle
    {
        get { return bombExplosionParticle; }
        set { bombExplosionParticle = value; }
    }
	
	// Use this for initialization
	void Start () {
		// Retrieve the bomb ticking animation and launch it
		bombTickingAnimation = this.GetComponent<Animation>();
		BombTicking.Play();
		
		// Retrieve the explosion sound and store it
		bombExplosionSound = this.GetComponent<AudioSource>();
		
		// Retrieve the Particle System and store it
		bombExplosionParticle = this.GetComponent<ParticleSystem>();
	}
	
	
	// Update is called once per frame
	void Update () {
		// time in Wait incrementation :
		timeInWait += Time.deltaTime;
		
		// If the time is up, we launch the explosion sound and animation
		if(timeInWait >= timeBeforeExplode) {
			// Play the Explosion Sound.
			BombExplosionSound.Play();
			
			// PLay the Explosion Particle
			BombExplosionParticle.Play();
			
			// Reset the timer for that bomb
			timeInWait = 0f;
		}
	}
}
