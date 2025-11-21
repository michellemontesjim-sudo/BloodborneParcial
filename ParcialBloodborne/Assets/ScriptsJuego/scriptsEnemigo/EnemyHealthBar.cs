using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public Image barraVida;
    public Canvas canvas;

    void Update()
    {
        if (enemyHealth == null) return;

        float vidaNormalizada = enemyHealth.vidaActual / enemyHealth.saludMax;
        barraVida.fillAmount = vidaNormalizada;

        // Que siempre mire a la cámara
        canvas.transform.LookAt(Camera.main.transform);
        canvas.transform.Rotate(0, 180, 0);
    }
}
