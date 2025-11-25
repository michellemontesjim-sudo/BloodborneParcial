using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthhh : MonoBehaviour
{
    public bool isDead = false;
    public bool isParrying = false;
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Jugador recibe " + amount + " de daño. Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            Debug.Log("Jugador de prueba murió.");
            // aquí luego pondrán animación de muerte real
        }
    }
}
