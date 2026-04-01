using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class KamikazeEnemy : MonoBehaviour
{
    // Enemy stats
    public int health = 1;           // How many hits the enemy can take
    public float speed = 2f;         // Vertical diving speed
    public float diveDelay = 3f;     // Time before starting dive
    public float destroyDelay = 0.5f; // Delay before destroying after death

    bool isDiving = false;           // True when enemy is diving towards player

    // References to visual objects
    public GameObject kamikaze;       // The enemy prefab itself
    public GameObject warningBar;     // Warning bar to indicate dive

    private Vector3 targetPos;        // Target position for dive

    public Animator animator;         // Animator for death animation
    private LevelManager levelManagerScript; // Reference to LevelManager for score/kill count
    private PlayerController playerController; // Reference to LevelManager for score/kill count

    // Audio
    private AudioManager audioManager;
    public AudioClip explosion;       // Sound for when kamikaze hits player
    public AudioClip flyBySound;     // Sound when diving starts

    // Start is called before the first frame update
    void Start()
    {
        // Get player reference
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // Get LevelManager and AudioManager references
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Set target position below the screen
        targetPos = new Vector3(transform.position.x, -10f, transform.position.z);

        // Wait for dive delay before starting dive
        StartCoroutine(WaitToDive());
    }

    // Update is called once per frame
    void Update()
    {
        // Move enemy down if it is diving
        if (isDiving)
        {
            // Disable warning bar if it exists
            if (warningBar != null)
            {
                warningBar.SetActive(false);
            }

            // Move enemy towards target Y position
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(transform.position.x, targetPos.y, transform.position.z),
                speed * Time.deltaTime
            );
        }
    }

    // Coroutine to wait before starting dive
    IEnumerator WaitToDive()
    {
        yield return new WaitForSeconds(diveDelay);

        // Play fly-by sound when dive starts
        audioManager.PlaySFX(flyBySound);

        // Start diving
        isDiving = true;
    }

    // Destroy the enemy when it leaves the camera view
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    // Handle collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            GameAnalytics.NewDesignEvent("enemy_killed:Kamikaze");

            // Destroy bullet
            Destroy(other.gameObject);

            // Stop movement
            speed = 0;

            playerController.OnEnemyHit();

            // Update game stats
            levelManagerScript.SetKillCount();
            levelManagerScript.UpdateScore(2);

            // Disable collider and play death animation
            GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("Dead");

            // Destroy the enemy after animation delay
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("Player"))
        {
            // Stop movement
            speed = 0;

            // Disable collider and play death animation
            GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("Dead");

            // Play explosion sound when hitting player
            audioManager.PlaySFX(explosion);

            // Destroy the enemy after animation delay
            Destroy(gameObject, destroyDelay);
        }
    }
}
