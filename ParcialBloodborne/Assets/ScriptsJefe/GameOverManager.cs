using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;

    void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        Time.timeScale = 1f; // por si saliste de una partida pausada
    }

    public void ShowGameOver()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;

        // 👇 IMPORTANTÍSIMO: liberar y mostrar el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        // Si tu controlador de player usa cursor bloqueado,
        // lo puedes volver a bloquear aquí si quieres:
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}

