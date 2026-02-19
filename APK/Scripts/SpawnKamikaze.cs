using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnKamikaze : MonoBehaviour
{
    // Prefab of the Kamikaze enemy to spawn
    public GameObject kamikazePrefab;

    // Delay before the first Kamikaze enemy spawns
    private float enemyStartDelay = 2;

    // Time interval between consecutive Kamikaze spawns
    private float enemyRepeatRate = 3;

    // Defines the rectangular spawn area: x = horizontal range, y = vertical spawn position
    public Vector2 spawnArea = new Vector2(6f, 7f);

    // Start is called before the first frame update
    void Start()
    {
        // Call the Spawn() method repeatedly after enemyStartDelay seconds, then every enemyRepeatRate seconds
        InvokeRepeating("Spawn", enemyStartDelay, enemyRepeatRate);
    }

    // Spawns a single Kamikaze enemy at a random horizontal position within the spawn area
    void Spawn()
    {
        Vector3 pos = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x), // Random X position within the horizontal range
            spawnArea.y,                              // Fixed Y position (top of the screen)
            0);                                       // Z position (2D game)

        // Instantiate the Kamikaze enemy prefab at the calculated position with no rotation
        Instantiate(kamikazePrefab, pos, Quaternion.identity);
    }
}
