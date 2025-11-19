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
    public float vida = 100f;
    public float rangoDeteccion = 12f;
    public float rangoAtaque = 2f;
    public float daño = 15f;
    public float tiempoEntreAtaques = 1.2f;

    bool yaAtaco = false;

    void Start()
    {
        if (objetivo == null)
        {
            objetivo = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (vida <= 0) return;

        float distancia = Vector3.Distance(transform.position, objetivo.position);

        // 1️⃣ Detección del jugador
        if (distancia <= rangoDeteccion && distancia > rangoAtaque)
        {
            Perseguir();
        }
        // 2️⃣ Ataque
        else if (distancia <= rangoAtaque)
        {
            Atacar();
        }
        // 3️⃣ Idle
        else
        {
            Idle();
        }
    }

    void Perseguir()
    {
        agente.isStopped = false;
        agente.SetDestination(objetivo.position);

        anim.SetBool("isMoving", true);
    }

    void Idle()
    {
        agente.isStopped = true;

        anim.SetBool("isMoving", false);
    }

    void Atacar()
    {
        agente.isStopped = true;

        transform.LookAt(objetivo);

        anim.SetBool("isMoving", false); // No se mueve mientras ataca

        if (!yaAtaco)
        {
            anim.SetTrigger("isAttacking");  
            StartCoroutine(HacerDaño());
        }
    }

    System.Collections.IEnumerator HacerDaño()
    {
        yaAtaco = true;

        // Espera medio segundo para sincronizar con la animación
        yield return new WaitForSeconds(0.5f);

        float distancia = Vector3.Distance(transform.position, objetivo.position);

        if (distancia <= rangoAtaque + 0.3f)
        {
            // Golpea al player
            PlayerHealth ph = objetivo.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TomarDaño(daño);
            }
        }

        yield return new WaitForSeconds(tiempoEntreAtaques);
        yaAtaco = false;
    }

    // Llamar desde animación o cuando reciba daño
    public void TomarDaño(float cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            Muerte();
        }
    }

    public void Muerte()
    {
        agente.isStopped = true;
        anim.SetTrigger("Death");

        Destroy(gameObject, 3f); // tiempos opciónal
    }
}
