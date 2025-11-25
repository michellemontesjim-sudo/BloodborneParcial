using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    [Header("Referencias")]
    public NavMeshAgent agente;
    public Animator anim;
    public Transform objetivo; // El player

    [Header("Stats del enemigo")]
    public float rangoDeteccion = 12f;
    public float rangoAtaque = 2f;
    public float daño = 15f;
    public float tiempoEntreAtaques = 1.2f;

    [Header("Patrullaje")]
    public Transform[] puntosPatrulla;
    public float esperaPatrulla = 2f;
    int indicePatrulla = 0;
    bool esperando = false;

    bool yaAtaco = false;

    public bool isStunned = false;
    public float stunDuration = 5f;
    private Coroutine stunRoutine;

    public AudioSource audioSource;
    public AudioClip sonidoAtaque;

    public enum EnemyState
    {
        Normal,
        Stunned,
        Dead
    }

    public EnemyState state = EnemyState.Normal;

    void Start()
    {
        if (objetivo == null)
            objetivo = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isStunned) return;

        if (objetivo == null) return;

        // 1️⃣ Revisar si el jugador está muerto
        PlayerHealth ph = objetivo.GetComponent<PlayerHealth>();
        if (ph != null && ph.state == PlayerHealth.CombatState.Dead)
        {
            // detener IA del enemigo
            agente.isStopped = true;
            anim.SetBool("isMoving", false);

            // evitar ataques
            yaAtaco = true;

            return;
        }

        // 2️⃣ Comportamiento normal
        float distancia = Vector3.Distance(transform.position, objetivo.position);

        if (distancia <= rangoDeteccion && distancia > rangoAtaque)
        {
            Perseguir();
        }
        else if (distancia <= rangoAtaque)
        {
            Atacar();
        }
        else
        {
            Idle();
        }

        //float distancia = Vector3.Distance(transform.position, objetivo.position);

        // Prioridad 1: Perseguir jugador
        if (distancia <= rangoDeteccion && distancia > rangoAtaque)
        {
            Perseguir();
        }
        // Prioridad 2: Atacar
        else if (distancia <= rangoAtaque)
        {
            Atacar();
        }
        // Prioridad 3: Patrullar
        else
        {
            Patrullar();
        }
    }

    // ---------- PATRULLAJE ----------
    void Patrullar()
    {
        if (puntosPatrulla.Length == 0)
        {
            Idle();
            return;
        }

        if (esperando) return;

        anim.SetBool("isMoving", true);
        agente.isStopped = false;

        agente.SetDestination(puntosPatrulla[indicePatrulla].position);

        if (!agente.pathPending && agente.remainingDistance < 0.4f)
        {
            StartCoroutine(EsperarYSeguir());
        }
    }

    IEnumerator EsperarYSeguir()
    {
        esperando = true;
        anim.SetBool("isMoving", false);

        yield return new WaitForSeconds(esperaPatrulla);

        indicePatrulla++;
        if (indicePatrulla >= puntosPatrulla.Length)
            indicePatrulla = 0;

        esperando = false;
    }

    // ---------- PERSEGUIR ----------
    void Perseguir()
    {
        agente.isStopped = false;
        agente.SetDestination(objetivo.position);

        anim.SetBool("isMoving", true);
    }

    // ---------- IDLE ----------
    void Idle()
    {
        agente.isStopped = true;
        anim.SetBool("isMoving", false);
    }

    // ---------- ATAQUE ----------
    void Atacar()
    {
        if (isStunned) return;
        agente.isStopped = true;

        transform.LookAt(objetivo);
        anim.SetBool("isMoving", false);

        
        if (!yaAtaco)
        {
            anim.SetTrigger("isAttacking");
            StartCoroutine(HacerDaño());
            audioSource.PlayOneShot(sonidoAtaque);
        }
    }

    IEnumerator HacerDaño()
    {
        var playerCombat = objetivo.GetComponent<PlayerCombat>();
        // --- PARRY ÉXITOSO ---
        if (playerCombat.isParrying)
        {
            Debug.Log("PARRY! Enemigo stuneado!");

            // Stun al enemigo
            EnemigoIA enemy = GetComponent<EnemigoIA>();
            if (enemy != null)
                enemy.StunEnemy();

            yield break; // ✔ Termina corrutina sin hacer daño
        }
        yaAtaco = true;
        yield return new WaitForSeconds(0.5f);

        float distancia = Vector3.Distance(transform.position, objetivo.position);

        if (distancia <= rangoAtaque + 0.3f)
        {
            PlayerHealth ph = objetivo.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TomarDaño(daño);
        }

        yield return new WaitForSeconds(tiempoEntreAtaques);
        yaAtaco = false;
        
    }
    // ---------- MUERTE ----------
    public void Muerte()
    {
        agente.isStopped = true;
        anim.SetTrigger("Death");
        Destroy(gameObject, 3f);
    }


    public void StunEnemy()
    {
        if (stunRoutine != null)
            StopCoroutine(stunRoutine);

        stunRoutine = StartCoroutine(StunCoroutine());
    }

    IEnumerator StunCoroutine()
    {
        isStunned = true;

        anim.SetBool("Stunned", true);
        agente.isStopped = true; // detener movimiento

        yield return new WaitForSeconds(stunDuration);

        anim.SetBool("Stunned", false);
        agente.isStopped = false;

        isStunned = false;
    }
}
