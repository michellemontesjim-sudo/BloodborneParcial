using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;   // para el video de créditos

public class VictoryManager : MonoBehaviour
{
    [Header("UI Victoria")]
    public GameObject victoryPanel;

    [Header("Créditos")]
    public GameObject creditsPanel;      // otro panel con el video
    public VideoPlayer creditsVideo;     // componente VideoPlayer 

    void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    // Llamado cuando ganes
    public void ShowVictory()
    {
        Time.timeScale = 0f; // pausa el juego

        // mostrar mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        FindObjectOfType<BGMController>().StopMusic();

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    // Botón "Volver al menú"
    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); ;
    }

    // Botón "Ver créditos"
    public void OnClickCredits()
    {
        Time.timeScale = 1f;

        // mostrar mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(true);

        if (creditsVideo != null)
            creditsVideo.Play();
    }

}
