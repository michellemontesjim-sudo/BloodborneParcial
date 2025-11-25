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
}
