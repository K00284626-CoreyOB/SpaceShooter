using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    // The position the pickup is moving toward (off-screen at the bottom)
    private Vector3 targetPos;

    // Movement speed of the pickup
    public float speed = 2f;

    // Called once when the object is instantiated
    void Start()
    {
        // Set the target Y position below the screen so the pickup falls downwards
        targetPos = new Vector3(transform.position.x, -10f, transform.position.z);
    }

    // Called every frame
    void Update()
    {
        // Move the pickup toward the target position at a constant speed
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(transform.position.x, targetPos.y, transform.position.z),
            speed * Time.deltaTime
        );
    }

    // Called automatically when the object is no longer visible by any camera
    void OnBecameInvisible()
    {
        // Destroy the pickup to prevent clutter and memory leaks
        Destroy(gameObject);
    }
}
