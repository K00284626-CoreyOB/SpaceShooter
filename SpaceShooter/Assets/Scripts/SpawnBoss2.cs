using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss2 : MonoBehaviour
{
    // Reference to the Boss2 prefab to spawn
    public GameObject boss2Prefab;

    // Position where the boss will appear in the scene
    private Vector3 spawnPos = new Vector3(20f, 0f, 0f);

    // Name of the boss (used for UI display, e.g., health bar)
    public string bossName = "Auralex the Phased Guardian";

    // Public method to spawn the boss at the predefined position
    public void SpawnBoss()
    {
        // Instantiate the boss prefab at spawnPos with default rotation
        Instantiate(boss2Prefab, spawnPos, Quaternion.identity);
    }
}
