using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBubble : MonoBehaviour
{
    // Reference to the Bubble enemy prefab to spawn
    public GameObject bubbleEnemyPrefab;

    // Count of currently alive enemies in the wave
    private int enemiesAlive;

    // Delay between waves
    public float waveDelay = 3f;

    // Reference to the LevelManager for updating score and kill count
    private LevelManager levelManagerScript;

    // Flag to indicate if the boss has been spawned (prevents further waves)
    public bool bossSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the LevelManager in the scene
        levelManagerScript = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        // Spawn the first wave of enemies
        SpawnWave();
    }

    // Spawns a single wave of bubble enemies at predetermined positions
    void SpawnWave()
    {
        Vector3 pos1 = new Vector3(20, 3, 0);   // Top spawn
        Vector3 pos2 = new Vector3(20, 0, 0);   // Middle spawn
        Vector3 pos3 = new Vector3(20, -3, 0);  // Bottom spawn

        // Instantiate enemies at the above positions
        InstantiateEnemy(pos1);
        InstantiateEnemy(pos2);
        InstantiateEnemy(pos3);
    }

    // Helper method to instantiate a single enemy and track alive count
    void InstantiateEnemy(Vector3 spawnPos)
    {
        // Rotate -90 degrees so the enemy faces left
        GameObject enemy = Instantiate(bubbleEnemyPrefab, spawnPos, Quaternion.Euler(0, 0, -90));

        // Increment the count of alive enemies
        enemiesAlive++;
    }

    // Called by individual enemies when they die
    public void EnemyDied()
    {
        // Reduce the alive count
        enemiesAlive--;

        // Update the global kill count and add score
        levelManagerScript.SetKillCount();
        levelManagerScript.UpdateScore(5);

        // If all enemies are dead and the boss hasn't spawned, schedule the next wave
        if (enemiesAlive <= 0 && bossSpawned == false)
        {
            StartCoroutine(SpawnNextWave());
        }
    }

    // Coroutine to spawn the next wave after a delay
    IEnumerator SpawnNextWave()
    {
        yield return new WaitForSeconds(waveDelay);
        SpawnWave();
    }

    // Called by LevelManager when the boss spawns
    public void BossSpawned()
    {
        bossSpawned = true;
    }
}
