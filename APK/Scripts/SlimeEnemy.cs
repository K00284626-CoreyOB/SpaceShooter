using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    // Enemy health
    public int health = 2;

    // Horizontal movement speed
    public float speed = 3f;

    // Delay before destroying object after death
    public float destroyDelay = 0.5f;

    // Shooting timing
    private float enemyShootDelay = 5;
    private float enemyRepeatRate = 5;

    // Target position for vertical movement
    private Vector3 targetPos;

    // Animator for death animations
    public Animator animator;

    // Reference to LevelManager for score tracking
    private LevelManager levelManagerScript;

    // Reference to player for aiming
    private GameObject player;

    // Projectile prefab and spawn point
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Transform firePoint;

    // Audio manager and sound effect
    private AudioManager audioManager;
    public AudioClip explosion;

    // Called once at start
    void Start()
    {
        // Get player reference
        player = GameObject.FindWithTag("Player");

        // Get LevelManager reference
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        // Get AudioManager reference
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Start shooting repeatedly
        InvokeRepeating("Shoot", enemyShootDelay, enemyRepeatRate);

        // Set vertical target position (move upward)
        targetPos = new Vector3(transform.position.x, -4, transform.position.z);
    }

    // Called once per frame
    void Update()
    {
        // Move vertically toward target
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (player != null)
        {
            // Rotate slime to face player
            if (player.transform.position != transform.position)
            {
                Vector3 targetDirection = player.transform.position - transform.position;
                targetDirection.z = 0f; // Keep in 2D plane
                transform.up = targetDirection;
            }
        }
    }

    // Spawn a projectile aimed forward
    public void Shoot()
    {
        // Instantiate slime projectile
        GameObject slime = Instantiate(slimePrefab, firePoint.position, firePoint.rotation);

        // Apply forward velocity
        Rigidbody2D slimeRb = slime.GetComponent<Rigidbody2D>();
        if (slimeRb != null)
        {
            float bulletSpeed = 10.0f; // Adjust as needed
            slimeRb.velocity = firePoint.up * bulletSpeed;
        }
    }

    // Handle collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for player bullet
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // Destroy bullet
            health--; // Reduce health

            if (health <= 0)
            {
                // Stop movement
                speed = 0;

                // Update LevelManager stats
                levelManagerScript.SetKillCount();
                levelManagerScript.UpdateScore(2);

                // Disable collider to prevent further collisions
                GetComponent<Collider2D>().enabled = false;

                // Trigger death animation
                animator.SetTrigger("Dead");

                // Play explosion sound
                audioManager.PlaySFX(explosion);

                // Destroy slime after a short delay
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
