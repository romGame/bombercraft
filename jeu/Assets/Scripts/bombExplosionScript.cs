using UnityEngine;
using System.Collections;

/* Auteur : Romain SPENATO et Jimmy STOIKOVITCH */

public class bombExplosionScript : MonoBehaviour {
	
	// Attributs
	float timeBeforeExplode = 3f;
	float timeInWait = 0f;
	bool bombActivated = false;
	bool isTicking = false;

    [SerializeField]
    float tailleExplosion = 9f;
    Vector3 TailleExplosion = Vector3.zero;

    BombManager _BombManager = null;
	
	Animation bombTickingAnimation = null;
	public Animation BombTicking {
        get { return bombTickingAnimation; }
        set { bombTickingAnimation = value; }
    }
	
	AudioSource bombExplosionSound = null;
	public AudioSource BombExplosionSound {
        get { return bombExplosionSound; }
        set { bombExplosionSound = value; }
    }

    SphereCollider bombExplosionTrigger = null;
    public SphereCollider BombExplosionTrigger {
        get { return bombExplosionTrigger; }
        set { bombExplosionTrigger = value; }
    }


	ParticleSystem bombExplosionParticle = null;
	public ParticleSystem BombExplosionParticle {
        get { return bombExplosionParticle; }
        set { bombExplosionParticle = value; }
    }
	
	Transform bombTransform = null;
	public Transform BombTransform {
        get { return bombTransform; }
        set { bombTransform = value; }
    }

    Transform bomGraphic = null;
    ToggleExplosion bombExplosion = null;

	// Use this for initialization
	void Start () {
		// Retrieve bomb manager instance
        _BombManager = BombManager.GetBombManagerInstance();
        // Retrieve the bomb ticking animation and launch it
        bombTickingAnimation = this.GetComponent<Animation>();

        // Retrieve the bom transform property
        bombTransform = this.GetComponent<Transform>();

        // Retrieve the explosion sound and store it
        bombExplosionSound = this.GetComponent<AudioSource>();

        // Retrieve the Particle System and store it
        bombExplosionParticle = this.GetComponent<ParticleSystem>();

        TailleExplosion.Set(tailleExplosion, tailleExplosion, tailleExplosion);

        SphereCollider[] listeSphereCollider = this.GetComponentsInChildren<SphereCollider>();
        for(int i = 0; i< listeSphereCollider.Length; i++) {
            if (listeSphereCollider[i].gameObject.layer == LayerMask.NameToLayer("Explosion")) {
                bombExplosionTrigger = (SphereCollider)listeSphereCollider[i];
                bombExplosionTrigger.isTrigger = false;
                bombExplosionTrigger.enabled = false;

                bombExplosionTrigger.GetComponent<SphereCollider>().radius = (0.11111f * tailleExplosion);
                break;
            }

        }


        Transform[] listeMesh = this.GetComponentsInChildren<Transform>();
        
        for(int i = 0; i< listeMesh.Length; i++) {
            if (listeMesh[i].gameObject.layer == LayerMask.NameToLayer("ExplosionGraphic")) {
                bomGraphic = ((Transform)listeMesh[i]);
                bomGraphic.transform.localScale = TailleExplosion;
                //bombGraphicMesh.enabled = false;
                break;
            }
        }

        bombExplosion = this.GetComponentInChildren<ToggleExplosion>();
        bombExplosion.Hide();
        _BombManager.AddBomb(this.gameObject);
	}


    public void EnableBomb() {
        bombActivated = true;
        bombExplosionTrigger.isTrigger = false;
        bombExplosionTrigger.enabled = false;

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

                bombExplosion.Show();
				// Launch the explosion animation :
				StartCoroutine(ExplosionAnim());
				
				// Reset the timer for that bomb
				timeInWait = 0f;
				
				// Desactivate the bomb
				bombActivated = false;
                bombExplosionTrigger.isTrigger = true;
                bombExplosionTrigger.enabled = true;
                this.transform.Translate(0f, 0.2f, 0f);

                
			}
		}	
	}
	
	IEnumerator ExplosionAnim() {
        // Play the Explosion Sound.
		BombExplosionSound.Play();
		
		// PLay the Explosion Particle
		BombExplosionParticle.Play();
		
        yield return new WaitForSeconds(2f);
		
        _BombManager.AddBomb(this.gameObject);
		
		// Move it in another place to make it disappear
        bombExplosion.Hide();
    }
}
