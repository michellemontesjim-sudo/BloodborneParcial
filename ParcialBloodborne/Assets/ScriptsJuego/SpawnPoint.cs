using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad = "Final Boss";

    private void OnTriggerEnter(Collider other)
    {
        // Subimos al objeto raíz del player
        Transform root = other.transform.root;

        if (root.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
