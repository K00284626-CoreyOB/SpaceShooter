using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Controller : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 60;      // Boss maximum HP
    public int currentHealth;       // Boss current HP

    [Header("Movement Settings")]
    public float speed = 2f;        // Horizontal movement speed
    public float verticalSpeed = 2f; // Vertical bounce speed

    public float topLimit = 4.3f;   // Upper Y boundary for bouncing
    public float bottomLimit = -4.3f; // Lower Y boundary for bouncing

    private LevelManager levelManagerScript;
    private Vector3 targetPos;      // Entry point the boss moves to at start
    private Vector3 targetPos2;     // Point to run toward after waiting

    [Header("State Flags")]
    public float destroyDelay = 0.5f;
    public float runDelay;         // Random delay before boss rushes
    bool isRunning = false;        // Whether the boss is currently charging
    bool pointReached = false;     // Boss reached entry point?
    bool reachedEntryPoint = false; // Has boss entered the screen fully?

    private BossHealthBar bossHealthBar;

    [Header("Audio")]
    private AudioManager audioManager;
    public AudioClip runSound;      // Sound played when boss begins running

    // Start is called before the first frame update
    void Start()
    {
        // Get main LevelManager
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        levelManagerScript.BossSpawned();

        // Initialize health bar
        currentHealth = maxHealth;
        bossHealthBar = GameObject.FindWithTag("BossHealthBar").GetComponent<BossHealthBar>();
        bossHealthBar.SetMaxHealth(maxHealth);

        // Get Audio Manager for sound playback
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Starting target position (boss moves from right to center screen)
        targetPos = new Vector3(8f, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        //Boss entering the screen from the right
        if (!reachedEntryPoint) //if boss hasnt reached the target position, move to the target position
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(targetPos.x, transform.position.y, transform.position.z),
                speed * Time.deltaTime
            );

            float dist = Mathf.Abs(transform.position.x - targetPos.x);

            // Once boss reaches entry point, begin idle bouncing phase
            if (dist < 0.01f)
            {
                reachedEntryPoint = true;
                pointReached = true;
                StartCoroutine(WaitToRun()); // Delay before boss charges
            }
        }

        //Boss bouncing vertically
        if (!isRunning && pointReached)//If the boss reached the target position and is not running, move up and down the screen.
        {
            transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);

            // Reverse direction at top
            if (transform.position.y >= topLimit)
                verticalSpeed = -Mathf.Abs(verticalSpeed);

            // Reverse direction at bottom
            if (transform.position.y <= bottomLimit)
                verticalSpeed = Mathf.Abs(verticalSpeed);
        }
        else if (isRunning)//if true, run across the screen.
        {
            //Boss performing a fast run across the screen
            transform.position = Vector3.MoveTowards(transform.position, targetPos2, speed * Time.deltaTime);
        }
    }

    // Random wait time before boss initiates a running attack.
    IEnumerator WaitToRun()
    {
        runDelay = Random.Range(3, 10);
        yield return new WaitForSeconds(runDelay);

        // Play run sound
        audioManager.PlaySFX(runSound);

        // Set running destination (offscreen to the left)
        targetPos2 = new Vector3(-15f, transform.position.y, transform.position.z);

        pointReached = false;
        isRunning = true;
    }

    // Called automatically when boss leaves the camera view. Used to respawn boss back to starting location.
    void OnBecameInvisible()
    {
        RespawnBoss();
    }

    // Resets position and state when boss runs offscreen.
    void RespawnBoss()
    {
        isRunning = false;
        pointReached = false;
        reachedEntryPoint = false;

        // Move boss back to right side of screen
        transform.position = new Vector3(20f, 0f, 0f);

        // Reset movement targets
        targetPos = new Vector3(8f, transform.position.y, transform.position.z);
        targetPos2 = new Vector3(-15f, transform.position.y, transform.position.z);
    }

    // Handles collision with player bullets. Reduces HP and triggers death when HP reaches zero.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);

            currentHealth--;
            bossHealthBar.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                // Stop movement completely
                speed = 0;
                verticalSpeed = 0;

                GetComponent<Collider2D>().enabled = false;

                // Award score and progress to next level
                levelManagerScript.UpdateScore(100);
                levelManagerScript.NextLevel(2);

                Destroy(gameObject);
            }
        }
    }
}
