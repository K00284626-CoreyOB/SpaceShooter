using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSaw : MonoBehaviour
{
    // Prefab of the saw enemy to spawn
    public GameObject sawPrefab;

    // Fixed spawn position for the saw (offscreen to the right)
    Vector3 pos = new Vector3(12, 0, 0);

    // Delay before the first saw spawns
    private float enemyStartDelay = 5;

    // Time interval between consecutive saw spawns
    private float enemyRepeatRate = 5;

    // Start is called before the first frame update
    void Start()
    {
        // Call the Spawn() method repeatedly: first call after enemyStartDelay seconds, then every enemyRepeatRate seconds
        InvokeRepeating("Spawn", enemyStartDelay, enemyRepeatRate);
    }

    // Instantiates a saw at the predefined spawn position
    void Spawn()
    {
        Instantiate(sawPrefab, pos, Quaternion.identity);
    }
}
