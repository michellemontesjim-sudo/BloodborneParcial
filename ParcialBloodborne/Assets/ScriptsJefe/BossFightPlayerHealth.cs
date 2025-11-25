using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightPlayerHealth : MonoBehaviour
{
    public enum CombatState
    {
        Normal,
        Hurt,
        Dead
    }

    [Header("Salud")]
    public float maxHealth = 150f;
    public float currentHealth = 150f;

    [Header("Estado")]
    public CombatState state = CombatState.Normal;
    public bool isDead = false;
    public bool isParrying = false;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // llamado por el boss: playerHealth.TomarDaño(attackDamage);
    public void TomarDaño(int daño)
    {
        // 1) ya está muerto
        if (state == CombatState.Dead)
            return;

        // 2) está haciendo parry → no recibe daño
        if (isParrying)
        {
            Debug.Log("Parry bloqueó el daño!");
            return;
        }

        // 3) ya está en animación de dolor → no acumular golpes
        if (state == CombatState.Hurt)
            return;

        // 4) aplicar daño
        currentHealth -= daño;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Jugador recibe " + daño + " de daño. Vida jugador ahora: " + currentHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(HurtState());
        }
        else
        {
            state = CombatState.Dead;
            isDead = true;

            if (anim != null)
                anim.SetTrigger("Die");

            // Activar pantalla de Game Over
            FindObjectOfType<GameOverManager>().ShowGameOver();
        }
    }

    IEnumerator HurtState()
    {
        state = CombatState.Hurt;

        if (anim != null)
            anim.SetTrigger("Herido");   

        yield return new WaitForSeconds(0.3f); // duración anim de golpe

        state = CombatState.Normal;
    }
}
