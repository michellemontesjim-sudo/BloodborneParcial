using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        // Opcional: solo cambiar si quien entra es el jugador
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Final Boss");
        }
    }
}
