using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;  // Panel de Game Over
    public GameObject hudUI;       // El objeto HUD con las barras

    void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (hudUI != null)
            hudUI.SetActive(true);   // HUD visible al inicio

        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        if (hudUI != null)
            hudUI.SetActive(false);  // ocultar barras

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        FindObjectOfType<BGMController>().StopMusic();
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        // Si recargas la escena, el Start volverá a encender el HUD
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

}


