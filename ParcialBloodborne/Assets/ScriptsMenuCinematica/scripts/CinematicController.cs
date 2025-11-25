using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CinematicController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button skipButton;
    public Button startButton;

    [Header("Nombre de la escena del juego")]
    public string gameSceneName = "Juego"; // Cambia esto al nombre real

    void Start()
    {
        videoPlayer.loopPointReached += VideoFinished;

        skipButton.onClick.AddListener(SkipCinematic);
        startButton.onClick.AddListener(StartGame);
    }

    void SkipCinematic()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void VideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
