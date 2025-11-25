using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXToggle : MonoBehaviour
{
    public AudioSource sfxSource;

    public void TurnOnSFX()
    {
        sfxSource.mute = false;
    }

    public void TurnOffSFX()
    {
        sfxSource.mute = true;
    }
}
