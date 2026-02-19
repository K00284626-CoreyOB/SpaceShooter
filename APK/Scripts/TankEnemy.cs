using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemy : MonoBehaviour
{
    // Enemy health
    public int health = 3;

    // Movement speed
    public float speed = 3f;

    // Delay before destroying the enemy object after death
    public float destroyDelay = 0.5f;

    // Rigidbody2D component for physics-based movement
    private Rigidbody2D enemyRb;

    // Animator for death animation
    public Animator animator;

    // References to other game objects and systems
    private GameObject player;               // Reference to the player
    private LevelManager levelManagerScript; // Reference to the level manager
    private AudioManager audioManager;       // Reference to the audio manager

    // Sound effect for explosion/death
    public AudioClip explosion;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody2D component attached to this enemy
        enemyRb = GetComponent<Rigidbody2D>();

        // Find the player in the scene
        player = GameObject.FindWithTag("Player");

        // Get the LevelManager to update score and kill count
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        // Get the AudioManager to play sound effects
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    // FixedUpdate is used for physics-based movement
    void FixedUpdate()
    {
        if (player != null)
        {
            // Calculate normalized direction vector towards the player
            Vector2 lookDirection = (player.transform.position - transform.position).normalized;

            // Move the enemy towards the player
            enemyRb.velocity = lookDirection * speed;

            // Rotate the enemy to face the player
            if (player.transform.position != transform.position)
            {
                Vector3 targetDirection = player.transform.position - transform.position;
                targetDirection.z = 0f;
                transform.up = -targetDirection; // Negative because of sprite orientation
            }
        }
    }

    // Handle collisions with other objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("Hit");

            // Destroy the incoming bullet
            Destroy(other.gameObject);

            // Reduce health
            health--;

            if (health <= 0)
            {
                // Stop movement immediately
                speed = 0;

                // Update global kill count and score
                levelManagerScript.SetKillCount();
                levelManagerScript.UpdateScore(10);

                // Disable the collider to prevent further hits
                GetComponent<Collider2D>().enabled = false;

                // Trigger death animation
                animator.SetTrigger("Dead");

                // Play explosion sound
                audioManager.PlaySFX(explosion);

                // Destroy the enemy game object after destroyDelay seconds
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
