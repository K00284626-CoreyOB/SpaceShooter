using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Player movement speed
    public float speed = 5f;

    // Reference to Rigidbody2D
    public Rigidbody2D rb;

    // Delay before destroying certain objects (like enemies)
    public float destroyDelay = 0.5f;

    // Invincibility flags
    private bool isInvincible = false;
    public float invincibleDuration = 2f;

    // Input variables for keyboard and joystick
    private float keyboardHorizontalInput;
    private float keyboardVerticalInput;
    private float joystickHorizontalInput;
    private float joystickVerticalInput;

    // Combined input values after clamping
    private float _horizontalInput;
    private float _verticalInput;

    // Animator for player animations
    public Animator animator;

    // Reference to a fixed joystick for mobile input
    public FixedJoystick joystick;

    // Prefabs for player projectiles and protective bubble
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bubblePrefab;
    private GameObject bubble;

    private GameObject player;

    // Transform representing where bullets are spawned
    [SerializeField] private Transform firePoint;

    // Reference to level manager for score/health updates
    private LevelManager levelManagerScript;

    // Canvas shown when hit by slime
    public Canvas slimeScreen;

    // Camera and padding for clamping player within screen bounds
    private Camera cam;
    public float padding = 0.5f;

    // Audio manager and clips
    private AudioManager audioManager;
    public AudioClip shoot;
    public AudioClip slimeClip;
    public AudioClip starSound;
    public AudioClip healthSound;
    public AudioClip bubbleSound;
    public AudioClip hitSound;
    public AudioClip bigHitSound;
    public AudioClip deadSound;

    // Start is called before the first frame update
    void Start()
    {
        // Reference to the player object in scene
        player = GameObject.FindWithTag("Player");

        // Get LevelManager reference for managing score, health, and game state
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        // Get AudioManager reference for sound effects
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        // Instantiate protective bubble (inactive at start)
        bubble = Instantiate(bubblePrefab, player.transform.position, Quaternion.identity);
        bubble.SetActive(false);

        // Reference main camera for screen bounds
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow movement when game is not paused
        if (!levelManagerScript.isPaused)
        {
            // Get keyboard input
            keyboardHorizontalInput = Input.GetAxis("Horizontal");
            keyboardVerticalInput = Input.GetAxis("Vertical");

            // Get joystick input
            joystickHorizontalInput = joystick.Horizontal;
            joystickVerticalInput = joystick.Vertical;

            // Combine inputs and clamp to valid range (-1 to 1)
            _horizontalInput = Mathf.Clamp(keyboardHorizontalInput + joystickHorizontalInput, -1f, 1f);
            _verticalInput = Mathf.Clamp(keyboardVerticalInput + joystickVerticalInput, -1f, 1f);

            // Create movement vector and scale by speed and deltaTime
            Vector3 tempVect = new Vector3(_horizontalInput, _verticalInput, 0).normalized * speed * Time.deltaTime;

            // Set animator bools based on vertical input
            if (_verticalInput < -0.2f)
                animator.SetBool("upPressed", true);
            else if (_verticalInput > 0.2f)
                animator.SetBool("downPressed", true);
            else
            {
                animator.SetBool("upPressed", false);
                animator.SetBool("downPressed", false);
            }

            // Apply movement
            transform.position += tempVect;

            // Clamp position within camera bounds
            Vector3 pos = transform.position;
            Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
            pos.x = Mathf.Clamp(pos.x, min.x + padding, max.x - padding);
            pos.y = Mathf.Clamp(pos.y, min.y + padding, max.y - padding);
            transform.position = pos;

            // Make protective bubble follow player without being a child
            if (bubble.activeSelf)
            {
                bubble.transform.position = transform.position;
            }
        }
    }

    // Player shoots a bullet when shoot button is pressed
    public void Shoot()
    {
        // Instantiate bullet at firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Add velocity to bullet
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            float bulletSpeed = 10.0f;
            bulletRb.velocity = firePoint.up * bulletSpeed;
        }

        // Play shooting sound
        audioManager.PlaySFX(shoot);
    }

    // Coroutine for slime hit effect
    IEnumerator SlimeHit()
    {
        audioManager.PlaySFX(slimeClip);
        slimeScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        slimeScreen.gameObject.SetActive(false);
    }

    // Called when the player collides with triggers
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Damage from small projectiles or saws
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Saw"))
        {
            audioManager.PlaySFX(hitSound);
            Destroy(other.gameObject);
            levelManagerScript.UpdateHealth(-1);

            if (levelManagerScript.health <= 0)
            {
                Destroy(gameObject);
                audioManager.PlaySFX(deadSound);
                levelManagerScript.YouDied();
            }
        }
        // Damage from larger enemies like tank or kamikaze
        else if (other.CompareTag("Tank") || other.CompareTag("Kamikaze"))
        {
            if (other.CompareTag("Kamikaze"))
                Destroy(other.gameObject, destroyDelay);
            else
                Destroy(other.gameObject);

            audioManager.PlaySFX(bigHitSound);
            levelManagerScript.UpdateHealth(-2);

            if (levelManagerScript.health <= 0)
            {
                Destroy(gameObject);
                audioManager.PlaySFX(deadSound);
                levelManagerScript.YouDied();
            }
        }
        // Slime pickups trigger screen effect
        else if (other.CompareTag("Slime"))
        {
            Destroy(other.gameObject);
            StartCoroutine(SlimeHit());
        }
        // Boss or beam damage with invincibility frames
        else if (other.CompareTag("Boss1") || other.CompareTag("Beam"))
        {
            if (isInvincible) return; // ignore damage during invincibility
            audioManager.PlaySFX(bigHitSound);
            levelManagerScript.UpdateHealth(-3);

            if (levelManagerScript.health <= 0)
            {
                Destroy(gameObject);
                audioManager.PlaySFX(deadSound);
                levelManagerScript.YouDied();
            }
        }
        // Collect score items
        else if (other.CompareTag("star"))
        {
            levelManagerScript.UpdateScore(20);
            audioManager.PlaySFX(starSound);
            Destroy(other.gameObject);
        }
        // Collect health pickups
        else if (other.CompareTag("HealthPickup"))
        {
            levelManagerScript.UpdateHealth(3);
            audioManager.PlaySFX(healthSound);
            Destroy(other.gameObject);
        }
        // Activate protective bubble
        else if (other.CompareTag("BubblePickup"))
        {
            Destroy(other.gameObject);
            audioManager.PlaySFX(bubbleSound);
            bubble.SetActive(true);
        }
    }

    // Deactivate the protective bubble when it takes a hit
    public void BubbleHit()
    {
        bubble.SetActive(false);
    }

    // Coroutine to handle temporary invincibility frames
    public IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }
}
