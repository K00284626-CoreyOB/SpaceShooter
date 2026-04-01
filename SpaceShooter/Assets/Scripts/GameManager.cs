using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Used just for start menu to start the game.
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenAchievements()
    {
        if (GooglePlayManager.Instance != null)
        {
            GooglePlayManager.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.LogWarning("GooglePlayManager not found!");
        }
    }

    public void OpenLeaderboard()
    {
        if (GooglePlayManager.Instance != null)
        {
            GooglePlayManager.Instance.ShowLeaderboardUI();
        }
        else
        {
            Debug.LogWarning("GooglePlayManager not found!");
        }
    }
}
