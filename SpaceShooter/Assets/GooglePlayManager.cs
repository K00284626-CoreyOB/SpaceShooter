using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
public class GooglePlayManager : MonoBehaviour
{
    void Start()
    {
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            Debug.Log($"Google Play sign-in result: {status}");
        });
    }
}