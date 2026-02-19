using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // Player stats
    public int score = 0;             // Player score
    public int health = 10;           // Player HP

    // Boss name display
    private string bossName;
    private TextMeshProUGUI scoreText;  // UI element for score
    private TextMeshProUGUI healthText; // UI element for health
    private TextMeshProUGUI nameText;   // UI element for boss name

    // Game state flags
    public bool isPaused = false;     // True if game is paused
    public bool bossSpawned = false;  // True if boss has spawned

    // UI canvases
    public Canvas pauseMenu;
    public Canvas deadMenu;
    public Canvas winMenu;
    public Canvas bossHealthBar;

    // Kill count tracking (for spawning bosses)
    public int killCount = 0;

    // References to spawn scripts for enemies and bosses
    private SpawnBoss1 spawnBoss1Script;
    private SpawnBoss2 spawnBoss2Script;
    private SpawnBoss3 spawnBoss3Script;
    private SpawnBubble spawnBubbleScript;
    private GameObject spawnManager;

    // Audio management
    private AudioManager audioManager;
    public AudioClip level1Music;
    public AudioClip boss1Music;
    public AudioClip boss1SFX;
    public AudioClip level2Music;
    public AudioClip boss2Music;
    public AudioClip level3Music;
    public AudioClip boss3Music;

    // Called once at scene start
    void Start()
    {
        // Prevent this object and UIs from being destroyed on scene load
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(pauseMenu);
        DontDestroyOnLoad(deadMenu);
        DontDestroyOnLoad(winMenu);
        DontDestroyOnLoad(bossHealthBar);

        // Ensure game runs at normal speed
        Time.timeScale = 1f;
    }

    void OnEnable()
    {
        // Subscribe to sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe to avoid duplicate calls
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called every time a new scene loads
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reconnect references to SpawnManager and UI elements
        spawnManager = GameObject.FindWithTag("SpawnManager");
        scoreText = GameObject.FindWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
        healthText = GameObject.FindWithTag("HealthText").GetComponent<TextMeshProUGUI>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        if (spawnManager != null)
        {
            spawnBoss1Script = spawnManager.GetComponent<SpawnBoss1>();
            spawnBoss2Script = spawnManager.GetComponent<SpawnBoss2>();
            spawnBoss3Script = spawnManager.GetComponent<SpawnBoss3>();
            spawnBubbleScript = spawnManager.GetComponent<SpawnBubble>();
        }

        // Reset per-level state
        killCount = 0;
        bossSpawned = false;

        // Update UI
        UpdateScoreUI();
        UpdateHealthUI();

        // Play level music depending on the scene
        if (scene.name == "Level1") audioManager.PlayMusic(level1Music);
        else if (scene.name == "Level2") audioManager.PlayMusic(level2Music);
        else if (scene.name == "Level3") audioManager.PlayMusic(level3Music);
    }

    // Called every frame
    void Update()
    {
        // Pause/unpause game with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        // Boss spawn logic depending on level and kill count
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (sceneName == "Level1" && killCount == 15 && !bossSpawned) //After 15 kills in level 1 spawn boss
        {
            // Spawn boss 1
            spawnBubbleScript.BossSpawned();
            bossSpawned = true;
            spawnBoss1Script.SpawnBoss();
            bossName = spawnBoss1Script.bossName;

            // Change music to boss theme
            audioManager.PlayMusic(boss1Music);
            audioManager.PlaySFX(boss1SFX);
        }
        else if (sceneName == "Level2" && killCount == 20 && !bossSpawned) //After 20 kills in level 2 spawn boss
        {
            // Spawn boss 2
            bossSpawned = true;
            spawnBoss2Script.SpawnBoss();
            bossName = spawnBoss2Script.bossName;

            //Change music to boss theme
            audioManager.PlayMusic(boss2Music);
        }
        else if (sceneName == "Level3" && killCount == 30 && !bossSpawned) //After 30 kills in level 3 spawn boss
        {
            // Spawn boss 3
            spawnBubbleScript.BossSpawned();
            bossSpawned = true;
            spawnBoss3Script.SpawnBoss();
            bossName = spawnBoss3Script.bossName;

            //Change music to boss theme
            audioManager.PlayMusic(boss3Music);
        }
    }

    // Update the score UI text
    public void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    // Update the health UI text
    public void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "HP: " + health.ToString();
    }

    // Add to player score
    public void UpdateScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    // Add/subtract player health
    public void UpdateHealth(int value)
    {
        health += value;
        UpdateHealthUI();
    }

    // Quit to main menu
    public void QuitGame()
    {
        isPaused = true;
        pauseMenu.gameObject.SetActive(false);
        deadMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        bossHealthBar.gameObject.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    // Restart the game from Level1
    public void RestartGame()
    {
        killCount = 0;
        health = 10;
        isPaused = false;
        deadMenu.gameObject.SetActive(false);
        bossHealthBar.gameObject.SetActive(false);
        SceneManager.LoadScene("Level1");
    }

    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.gameObject.SetActive(true);
    }

    // Show death menu
    public void YouDied()
    {
        isPaused = true;
        Time.timeScale = 0f;
        deadMenu.gameObject.SetActive(true);
    }

    // Show win menu
    public void YouWin()
    {
        isPaused = true;
        Time.timeScale = 0f;
        winMenu.gameObject.SetActive(true);
    }

    // Resume the game from pause
    public void ResumeGame()
    {
        Debug.Log("ResumeGame called on: " + gameObject.name);
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.gameObject.SetActive(false);
    }

    // Show boss health bar and set the name text
    public void BossSpawned()
    {
        bossHealthBar.gameObject.SetActive(true);
        nameText = GameObject.FindWithTag("NameText").GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = bossName;
    }

    // Check if the game is paused
    public bool IsPaused()
    {
        return isPaused;
    }

    // Increment kill count
    public void SetKillCount()
    {
        killCount++;
    }

    // Load the next level
    public void NextLevel(int levelNum)
    {
        killCount = 0;
        bossSpawned = false;
        bossHealthBar.gameObject.SetActive(false);
        SceneManager.LoadScene("Level" + levelNum);
    }
}
