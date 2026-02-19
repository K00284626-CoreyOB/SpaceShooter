using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEyeball : MonoBehaviour
{
    // Prefab of the Eyeball enemy to spawn
    public GameObject eyeballPrefab;

    // Defines the rectangular spawn area: x = horizontal spawn position, y = vertical range
    public Vector2 spawnArea = new Vector2(13f, 5f);

    // Delay before the first enemy spawns
    private float enemyStartDelay = 3;

    // Time interval between consecutive enemy spawns
    private float enemyRepeatRate = 5;

    // Start is called before the first frame update
    void Start()
    {
        // Repeatedly call the Spawn() method starting after a delay
        InvokeRepeating("Spawn", enemyStartDelay, enemyRepeatRate);
    }

    // Spawns a single Eyeball enemy at a random vertical position within the spawn area
    void Spawn()
    {
        Vector3 pos = new Vector3(
            spawnArea.x,                      // Fixed horizontal spawn position
            Random.Range(-spawnArea.y, spawnArea.y),  // Random vertical position within range
            0);                               // Z-position (2D game)

        // Instantiate the Eyeball enemy prefab at the calculated position with no rotation
        Instantiate(eyeballPrefab, pos, Quaternion.identity);
    }
}
