using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- CONTROL DE MÚSICA ---
    public void SetMusicMuted(bool muted)
    {
        musicSource.mute = muted;
    }

    // --- CONTROL DE EFECTOS ---
    public void SetSFXMuted(bool muted)
    {
        sfxSource.mute = muted;
    }

    // --- REPRODUCIR EFECTOS ---
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
