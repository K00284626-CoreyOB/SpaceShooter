using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShieldController : MonoBehaviour
{
    // Reference to the player script so the shield can notify it when hit
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        // Get the player in the scene
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame 
    void Update()
    {
        // (Nothing needed here for shield behavior)
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Shield blocks certain enemy types:
        // - Enemy bullets
        // - Kamikaze enemies
        // - Tanks
        // - Beam attacks
        // - Saw enemies
        if (other.CompareTag("EnemyBullet") ||
            other.CompareTag("Kamikaze") ||
            other.CompareTag("Tank") ||
            other.CompareTag("Beam") ||
            other.CompareTag("Saw"))
        {
            // Destroy the incoming danger
            Destroy(other.gameObject);

            // Notify the player that the shield has been hit
            player.BubbleHit();
        }
        else if (other.CompareTag("Boss1"))
        {
            // Trigger bubble break & invincibility
            player.BubbleHit();
            player.StartCoroutine(player.InvincibilityFrames());
        }
    }
}
