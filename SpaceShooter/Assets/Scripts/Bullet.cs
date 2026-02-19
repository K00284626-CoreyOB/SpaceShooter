using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Called automatically by Unity when the object leaves the camera's view.
    // Used here to clean up bullets so they don't exist forever off-screen.
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the bullet hits a shield, destroy the bullet.
        // The shield itself handles any effects (like reducing durability).
        if (other.CompareTag("Shield"))
        {
            Destroy(gameObject);
        }
    }
}
