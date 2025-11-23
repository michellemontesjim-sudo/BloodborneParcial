using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportAfterLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Final Boss")
        {
            StartCoroutine(TeleportWhenReady());
        }
    }

    IEnumerator TeleportWhenReady()
    {
        // Esperamos varios frames para asegurarnos que el piso YA EXISTE
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.05f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawn = GameObject.FindGameObjectWithTag("SpawnPoint");

        if (player == null || spawn == null)
        {
            Debug.LogWarning("Player o SpawnPoint no encontrados.");
            yield break;
        }

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        // Teleport
        player.transform.position = spawn.transform.position;
        player.transform.rotation = spawn.transform.rotation;

        // Esperamos UN FRAME para que el CharacterController se ajuste al nuevo piso
        yield return null;

        if (cc != null)
            cc.enabled = true;

        Debug.Log("Player teletransportado correctamente y sin caer.");
    }
}
