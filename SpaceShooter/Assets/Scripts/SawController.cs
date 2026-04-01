using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class SawController : MonoBehaviour
{
    // Enemy health
    public int health = 1;

    // Horizontal movement speed
    public float speed = 2f;

    // Vertical bouncing speed
    public float verticalSpeed = 2f;

    // Limits for vertical movement
    public float topLimit = 4.3f;
    public float bottomLimit = -4.3f;

    // Reference to LevelManager for score and kill tracking
    private LevelManager levelManagerScript;
    private PlayerController playerController;

    // Target position for horizontal movement
    private Vector3 targetPos;

    // Animator for death animation
    public Animator animator;

    // Delay before destroying object after death
    public float destroyDelay = 0.5f;

    // Audio manager and sound effect
    private AudioManager audioManager;
    public AudioClip explosion;

    // Called once at the start
    void Start()
    {
        // Get player reference
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // Get reference to LevelManager in the scene
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        // Get reference to AudioManager
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Set horizontal target position off-screen to the left
        targetPos = new Vector3(-15f, transform.position.y, transform.position.z);

        // Randomize vertical direction to make saw movement less predictable
        if (Random.value > 0.5f)
        {
            verticalSpeed = -verticalSpeed;
        }
    }

    // Called every frame
    void Update()
    {
        // Move horizontally towards target X position
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(targetPos.x, transform.position.y, transform.position.z),
            speed * Time.deltaTime
        );

        // Move vertically for bouncing motion
        transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);

        // Reverse vertical direction if hitting top or bottom limits
        if (transform.position.y >= topLimit)
        {
            verticalSpeed = -Mathf.Abs(verticalSpeed);
        }
        if (transform.position.y <= bottomLimit)
        {
            verticalSpeed = Mathf.Abs(verticalSpeed);
        }
    }

    // Destroy the saw when it leaves the camera view
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    // Called when saw collides with something
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit by player's bullet
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // Destroy the bullet
            health--; // Reduce health

            playerController.OnEnemyHit();

            // If health reaches 0, trigger death
            if (health <= 0)
            {
                GameAnalytics.NewDesignEvent("enemy_killed:Saw");

                // Stop movement
                speed = 0;
                verticalSpeed = 0;

                // Update LevelManager stats
                levelManagerScript.SetKillCount();
                levelManagerScript.UpdateScore(5);

                // Disable collider to prevent further collisions
                GetComponent<Collider2D>().enabled = false;

                // Trigger death animation
                animator.SetTrigger("Dead");

                // Play explosion sound
                audioManager.PlaySFX(explosion);

                // Destroy saw object after delay
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
