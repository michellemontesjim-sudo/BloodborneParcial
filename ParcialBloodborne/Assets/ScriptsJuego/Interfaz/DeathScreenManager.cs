using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    public CanvasGroup deathPanel;   // CanvasGroup del panel de muerte
    public float fadeDuration = 1.2f;

    private bool shown = false;

    public void ShowDeathScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!shown)
        {
            shown = true;
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        deathPanel.gameObject.SetActive(true);

        float t = 0;

        // Activar interacción visual
        deathPanel.interactable = false;
        deathPanel.blocksRaycasts = false;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            deathPanel.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        // Al terminar el fade, actívamos interacción del menú
        deathPanel.interactable = true;
        deathPanel.blocksRaycasts = true;

         // Pausar el juego
    }

    // Botón: Reintentar
    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Botón: Ir al menú
    public void GoToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu"); // cambia nombre si tu escena es otra
    }
}
