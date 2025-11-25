using System.Collections;            // ⬅ IMPORTANTE para IEnumerator
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;  // Panel de Game Over
    public GameObject hudUI;       // HUD con barras de vida, etc.

    void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (hudUI != null)
            hudUI.SetActive(true);

        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        // ocultar HUD
        if (hudUI != null)
            hudUI.SetActive(false);

        // mostrar panel de Game Over
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // pausar juego
        Time.timeScale = 0f;

        // mostrar mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // reproducir sonido de muerte (AudioSource en este mismo objeto, opcional)
        AudioSource deathAudio = GetComponent<AudioSource>();
        if (deathAudio != null)
            deathAudio.Play();

        // fade–out de la música de fondo
        StartCoroutine(FadeOutMusic());
    }

    IEnumerator FadeOutMusic()
    {
        BGMController bgmCtrl = FindObjectOfType<BGMController>();
        if (bgmCtrl == null || bgmCtrl.bgm == null)
            yield break;

        AudioSource audio = bgmCtrl.bgm;
        float startVolume = audio.volume;

        float duration = 1.5f;  // duración del fade
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;              // funciona aunque timeScale = 0
            audio.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        audio.Stop();
        audio.volume = startVolume;                   // dejarlo listo por si se usa en otra escena
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
