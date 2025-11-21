using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float saludMax = 100f;
    public float vidaActual;

    [Header("UI")]
    public Image barraVida;        // arrastrar aquí la imagen con Fill
    public Transform uiBarra;      // el CanvasVida

    Transform cam;

    void Start()
    {
        vidaActual = saludMax;
        cam = Camera.main.transform;
    }

    void Update()
    {
        // La barra siempre mira a la cámara
        if (uiBarra != null)
            uiBarra.LookAt(cam);
    }

    public void RecibirDaño(float daño)
    {
        vidaActual -= daño;

        if (barraVida != null)
            barraVida.fillAmount = vidaActual / saludMax;

        if (vidaActual <= 0)
        {
            GetComponent<EnemigoIA>().Muerte();
            Destroy(gameObject, 3f);
        }
    }
}
