using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VictoryManager : MonoBehaviour
{
    [Header("UI Victoria")]
    public GameObject victoryPanel;

    [Header("Créditos")]
    public GameObject creditsPanel;
    public VideoPlayer creditsVideo;
    public string mainMenuSceneName = "Menu";

    void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        StartCoroutine(FadeOutMusic());
    }

    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnClickCredits()
    {
        Time.timeScale = 1f;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(true);

        if (creditsVideo != null)
            creditsVideo.Play();
    }

    IEnumerator FadeOutMusic()
    {
        BGMController bgmCtrl = FindObjectOfType<BGMController>();
        if (bgmCtrl == null || bgmCtrl.bgm == null)
            yield break;

        AudioSource audio = bgmCtrl.bgm;
        float startVolume = audio.volume;

        float duration = 1.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        audio.Stop();
        audio.volume = startVolume;
    }
}
