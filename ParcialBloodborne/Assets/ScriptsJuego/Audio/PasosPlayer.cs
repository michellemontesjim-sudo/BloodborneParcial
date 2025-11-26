using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasosPlayer : MonoBehaviour
{
    public CharacterController controller;
    public AudioSource audioSource;
    public AudioClip[] sonidosPasos;
    public float intervaloPasos = 0.4f;

    float contador = -1f;

    void Update()
    {
        bool estaCaminando = controller.velocity.magnitude > 0.2f && controller.isGrounded;

        if (estaCaminando)
        {
            if (contador < 0f)
            {
                // Primer paso al comenzar a caminar
                audioSource.PlayOneShot(sonidosPasos[Random.Range(0, sonidosPasos.Length)]);
                contador = intervaloPasos;
            }
            else
            {
                contador -= Time.deltaTime;
                if (contador <= 0f)
                {
                    audioSource.PlayOneShot(sonidosPasos[Random.Range(0, sonidosPasos.Length)]);
                    contador = intervaloPasos;
                }
            }
        }
        else
        {
            // jugador parado → detener sistema y esperar el próximo inicio de movimiento
            contador = -1f;
        }
    }
}
