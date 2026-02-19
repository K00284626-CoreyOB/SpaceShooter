using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Controller : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;             // Boss maximum HP
    public int currentHealth;               // Boss current HP

    [Header("Movement Settings")]
    public float speed = 3f;                // Boss entry movement speed
    private Vector3 targetPos;              // Where the boss moves before starting patterns
    private bool reachedEntryPoint = false; // True when boss has fully entered the screen

    // Starting position used for hovering/bobbing motion
    private Vector3 startingPos;

    [Header("Bobbing Settings")]
    public float bobAmplitude = 0.5f;       // Vertical movement height
    public float bobFrequency = 1f;         // Vertical movement speed

    [Header("Hover Settings")]
    public float hoverAmplitude = 1f;       // Horizontal movement distance
    public float hoverFrequency = 0.5f;     // Horizontal movement speed

    [Header("Shooting Settings")]
    public float enemyShootDelay = 3f;      // Delay before boss starts shooting
    public float enemyRepeatRate = 1.5f;    // How often boss fires
    public Transform firePoint;             // Fire position for standard bullets
    public GameObject bulletPrefab;         // Prefab for regular shots

    [Header("Beam Attack Settings")]
    public GameObject beamPrefab;           // Big laser prefab
    public float beamDuration = 1f;         // How long beam stays active
    public float beamCooldown = 8f;         // Delay between beam attacks
    public Transform beamPoint;             // Where the beam spawns from
    private bool isFiringBeam = false;      // Freezes movement + disables normal patterns during beam
    private GameObject activeBeam;

    // References
    private GameObject player;
    private LevelManager levelManagerScript;
    private BossHealthBar bossHealthBar;

    [Header("Audio")]
    private AudioManager audioManager;
    public AudioClip shootSound;            // Sound for regular shots
    public AudioClip beamSound;             // Sound for beam attack

    // Start is called before the first frame update
    void Start()
    {
        // Lock initial position for bobbing/hovering
        startingPos = transform.position;

        // References to player and level manager
        player = GameObject.FindGameObjectWithTag("Player");
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Inform level manager that the boss has appeared
        levelManagerScript.BossSpawned();

        // Set up health and UI
        currentHealth = maxHealth;
        bossHealthBar = GameObject.FindWithTag("BossHealthBar").GetComponent<BossHealthBar>();
        bossHealthBar.SetMaxHealth(maxHealth);

        // Choose the position where boss will stop its entry movement
        targetPos = new Vector3(8f, transform.position.y, transform.position.z);

        // Start regular shooting pattern
        InvokeRepeating(nameof(Shoot), enemyShootDelay, enemyRepeatRate);

        // Start repeating beam attack loop
        StartCoroutine(BeamAttackRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Freeze boss if currently firing the beam
        if (isFiringBeam)
            return;

        // Entry movement toward the screen
        if (!reachedEntryPoint)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            // Check if boss has reached its fighting position
            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                reachedEntryPoint = true;
                startingPos = transform.position; // Reset bobbing origin
            }
        }
        else
        {
            // Vertical bobbing motion
            float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;

            // Horizontal hovering motion
            float hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

            // Combine both motions
            transform.position = new Vector3(
                startingPos.x + hoverOffset,
                startingPos.y + bobOffset,
                startingPos.z
            );
        }
    }

    // Fires a standard projectile toward the player.
    public void Shoot()
    {
        // Player may be missing/dead — avoid errors
        if (player == null)
            return;

        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        audioManager.PlaySFX(shootSound);

        // Give bullet velocity
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            float bulletSpeed = 10f;

            // Aim directly at the player
            Vector2 direction = (player.transform.position - firePoint.position).normalized;
            bulletRb.velocity = direction * bulletSpeed;
        }
    }

    // Continuously performs the beam attack with cooldowns in between.
    IEnumerator BeamAttackRoutine()
    {
        // Delay before first beam
        yield return new WaitForSeconds(beamCooldown);

        while (true)
        {
            // Disable movement
            isFiringBeam = true;

            // Spawn the beam and play audio
            activeBeam = Instantiate(beamPrefab, beamPoint.position, beamPrefab.transform.rotation, transform);
            audioManager.PlaySFX(beamSound);

            // Keep the beam active
            yield return new WaitForSeconds(beamDuration);

            // Remove beam
            Destroy(activeBeam);

            // Allow movement again
            isFiringBeam = false;

            // Wait before next beam
            yield return new WaitForSeconds(beamCooldown);
        }
    }

    // Handles taking damage from the player.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);

            currentHealth--;
            bossHealthBar.SetHealth(currentHealth);

            // Boss defeated
            if (currentHealth <= 0)
            {
                speed = 0; // Stop movement
                levelManagerScript.UpdateScore(300);
                levelManagerScript.YouWin(); // Player wins the game

                GetComponent<Collider2D>().enabled = false;
                Destroy(gameObject);
            }
        }
    }
}
