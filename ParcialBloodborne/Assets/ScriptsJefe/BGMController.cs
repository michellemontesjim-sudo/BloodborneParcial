using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    AudioSource bgm;

    void Awake()
    {
        bgm = GetComponent<AudioSource>();
    }

    public void StopMusic()
    {
        if (bgm.isPlaying)
            bgm.Stop();
    }
}

