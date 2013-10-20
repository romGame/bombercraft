using UnityEngine;
using System.Collections;

public class bombExplosionScript : MonoBehaviour {
	
	// Attributs
	float timeBeforeExplode = 3f;
	float timeInWait = 0f;
	bool bombActivated = false;
	bool isTicking = false;
	
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
	
	Transform bombTransform = null;
	public Transform BombTransform
    {
        get { return bombTransform; }
        set { bombTransform = value; }
    }
	
	// Use this for initialization
	void Start () {
		// Retrieve the bomb ticking animation and launch it
		bombTickingAnimation = this.GetComponent<Animation>();
		
		// Retrieve the bom transform property
		bombTransform = this.GetComponent<Transform>();
		
		// Retrieve the explosion sound and store it
		bombExplosionSound = this.GetComponent<AudioSource>();
		
		// Retrieve the Particle System and store it
		bombExplosionParticle = this.GetComponent<ParticleSystem>();
		
		// Activate the bomb (for test because after, it will be the user which will activate a bomb)
		bombActivated = true;
	}
	
	
	// Update is called once per frame
	void Update () {
		if(bombActivated) {
			
			if(isTicking == false) {
				isTicking = true;
				BombTicking.Play("bombTickingAnimation");
			}
			
			// time in Wait incrementation :
			timeInWait += Time.deltaTime;
		
			// If the time is up, we launch the explosion sound and animation
			if(timeInWait >= timeBeforeExplode) {
				// Desactivate the ticking and the animation
				isTicking = false;
				BombTicking.Stop("bombTickingAnimation");
				
				// Launch the explosion animation :
				StartCoroutine(ExplosionAnim());
				
				// Reset the timer for that bomb
				timeInWait = 0f;
				
				// Desactivate the bomb
				bombActivated = false;
			}
		}
		
	}
	
	IEnumerator ExplosionAnim() {
        // Play the Explosion Sound.
		BombExplosionSound.Play();
			
		// PLay the Explosion Particle
		BombExplosionParticle.Play();
 
        yield return new WaitForSeconds(2f);
				
		// Move it in another place to make it disappear
		bombTransform.Translate(new Vector3(100f, 100f, 100f));
 
        Debug.Log("Bomb Explosion Finished");
    }
}
