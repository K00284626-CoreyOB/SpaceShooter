using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeballController : MonoBehaviour
{
    // Enemy stats
    public int health = 3;          // How many hits the enemy can take

    // Shooting timing
    private float enemyShootDelay = 2;   // Delay before first shot
    private float enemyRepeatRate = 2;   // Time between subsequent shots

    // Hovering and movement
    public float hoverSpeed = 1.5f;     // How fast the enemy moves to hover position
    public float hoverDistance = 2f;    // Distance to maintain from the player

    // References to prefabs & objects
    [SerializeField] private GameObject bulletPrefab; // Enemy bullet prefab
    [SerializeField] private Transform firePoint;     // Where bullets are spawned
    private GameObject player;                        // Reference to the player
    private LevelManager levelManagerScript;          // Reference to level manager

    // Animation & destruction
    public Animator animator;       // Animator for death animations
    public float destroyDelay = 0.5f; // Delay before destroying enemy after death animation

    // Audio
    private AudioManager audioManager; // Audio manager reference
    public AudioClip explosion;        // Explosion sound when killed

    // Start is called before the first frame update
    void Start()
    {
        // Find references in the scene
        player = GameObject.FindWithTag("Player");
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Begin shooting repeatedly
        InvokeRepeating("Shoot", enemyShootDelay, enemyRepeatRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return; // Do nothing if player is missing (dead or not spawned)

        // Calculate direction to the player
        Vector2 dir = (player.transform.position - transform.position).normalized;

        // Hover at a fixed distance behind the player
        Vector2 target = (Vector2)player.transform.position - dir * hoverDistance;
        transform.position = Vector2.Lerp(transform.position, target, hoverSpeed * Time.deltaTime);

        // Rotate to face the player
        if (player.transform.position != transform.position)
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            targetDirection.z = 0f;
            transform.up = -targetDirection; // Enemy looks toward player
        }
    }

    // Shoots a bullet from the firePoint
    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Give the bullet a forward velocity
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            float bulletSpeed = 10.0f;
            bulletRb.velocity = firePoint.up * bulletSpeed;
        }
    }

    // Handle collisions with player bullets
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            // Destroy the bullet that hit
            Destroy(other.gameObject);

            // Reduce health
            health--;

            // Check for death
            if (health <= 0)
            {
                // Stop movement (if using speed for other logic)
                hoverSpeed = 0;

                // Update level stats
                levelManagerScript.SetKillCount();
                levelManagerScript.UpdateScore(10);

                // Disable collision so it won't get hit again
                GetComponent<Collider2D>().enabled = false;

                // Play death animation
                animator.SetTrigger("Dead");

                // Play explosion sound
                audioManager.PlaySFX(explosion);

                // Destroy the enemy after a delay to allow animation & sound
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
