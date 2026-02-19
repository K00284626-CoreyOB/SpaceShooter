using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSlimeAlien : MonoBehaviour
{
    // Prefab of the slime alien enemy to spawn
    public GameObject slimeAlienPrefab;

    // Horizontal and vertical area limits for spawning (y is fixed here, x is randomized)
    public Vector2 spawnArea = new Vector2(6f, -7f);

    // Delay before the first slime alien spawns
    private float enemyStartDelay = 2;

    // Time interval between consecutive slime alien spawns
    private float enemyRepeatRate = 10;

    // Start is called before the first frame update
    void Start()
    {
        // Repeatedly call the Spawn() method
        InvokeRepeating("Spawn", enemyStartDelay, enemyRepeatRate);
    }

    // Method that spawns a slime alien at a random horizontal position within spawnArea
    void Spawn()
    {
        Vector3 pos = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x), // Random X position
            spawnArea.y,                             // Fixed Y position
            0);                                      // Z position (2D plane)

        // Instantiate the slime alien prefab at the calculated position with default rotation
        Instantiate(slimeAlienPrefab, pos, Quaternion.identity);
    }
}
