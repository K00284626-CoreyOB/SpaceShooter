using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPickups : MonoBehaviour
{
    // Array of pickup prefabs to spawn (e.g., health, bubble, star)
    public GameObject[] pickups;

    // Delay before the first pickup spawns
    private float pickupStartDelay = 2;

    // Time interval between consecutive pickups
    private float pickupRepeatRate = 10;

    // Defines the spawn area: x = horizontal range, y = vertical spawn position
    public Vector2 spawnArea = new Vector2(6f, 7f);

    // Start is called before the first frame update
    void Start()
    {
        // Repeatedly call the Spawn() method after pickupStartDelay seconds, then every pickupRepeatRate seconds
        InvokeRepeating("Spawn", pickupStartDelay, pickupRepeatRate);
    }

    // Spawns a single random pickup at a random horizontal position within the spawn area
    void Spawn()
    {
        Vector3 pos = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x), // Random X position within horizontal range
            spawnArea.y,                              // Fixed Y position (top of screen)
            0);                                       // Z position (2D game)

        // Pick a random pickup prefab from the array
        int num = Random.Range(0, pickups.Length);

        // Instantiate the selected pickup at the calculated position with no rotation
        Instantiate(pickups[num], pos, Quaternion.identity);
    }
}
