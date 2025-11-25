using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    [Header("Fuente de música de fondo")]
    public AudioSource bgm;   // arrastra aquí TU AudioSource de la música

    void Awake()
    {
        // Si no lo asignas en el inspector, intenta buscarlo en el mismo objeto
        if (bgm == null)
            bgm = GetComponent<AudioSource>();
    }

    public void StopMusic()
    {
        if (bgm != null && bgm.isPlaying)
            bgm.Stop();
    }
}

