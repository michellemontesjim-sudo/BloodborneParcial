using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;
    public Animator animator;
    public float maxHealth = 100f;
    public float currentHealth;
    public float moveSpeed = 1f;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    public float attackCooldown = 2f;

    public Transform rightHandPoint;
    public Transform leftHandPoint;
    public Transform rightFootPoint;
    public float attackRadius = 1f;

    public float runHealthThreshold = 0.3f; // porcentaje de vida para empezar a correr (30%)

    private bool canAttack = true;
    private bool isPhase2 = false;
    private bool isDead = false;
    private bool isStunned = false;
    private bool isAttacking = false;
    private Quaternion attackRotation;
    private int currentAttack = 1;

    void Start()
    {
        currentHealth = maxHealth;

        // Por seguridad: empezamos sin ataque seleccionado
        animator.SetInteger("NumAttack", 0);
    }

    void Update()
    {
        if (player == null) return;
        if (isDead || isStunned) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.isDead)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // ---------------- ROTACIÓN ----------------
        if (!isAttacking)
        {
            Vector3 lookDir = (player.position - transform.position);
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(lookDir),
                    Time.deltaTime * 5f
                );
            }
        }
        else
        {
            transform.rotation = attackRotation;
        }

        // -------------- MOVIMIENTO / ATAQUE --------------
        bool inAttackRange = distance <= attackRange;
        bool shouldMove = !isAttacking && !inAttackRange;

        animator.SetBool("isMoving", shouldMove);

        if (shouldMove)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            if (!isAttacking && canAttack && inAttackRange)
            {
                StartCoroutine(Attack());
            }
        }

        // -------------- FASE 2 / CORRER --------------
        bool shouldRun = currentHealth <= maxHealth * runHealthThreshold;
        animator.SetBool("isRunning", shouldRun);

        if (!isPhase2 && currentHealth <= maxHealth * 0.5f)
        {
            StartPhase2();
        }

        // -------------- ANTI-BUG DE ATAQUE --------------
        ForceEndAttackIfStuck();
    }

    IEnumerator Attack()
    {
        Debug.Log("START ATTACK");

        if (!canAttack || isAttacking)
            yield break;   // por seguridad, evitar ataques dobles

        canAttack = false;
        isAttacking = true;

        // Guardar rotación con la que empieza el ataque
        attackRotation = transform.rotation;

        // Elegir un ataque aleatorio entre 1 y 4
        int num = Random.Range(1, 5);
        currentAttack = num;

        // Esto es lo único que necesita el Animator para ir al ataque
        animator.SetInteger("NumAttack", currentAttack);

        // Solo manejamos el cooldown aquí
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;

        Debug.Log("END ATTACK");
    }


    // Llamado por Animation Event (si lo tienes) PERO
    // también lo llamamos nosotros desde ForceEndAttackIfStuck()
    public void EndAttack()
    {
        Debug.Log("EndAttack EVENT");

        isAttacking = false;
        animator.SetInteger("NumAttack", 0);   // volver a “sin ataque”
    }


    // POR SI EL EVENTO FALLA EN ALGÚN ATAQUE
    void ForceEndAttackIfStuck()
    {
        if (!isAttacking) return;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        bool isInAttackState =
            info.IsName("attack 1") ||
            info.IsName("attack 2") ||
            info.IsName("attack 3") ||
            info.IsName("attack 4");

        if (isInAttackState && info.normalizedTime >= 0.98f)
        {
            Debug.Log("Force EndAttack by code (normalizedTime) " + info.normalizedTime);
            EndAttack();
        }
    }

    // Llamado desde Animation Event para aplicar daño
    public void AnalizarAtaque()
    {
        HashSet<PlayerHealth> yaGolpeados = new HashSet<PlayerHealth>();

        switch (currentAttack)
        {
            case 1:
                RevisarGolpe(rightHandPoint, yaGolpeados);
                RevisarGolpe(leftHandPoint, yaGolpeados);
                RevisarGolpe(rightFootPoint, yaGolpeados);
                break;

            case 2:
                RevisarGolpe(rightHandPoint, yaGolpeados);
                break;

            case 3:
                RevisarGolpe(rightFootPoint, yaGolpeados);
                break;

            case 4:
                RevisarGolpe(rightHandPoint, yaGolpeados);
                RevisarGolpe(leftHandPoint, yaGolpeados);
                break;
        }
    }

    void RevisarGolpe(Transform point, HashSet<PlayerHealth> yaGolpeados)
    {
        if (point == null) return;

        Collider[] hits = Physics.OverlapSphere(point.position, attackRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth == null) continue;

            if (yaGolpeados.Contains(playerHealth)) continue;
            yaGolpeados.Add(playerHealth);

            if (playerHealth.isParrying)
            {
                Debug.Log("Parry exitoso! Boss aturdido.");
                StartCoroutine(Stun(2f));
            }
            else
            {
                playerHealth.TomarDaño(attackDamage);
                Debug.Log("Boss golpea al jugador!");
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
        Gizmos.color = Color.red;

        if (rightHandPoint != null)
            Gizmos.DrawWireSphere(rightHandPoint.position, attackRadius);

        if (leftHandPoint != null)
            Gizmos.DrawWireSphere(leftHandPoint.position, attackRadius);

        if (rightFootPoint != null)
            Gizmos.DrawWireSphere(rightFootPoint.position, attackRadius);
    }
}




