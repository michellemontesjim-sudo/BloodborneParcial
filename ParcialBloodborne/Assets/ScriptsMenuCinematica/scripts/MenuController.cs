using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public string cinematicSceneName = "cinematica"; // Cambia a tu nombre real EXACTO

    public void StartCinematic()
    {
        SceneManager.LoadScene(cinematicSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");

        // Si estás dentro del editor, detiene el modo Play
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si está compilado, cierra la aplicación
        Application.Quit();
#endif
    }
}
