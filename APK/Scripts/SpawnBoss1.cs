using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss1 : MonoBehaviour
{
    // Reference to the Boss1 prefab to spawn
    public GameObject boss1Prefab;

    // Position where the boss will appear in the scene
    private Vector3 spawnPos = new Vector3(20f, 0f, 0f);

    // Name of the boss (used for UI display, e.g., health bar)
    public string bossName = "Zar’Lobath the Shell Titan";

    // Public method to spawn the boss at the defined position
    public void SpawnBoss()
    {
        // Instantiate the boss prefab at spawnPos with default rotation
        Instantiate(boss1Prefab, spawnPos, Quaternion.identity);
    }
}
