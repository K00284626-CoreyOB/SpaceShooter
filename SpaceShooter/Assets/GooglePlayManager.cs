//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using UnityEngine;

public class GooglePlayManager : MonoBehaviour
{
    public static GooglePlayManager Instance { get; private set; }

    private bool isAuthenticated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayGames();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize the Play Games platform and authenticate the user
    private void InitializePlayGames()
    {
        //PlayGamesPlatform.DebugLogEnabled = true;
        //PlayGamesPlatform.Activate();

        Social.localUser.Authenticate(success => {
            isAuthenticated = success;
            Debug.Log("Play Games sign in: " + success);
        });
    }

    public void UnlockAchievement(string achievementId)
    {
        Debug.Log("UnlockAchievement called. auth=" + isAuthenticated + " id=" + achievementId);
        if (!isAuthenticated) return;

        Social.ReportProgress(achievementId, 100.0f, success =>
        {
            Debug.Log("Achievement unlock: " + achievementId + " | " + success);
        });
    }

    public void UnlockFirstKill()
    {
        if (!isAuthenticated) return;

        //PlayGamesPlatform.Instance.UnlockAchievement(
        //    GPGSIds.achievement_first_kill,
        //    success => Debug.Log("First Kill unlocked: " + success)
        //);
    }

    public void UnlockFirstDeath()
    {
        if (!isAuthenticated) return;

        //PlayGamesPlatform.Instance.UnlockAchievement(
        //    GPGSIds.achievement_first_death,
        //    success => Debug.Log("First Death unlocked: " + success)
        //);
    }

    public void Set20KillsAchievementProgress(int totalKills)
    {
        if (!isAuthenticated) return;

        //PlayGamesPlatform.Instance.SetStepsAtLeast(
        //    GPGSIds.achievement_get_20_kills,
        //    totalKills,
        //    success => Debug.Log("20 Kills progress set: " + success)
        //);
    }

    public void SubmitHighScore(long score)
    {
        if (!isAuthenticated) return;

        Social.ReportScore(score, GPGSIds.leaderboard_high_scores, success =>
        {
            Debug.Log("Leaderboard submit: " + success + " | score: " + score);
        });
    }

    public void UnlockBoss1()
    {
        if (!isAuthenticated) return;

        //PlayGamesPlatform.Instance.UnlockAchievement(
        //    GPGSIds.achievement_zarlobath_the_shell_titan,
        //    success => Debug.Log("Boss 1 unlocked: " + success)
        //);
    }

    public void UnlockBoss2()
    {
        if (!isAuthenticated) return;

        //PlayGamesPlatform.Instance.UnlockAchievement(
        //    GPGSIds.achievement_auralex_the_phased_guardian,
        //    success => Debug.Log("Boss 2 unlocked: " + success)
        //);
    }

    public void UnlockFinalBoss()
    {
        if (!isAuthenticated) return;

        //PlayGamesPlatform.Instance.UnlockAchievement(
        //    GPGSIds.achievement_leviathar_prime,
        //    success => Debug.Log("Final Boss unlocked: " + success)
        //);
    }

    public void ShowAchievementsUI()
    {
        Debug.Log("ShowAchievementsUI called. isAuthenticated = " + isAuthenticated);
        if (!isAuthenticated) return;
        Social.ShowAchievementsUI();
    }

    public void ShowLeaderboardUI()
    {
        Debug.Log("ShowLeaderboardUI called. isAuthenticated = " + isAuthenticated);
        if (!isAuthenticated) return;
        Social.ShowLeaderboardUI();
    }
}