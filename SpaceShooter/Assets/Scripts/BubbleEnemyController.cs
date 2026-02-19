using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BubbleEnemyController : MonoBehaviour
{
    // Enemy health
    public int health = 3;

    // Movement speed toward target position
    public float speed = 3f;

    // Target position enemy moves toward when entering screen
    private Vector3 targetPos;

    // Shooting values
    private float enemyShootDelay = 3f;  // Time before first shot
    private float enemyRepeatRate = 3f;  // Delay between shots

    // Delay before destroying after death animation
    public float destroyDelay = 0.3f;

    // Reference to the spawn manager (tracks enemy spawns/deaths)
    private SpawnBubble spawnBubble;

    // Animator for death animation
    public Animator animator;

    // Bullet prefab and fire position
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    // Audio
    private AudioManager audioManager;
    public AudioClip explosion;
    public AudioClip shootSound;

    // Start is called before the first frame update
    void Start()
    {
        // Get managers
        spawnBubble = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnBubble>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Move enemy from right side toward gameplay area
        targetPos = new Vector3(7f, transform.position.y, transform.position.z);

        // Begin shooting periodically
        InvokeRepeating("Shoot", enemyShootDelay, enemyRepeatRate);
    }

    // Update is called once per frame
    void Update()
    {
        // Move toward the target entrance position
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    public void Shoot()
    {
        audioManager.PlaySFX(shootSound);

        // Create bullet instance
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Add velocity to bullet if it has a Rigidbody2D
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            float bulletSpeed = 10f;
            bulletRb.velocity = firePoint.up * bulletSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check collision with player bullets
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);  // Remove the player's bullet
            health--;

            if (health <= 0)
            {
                // Stop enemy movement
                speed = 0;

                // Disable hitbox to avoid extra collisions
                GetComponent<Collider2D>().enabled = false;

                // Play death animation and sound
                animator.SetTrigger("Dead");
                audioManager.PlaySFX(explosion);

                // Notify spawn manager that this enemy died
                spawnBubble.EnemyDied();

                // Destroy enemy after animation finishes
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
