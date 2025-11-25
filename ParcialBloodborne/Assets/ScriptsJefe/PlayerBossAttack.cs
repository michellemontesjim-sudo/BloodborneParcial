using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBossAttack : MonoBehaviour
{
    [Header("Referencia al punto de ataque EXISTENTE")]
    public Transform attackPoint;      // aquí arrastras 'puntoAtaque' (el mismo de PlayerCombat)
    public float attackRadius = 1.13f; // pon el mismo valor que en PlayerCombat
    public int attackDamage = 30;      // pon el mismo valor que en PlayerCombat

    [Header("Input")]
    public KeyCode attackKey = KeyCode.Mouse0; // click izquierdo por defecto

    void Update()
    {
        // Leemos el input, igual que harías en PlayerCombat, pero sin tocar ese script
        if (Input.GetKeyDown(attackKey))
        {
            DoAttack();
        }
    }

    void DoAttack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("[PlayerBossAttack] No hay attackPoint asignado.");
            return;
        }

        // Detectar todo lo que esté en el radio
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius);

        foreach (var hit in hits)
        {
            // Buscamos un BossController en el objeto o en sus padres
            BossController boss = hit.GetComponentInParent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
                Debug.Log("Jugador golpea al Boss. Daño: " + attackDamage +
                          " | Vida boss ahora: " + boss.currentHealth);
            }
        }
    }

    // Solo para ver el área en la Scene
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
