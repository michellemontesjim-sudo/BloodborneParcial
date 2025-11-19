using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;
    public Animator animator;
    public float maxHealth = 100f;
    public float currentHealth;
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public int attackDamage = 20;
    public float attackCooldown = 2f;

    public Transform attackPoint;
    public float attackRadius = 1f;

    public float runHealthThreshold = 0.3f; // NUEVO: porcentaje de vida para empezar a correr (30%)

    private bool canAttack = true;
    private bool isPhase2 = false;
    private bool isDead = false;
    private bool isStunned = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

        // 🔴 Mientras no tengas el player real:
        if (player == null) return;

        if (isDead || isStunned) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null && playerHealth.isDead)
        {
            animator.SetBool("isMoving", false);
            return; // no atacar si el jugador está muerto
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Rotar hacia el jugador
        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

        if (distance > attackRange)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
            if (canAttack)
                StartCoroutine(Attack());
        }

        // NUEVO: decidir si debe correr o no (según % de vida)
        bool shouldRun = currentHealth <= maxHealth * runHealthThreshold;
        animator.SetBool("isRunning", shouldRun);

        // Revisar cambio de fase
        if (!isPhase2 && currentHealth <= maxHealth * 0.5f)
        {
            StartPhase2();
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        int num = Random.Range(1, 5);
        animator.SetInteger("NumAttack", num);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void AnalizarAtaque()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Si el jugador está haciendo parry, el boss se aturde
                    if (playerHealth.isParrying)
                    {
                        Debug.Log("⚡ Parry exitoso! Boss aturdido.");
                        StartCoroutine(Stun(2f));
                    }
                    else
                    {
                        playerHealth.Damage(attackDamage);
                        Debug.Log("Boss golpea al jugador!");
                    }
                }
            }
        }
    }

    IEnumerator Stun(float duration)
    {
        isStunned = true;
        animator.SetTrigger("Stunned");
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    void StartPhase2()
    {
        isPhase2 = true;
        moveSpeed *= 1.5f;
        attackDamage *= 2;
        attackCooldown *= 0.8f;
        animator.SetTrigger("Enrage");
        Debug.Log("¡El Boss entra en la Fase 2!");
        GetComponentInChildren<Renderer>().material.color = Color.red;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        Debug.Log("Boss derrotado");
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}

