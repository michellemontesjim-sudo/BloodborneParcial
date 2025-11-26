using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public float runHealthThreshold = 0.3f;

    // ------------ Persecución / regreso -----------
    public float chaseRadius = 9f;          // radio máximo para seguir al jugador
    public float returnStopDistance = 0.5f; // qué tan cerca del origen se considera que llegó

    private bool canAttack = true;
    private bool isPhase2 = false;
    private bool isDead = false;
    private bool isStunned = false;
    private bool isAttacking = false;
    private Quaternion attackRotation;
    private int currentAttack = 1;

    [Header("Audio ataques")]
    public AudioSource audioSource;          // rugidos / golpes / enrage
    public AudioClip enrageClip;
    public AudioClip attack1Clip;
    public AudioClip attack2Clip;
    public AudioClip attack3Clip;
    public AudioClip attack4Clip;

    [Header("Audio movimiento")]
    public AudioSource movementAudioSource;  // para loops de pasos
    public AudioClip walkStepClip;           // heavy step (caminar)  → loop
    public AudioClip runStepClip;            // heavy step 2 (correr) → loop

    // Punto inicial y NavMesh
    private Vector3 startPosition;
    private Quaternion startRotation;
    private NavMeshAgent agent;

    // Estado simple del boss
    private enum BossState
    {
        IdleGuard,   // quieto en el punto inicial
        Chasing,     // persiguiendo al jugador
        Returning    // regresando al punto inicial
    }

    private BossState state = BossState.IdleGuard;

    // Estado del sonido de movimiento
    private enum MovementSoundState
    {
        None,
        Walking,
        Running
    }

    private MovementSoundState movementSoundState = MovementSoundState.None;

    void Start()
    {
        currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.SetInteger("NumAttack", 0);

        // Guardamos el punto inicial y la rotación con la que nace el boss
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Configurar NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false;  // nosotros controlamos la rotación
            agent.stoppingDistance = 0f;
        }

        // Audio de ataques
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Si no asignas movementAudioSource a mano y tienes 2 AudioSource en el boss,
        // intenta coger el segundo automáticamente
        if (movementAudioSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1)
                movementAudioSource = sources[1];
        }
    }

    void Update()
    {
        // Buscar player automáticamente
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                return;
        }

        if (isDead || isStunned) return;

        BossFightPlayerHealth playerHealth = player.GetComponent<BossFightPlayerHealth>();
        if (playerHealth != null && playerHealth.isDead)
        {
            animator.SetBool("isMoving", false);
            if (agent != null) agent.isStopped = true;
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distToStart = Vector3.Distance(transform.position, startPosition);

        // ---------------- CAMBIOS DE ESTADO ----------------
        switch (state)
        {
            case BossState.IdleGuard:
                // si el jugador entra al radio → perseguir
                if (distToPlayer <= chaseRadius)
                    state = BossState.Chasing;
                break;

            case BossState.Chasing:
                // si el jugador se sale del radio → volver al origen
                if (distToPlayer > chaseRadius + 0.1f)
                    state = BossState.Returning;
                break;

            case BossState.Returning:
                // si el jugador vuelve a entrar al radio → perseguir otra vez
                if (distToPlayer <= chaseRadius)
                {
                    state = BossState.Chasing;
                }
                // si ya llegó cerca del origen → quedarse de guardia
                else if (distToStart <= returnStopDistance)
                {
                    state = BossState.IdleGuard;
                }
                break;
        }

        // ---------------- COMPORTAMIENTO POR ESTADO ----------------
        switch (state)
        {
            case BossState.IdleGuard:
                BehaviourIdleGuard();
                break;

            case BossState.Chasing:
                BehaviourChasing(distToPlayer);
                break;

            case BossState.Returning:
                BehaviourReturning(distToStart);
                break;
        }

        // -------------- FASE 2 / CORRER --------------
        bool shouldRun = isPhase2;
        animator.SetBool("isRunning", shouldRun);

        if (!isPhase2 && currentHealth <= maxHealth * 0.5f)
        {
            StartPhase2();
        }

        // -------------- ANTI-BUG DE ATAQUE --------------
        ForceEndAttackIfStuck();

        // -------------- SONIDO DE MOVIMIENTO --------------
        bool movingAnim = animator.GetBool("isMoving");
        bool runningAnim = animator.GetBool("isRunning");
        UpdateMovementSound(movingAnim, runningAnim);
    }

    // --------- ESTADO: QUIETO EN EL PUNTO INICIAL ---------
    void BehaviourIdleGuard()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        animator.SetBool("isMoving", false);

        // Mirar hacia la rotación inicial
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            startRotation,
            Time.deltaTime * 5f
        );
    }

    // --------- ESTADO: PERSIGUIENDO AL JUGADOR ---------
    void BehaviourChasing(float distToPlayer)
    {
        // Si no hay NavMeshAgent, usar el movimiento viejo por si acaso
        if (agent == null)
        {
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
                // Mientras ataca, se queda con la rotación del inicio del ataque
                transform.rotation = attackRotation;
            }

            bool inAttackRangeOld = distToPlayer <= attackRange;
            bool shouldMoveOld = !isAttacking && !inAttackRangeOld;

            animator.SetBool("isMoving", shouldMoveOld);

            if (shouldMoveOld)
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
            else if (!isAttacking && canAttack && inAttackRangeOld)
            {
                StartCoroutine(Attack());
            }

            return;
        }

        // si está atacando, NO se mueve NI persigue, solo se queda plantado
        if (isAttacking)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            transform.rotation = attackRotation;
            animator.SetBool("isMoving", false);
            return;
        }

        bool inAttackRange = distToPlayer <= attackRange;

        if (inAttackRange)
        {
            // Parar el agent y atacar
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            // Antes de lanzar el ataque, mirar hacia el jugador
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    rot,
                    Time.deltaTime * 7f
                );
            }

            animator.SetBool("isMoving", false);

            if (!isAttacking && canAttack)
                StartCoroutine(Attack());
        }
        else
        {
            // Perseguir usando NavMesh
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(player.position);

            // Rotar en la dirección de movimiento
            Vector3 vel = agent.desiredVelocity;
            vel.y = 0f;

            if (vel.sqrMagnitude > 0.01f)
            {
                Quaternion rot = Quaternion.LookRotation(vel);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    rot,
                    Time.deltaTime * 7f
                );
            }

            animator.SetBool("isMoving", agent.velocity.magnitude > 0.1f);
        }
    }

    // --------- ESTADO: REGRESANDO AL PUNTO INICIAL ---------
    void BehaviourReturning(float distToStart)
    {
        if (agent == null)
            return;

        agent.isStopped = false;
        agent.speed = moveSpeed;
        agent.SetDestination(startPosition);

        if (distToStart > returnStopDistance)
        {
            // Rotar hacia el punto de origen
            Vector3 vel = agent.desiredVelocity;
            vel.y = 0f;
            if (vel.sqrMagnitude < 0.01f)
            {
                vel = startPosition - transform.position;
                vel.y = 0f;
            }

            if (vel.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(vel);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    rot,
                    Time.deltaTime * 7f
                );
            }

            animator.SetBool("isMoving", agent.velocity.magnitude > 0.1f);
        }
        else
        {
            // Ya llegó suficientemente cerca del origen
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            animator.SetBool("isMoving", false);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                startRotation,
                Time.deltaTime * 5f
            );
        }

        // En este estado NO ataca
    }

    // ------------------- ATAQUE / DAÑO -------------------
    IEnumerator Attack()
    {
        if (!canAttack || isAttacking)
            yield break;

        canAttack = false;
        isAttacking = true;

        // Guardamos la rotación con la que empieza el ataque
        attackRotation = transform.rotation;

        int num = Random.Range(1, 5);
        currentAttack = num;

        animator.SetInteger("NumAttack", currentAttack);

        // Sonido según el ataque
        switch (currentAttack)
        {
            case 1:
                PlayClip(attack1Clip);   // rugido para puño 3
                break;
            case 2:
                PlayClip(attack2Clip);   // rugido para puño 1
                break;
            case 3:
                PlayClip(attack3Clip);   // rugido para puño 2
                break;
            case 4:
                PlayClip(attack4Clip);   // rugido para puño 3
                break;
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetInteger("NumAttack", 0);
    }

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
            EndAttack();
        }
    }

    void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    void UpdateMovementSound(bool isMoving, bool isRunning)
    {
        if (movementAudioSource == null)
            return;

        // si está muerto, aturdido o atacando, paramos los pasos
        if (isDead || isStunned || isAttacking)
        {
            if (movementAudioSource.isPlaying)
                movementAudioSource.Stop();

            movementSoundState = MovementSoundState.None;
            return;
        }

        MovementSoundState desiredState = MovementSoundState.None;

        if (isMoving)
        {
            desiredState = isRunning ? MovementSoundState.Running : MovementSoundState.Walking;
        }

        if (desiredState == movementSoundState)
            return; // ya estamos en ese estado

        switch (desiredState)
        {
            case MovementSoundState.None:
                movementAudioSource.Stop();
                break;

            case MovementSoundState.Walking:
                if (walkStepClip != null)
                {
                    movementAudioSource.clip = walkStepClip;
                    movementAudioSource.loop = true;
                    movementAudioSource.Play();
                }
                break;

            case MovementSoundState.Running:
                if (runStepClip != null)
                {
                    movementAudioSource.clip = runStepClip;
                    movementAudioSource.loop = true;
                    movementAudioSource.Play();
                }
                break;
        }

        movementSoundState = desiredState;
    }

    public void AnalizarAtaque()
    {
        HashSet<BossFightPlayerHealth> yaGolpeados = new HashSet<BossFightPlayerHealth>();

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

    void RevisarGolpe(Transform point, HashSet<BossFightPlayerHealth> yaGolpeados)
    {
        if (point == null) return;

        Collider[] hits = Physics.OverlapSphere(point.position, attackRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            BossFightPlayerHealth playerHealth = hit.GetComponentInParent<BossFightPlayerHealth>();
            if (playerHealth == null) continue;

            if (yaGolpeados.Contains(playerHealth)) continue;
            yaGolpeados.Add(playerHealth);

            if (playerHealth.isParrying)
            {
                StartCoroutine(Stun(2f));
            }
            else
            {
                playerHealth.TomarDaño(attackDamage);
                Debug.Log("Boss golpea al jugador. Daño: " + attackDamage +
                          " | Vida jugador ahora: " + playerHealth.currentHealth);
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
        PlayClip(enrageClip);    // sonido "enrage"

        isPhase2 = true;
        moveSpeed *= 1.5f;
        attackDamage *= 2;
        attackCooldown *= 0.8f;
        animator.SetTrigger("Enrage");

        GetComponentInChildren<Renderer>().material.color = Color.red;

        if (agent != null) agent.speed = moveSpeed;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        animator.SetTrigger("Hit");

        Debug.Log("Boss recibe " + amount + " de daño. Vida boss ahora: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        Debug.Log("Boss derrotado");

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        
        StartCoroutine(ShowVictoryAfterDelay());
    }

    IEnumerator ShowVictoryAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        VictoryManager vm = FindObjectOfType<VictoryManager>();
        if (vm != null)
            vm.ShowVictory();

        this.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        // Radio de persecución
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        if (rightHandPoint != null)
            Gizmos.DrawWireSphere(rightHandPoint.position, attackRadius);
        if (leftHandPoint != null)
            Gizmos.DrawWireSphere(leftHandPoint.position, attackRadius);
        if (rightFootPoint != null)
            Gizmos.DrawWireSphere(rightFootPoint.position, attackRadius);
    }
}
