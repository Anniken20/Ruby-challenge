using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    public bool vertical; // Used to set if robot is moving veritcally
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;

    // Particle Smoke Variable
    public ParticleSystem smokeEffect;

    // broken variable
    bool broken = true;

    // Animation
    Animator animator;

    // Audio Source Variables
    AudioSource audioSource;
    public AudioClip fixedSound;
    public AudioClip brokenSound;

    // Ruby Controller
    private RubyController rubyController;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;

        // Animation
        animator = GetComponent<Animator>();

        // Audio Component
        audioSource = GetComponent<AudioSource>();

        // Broken Sound Plays on Loop
        audioSource.clip = brokenSound;
        audioSource.loop = true;
        audioSource.Play();

        //Ruby
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
        rubyController = rubyControllerObject.GetComponent<RubyController>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        // Fixing robot code
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if (!broken)
        {
            return;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction; ;

            // Animation (Vertical)
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }

        else
        {
            position.x = position.x + Time.deltaTime * speed * direction; ;

            // Animation (Horizontal)
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        // Fixing robot code
        if (!broken)
        {
            return;
        }

        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
   
    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;

        //optional if you added the fixed animation
        animator.SetTrigger("Fixed");

        // Particle effect set to false
        smokeEffect.Stop();

        // Broken sound effect stops and plays fixed sound
        audioSource.clip = fixedSound;
        audioSource.loop = false;
        audioSource.Play();

        if (rubyController != null)
        {
            rubyController.FixedRobots(1);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeHealth(-1);
        }
    }

    // Plays sounds from this script and others
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}