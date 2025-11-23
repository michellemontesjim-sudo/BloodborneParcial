using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        // Opcional: solo cambiar si quien entra es el jugador
        if (root.CompareTag("Player"))
        {
            SceneManager.LoadScene("Final Boss");
        }
    }
}
