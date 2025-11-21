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
    private bool isAttacking = false;      // para no moverse mientras ataca
    private Quaternion attackRotation;     // rotación fija durante el ataque
    private int currentAttack = 1;         // cuál ataque está usando (1-4)

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null) return;
        if (isDead || isStunned) return;

        PlayerHealthhhh playerHealth = player.GetComponent<PlayerHealthhhh>();
        if (playerHealth != null && playerHealth.isDead)
        {
            animator.SetBool("isMoving", false);
            return; // no atacar si el jugador está muerto
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Rotar hacia el jugador solo si NO está atacando
        if (!isAttacking)
        {
            Vector3 lookDir = (player.position - transform.position);
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(lookDir),
                    Time.deltaTime * 5f);
        }
        else
        {
            // Mantener la rotación guardada al empezar el ataque
            transform.rotation = attackRotation;
        }

        // ----------- MOVIMIENTO + isMoving -----------
        bool shouldMove = distance > attackRange && !isAttacking;

        // el Animator siempre sabe si se está moviendo o no
        animator.SetBool("isMoving", shouldMove);

        if (shouldMove)
        {
            // solo mover el boss si realmente debe moverse
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            // atacar solo si está realmente dentro del rango (con pequeño margen)
            if (canAttack && !isAttacking && distance <= attackRange - 0.2f)
            {
                StartCoroutine(Attack());
            }
        }
        // ---------------------------------------------

        // decidir si debe correr o no (según % de vida)
        bool shouldRun = currentHealth <= maxHealth * runHealthThreshold;
        animator.SetBool("isRunning", shouldRun);

        // Revisar cambio de fase (50% de vida)
        if (!isPhase2 && currentHealth <= maxHealth * 0.5f)
        {
            StartPhase2();
        }

        // failsafe de emergencia si por bug el boss se queda pegado en un ataque
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (isAttacking && info.normalizedTime >= 1f && !info.IsTag("Attack"))
        {
            isAttacking = false;
            canAttack = true;
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;

        // Guardar la rotación actual del jefe (mirando hacia donde atacó)
        attackRotation = transform.rotation;

        int num = Random.Range(1, 5); // 1,2,3,4
        currentAttack = num;

        animator.SetInteger("NumAttack", num);
        animator.SetTrigger("Attack");

        // solo controlamos el cooldown aquí
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        // OJO: isAttacking se pone a false en EndAttack() (evento de animación)
    }

    // Llamado desde un Animation Event al final de cada animación de ataque
    public void EndAttack()
    {
        isAttacking = false;
        canAttack = true;
    }

    // Llamado desde un Animation Event en el frame del golpe
    public void AnalizarAtaque()
    {
        // Para este evento queremos evitar daño múltiple por estar dentro de varios puntos
        HashSet<PlayerHealthhhh> yaGolpeados = new HashSet<PlayerHealthhhh>();

        switch (currentAttack)
        {
            case 1:
                // ATTACK 1: salto -> usa las 3 hitbox pero solo 1 daño por jugador
                RevisarGolpe(rightHandPoint, yaGolpeados);
                RevisarGolpe(leftHandPoint, yaGolpeados);
                RevisarGolpe(rightFootPoint, yaGolpeados);
                break;

            case 2:
                // ATTACK 2: solo mano derecha
                RevisarGolpe(rightHandPoint, yaGolpeados);
                break;

            case 3:
                // ATTACK 3: solo pie derecho
                RevisarGolpe(rightFootPoint, yaGolpeados);
                break;

            case 4:
                // ATTACK 4: combo mano derecha + izquierda
                // En cada EVENTO de la animación se evalúan ambas.
                // Si tienes 2 eventos (uno en cada golpe), el jugador
                // podrá recibir 2 daños: uno por cada evento.
                RevisarGolpe(rightHandPoint, yaGolpeados);
                RevisarGolpe(leftHandPoint, yaGolpeados);
                break;
        }
    }

    void RevisarGolpe(Transform point, HashSet<PlayerHealthhhh> yaGolpeados)
    {
        if (point == null) return;

        Collider[] hits = Physics.OverlapSphere(point.position, attackRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            PlayerHealthhhh playerHealth = hit.GetComponent<PlayerHealthhhh>();
            if (playerHealth == null) continue;

            // Evitar que el mismo Player reciba múltiples golpes en este mismo evento
            if (yaGolpeados.Contains(playerHealth)) continue;
            yaGolpeados.Add(playerHealth);

            if (playerHealth.isParrying)
            {
                Debug.Log("Parry exitoso! Boss aturdido.");
                StartCoroutine(Stun(2f));
            }
            else
            {
                playerHealth.Damage(attackDamage);
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



