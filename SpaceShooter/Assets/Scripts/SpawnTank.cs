using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTank : MonoBehaviour
{
    // Prefab of the tank enemy to spawn
    public GameObject tankPrefab;

    // Fixed spawn position for tanks
    private Vector3 pos = new Vector3(12, 0, 0);

    // Delay before the first tank spawns
    private float enemyStartDelay = 2;

    // Time interval between consecutive tank spawns
    private float enemyRepeatRate = 15;

    // Start is called before the first frame update
    void Start()
    {
        // Repeatedly call the Spawn() method
        InvokeRepeating("Spawn", enemyStartDelay, enemyRepeatRate);
    }

    // Method that spawns a tank at the fixed position
    void Spawn()
    {
        Instantiate(tankPrefab, pos, Quaternion.identity);
    }
}
