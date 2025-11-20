using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float salud = 100f;
    private EnemigoIA ai;

    void Start()
    {
        ai = GetComponent<EnemigoIA>();
    }

    public void RecibirDaño(float daño)
    {
        salud -= daño;

        if (salud <= 0)
        {
            ai.Muerte();       // que IA maneje la animación
            Destroy(gameObject, 3f);
        }
    }
}
