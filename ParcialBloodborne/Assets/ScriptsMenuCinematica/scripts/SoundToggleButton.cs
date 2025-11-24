using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundToggleButton : MonoBehaviour
{
    public enum ToggleType { Music, SFX }
    public ToggleType toggleFor;

    public GameObject iconOn;
    public GameObject iconOff;

    private bool isOn = true;

    public void Toggle()
    {
        isOn = !isOn;

        if (toggleFor == ToggleType.Music)
            AudioManager.Instance.SetMusicMuted(!isOn);

        if (toggleFor == ToggleType.SFX)
            AudioManager.Instance.SetSFXMuted(!isOn);

        iconOn.SetActive(isOn);
        iconOff.SetActive(!isOn);
    }
}
