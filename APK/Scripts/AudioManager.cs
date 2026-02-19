using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Static reference so any script can easily access the AudioManager
    public static AudioManager instance;

    // Audio sources used for sound effects and background music
    public AudioSource sfxSource;     // Plays one-shot sound effects
    public AudioSource musicSource;   // Plays looping background music

    void Awake()
    {
        // Singleton pattern:
        // If an instance doesn't exist yet, set this one.
        // If one already exists, destroy this duplicate to avoid conflicts.
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Prevent AudioManager from being destroyed between scene loads
        DontDestroyOnLoad(gameObject);
    }

    // Plays a sound effect once without interrupting other sounds.
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Starts playing background music. 
    // Automatically loops and avoids restarting the same track.
    public void PlayMusic(AudioClip clip)
    {
        // If this clip is already playing, do nothing
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        // Switch to the new music track
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
}
