using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class Boss2Controller : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;       // Boss maximum HP
    public int currentHealth;        // Boss current HP

    [Header("Movement Settings")]
    public float speed = 2f;         // Movement speed entering screen
    private bool reachedEntryPoint = false;

    private Vector3 targetPos;       // Target screen position for boss entrance
    private Vector3 spawnPos = new Vector3(8f, 0, 0); // Shield spawn position

    [Header("Shooting Settings")]
    private float enemyShootDelay = 3f;     // Initial wait before shooting
    private float enemyRepeatRate = 1.5f;   // Rate at which boss fires bullets
    public Transform[] firePoints;          // Random fire points for bullets

    [Header("Prefabs")]
    [SerializeField] private GameObject bulletPrefab; // Boss bullet
    [SerializeField] private GameObject shieldPrefab; // Boss shield
    private GameObject shield;                         // Actual shield instance

    private LevelManager levelManagerScript;
    private PlayerController playerController;
    private BossHealthBar bossHealthBar;

    [Header("Audio")]
    private AudioManager audioManager;
    public AudioClip shieldSound;      // SFX played when shield activates

    // Start is called before the first frame update
    void Start()
    {
        // Get player reference
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // Connect to key managers
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Notify level manager that a boss spawned
        levelManagerScript.BossSpawned();

        // Set up health and UI
        currentHealth = maxHealth;
        bossHealthBar = GameObject.FindWithTag("BossHealthBar").GetComponent<BossHealthBar>();
        bossHealthBar.SetMaxHealth(maxHealth);

        // Boss moves from right into screen
        targetPos = new Vector3(8f, transform.position.y, transform.position.z);

        // Instantiate shield but keep it disabled until coroutine activates it
        shield = Instantiate(shieldPrefab, spawnPos, Quaternion.identity);
        shield.SetActive(false);

        // Begin shooting loop
        InvokeRepeating("Shoot", enemyShootDelay, enemyRepeatRate);

        // Start shield cycle
        StartCoroutine(WaitForShield());
    }

    // Update is called once per frame
    void Update()
    {
        // Move toward entry point until reached
        if (!reachedEntryPoint)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(targetPos.x, transform.position.y, transform.position.z),
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                reachedEntryPoint = true;
            }
        }
    }

    // Fires a bullet from one of several possible fire positions.
    public void Shoot()
    {
        int num = Random.Range(0, firePoints.Length);

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoints[num].position,
            firePoints[num].rotation
        );

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            float bulletSpeed = 10f;
            bulletRb.velocity = firePoints[num].up * bulletSpeed;
        }
    }

    // Waits a few seconds before activating the boss shield.
    IEnumerator WaitForShield()
    {
        yield return new WaitForSeconds(5);
        StartCoroutine(ShieldUp());
    }

    // Turns shield on for 6 seconds, plays sound, then turns it off.
    // Loops forever.
    IEnumerator ShieldUp()
    {
        shield.SetActive(true);
        audioManager.PlaySFX(shieldSound);

        yield return new WaitForSeconds(6);

        shield.SetActive(false);

        // Restart shield cycle
        StartCoroutine(WaitForShield());
    }

    // Takes damage from bullets, updates HP bar, and handles boss death.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);

            currentHealth--;
            bossHealthBar.SetHealth(currentHealth);

            playerController.OnEnemyHit();

            if (currentHealth <= 0)
            {
                //Game analytics for level complete
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level_2");
                float accuracy = (float)playerController.shotsHit / playerController.shotsFired;

                GameAnalytics.NewDesignEvent("combat:accuracy", accuracy);
                GameAnalytics.NewDesignEvent("enemy_killed:Boss 2");

                speed = 0; // Stop movement on death
                GetComponent<Collider2D>().enabled = false;

                levelManagerScript.UpdateScore(200);
                levelManagerScript.NextLevel(3);

                GooglePlayManager.Instance.UnlockBoss2();

                Destroy(gameObject);
            }
        }
    }
}
