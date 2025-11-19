using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Componentes")]
    public Animator anim;

    
    private int cantClick;
    private bool canIClick;

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public int attackDamage = 25;

    [Header("Parry / Block")]
    public KeyCode parryKey = KeyCode.E;
    public float parryDuration = 0.3f;
    public float parryCooldown = 1f;
    public bool isParrying = false;
    private bool canParry = true;

    private PlayerHealth playerHealth;

    void Start()
    {
        cantClick = 0;
        canIClick = true;
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // ----- ATAQUE -----
        if (Input.GetButtonDown("Fire1"))
            StartCombo();

        // ----- PARRY -----
        if (Input.GetKeyDown(parryKey) && canParry)
            StartCoroutine(ParryWindow());

        // sincronizamos con PlayerHealth
        if (playerHealth != null)
            playerHealth.isParrying = isParrying;
    }

    // ---------------------------------------------------------------------
    // ATAQUE Y COMBO
    // ---------------------------------------------------------------------
    void StartCombo()
    {
        /*if (!canIClick)
            return;

        cantClick++;

        if (cantClick == 1)
        {
            
            anim.SetInteger("Ataque", 1);
        }*/

        if (canIClick)
        {
            cantClick++;
        }

        if (cantClick == 1)
        {
            anim.SetInteger("Ataque", 1);
        }
    }

    public void VerificarCombo()
    {
        /*canIClick = false;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("Ataque1"))
        {
            if (cantClick >= 2)
            {
                anim.SetInteger("Ataque", 2);
            }
            else
            {
                anim.SetInteger("Ataque", 0);
                cantClick = 0;
            }

            canIClick = true;
        }

        else if (info.IsName("Ataque2"))
        {
            if (cantClick >= 3)
            {
                anim.SetInteger("Ataque", 3);
            }
            else
            {
                anim.SetInteger("Ataque", 0);
                cantClick = 0;
            }

            canIClick = true;
        }

        else if (info.IsName("Ataque3"))
        {
            anim.SetInteger("Ataque", 0);
            cantClick = 0;
            canIClick = true;
        }*/

        canIClick = false;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ataque1") && cantClick == 1)
        {
            anim.SetInteger("Ataque", 0);
            canIClick = true;
            cantClick = 0;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ataque1") && cantClick >= 2)
        {
            anim.SetInteger("Ataque", 2);
            canIClick = true;
        }

        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ataque2") && cantClick == 2)
        {
            anim.SetInteger("Ataque", 0);
            canIClick = true;
            cantClick = 0;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ataque2") && cantClick >= 3)
        {
            anim.SetInteger("Ataque", 3);
            canIClick = true;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ataque3"))
        {
            anim.SetInteger("Ataque", 0);
            canIClick = true;
            cantClick = 0;
        }
    }

    // ---------------------------------------------------------------------
    // DAÑO AL ENEMIGO
    // ---------------------------------------------------------------------
    public void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.RecibirDaño(attackDamage);
                    Debug.Log("Jugador golpea al enemigo!");
                }
            }
        }
    }

    // ---------------------------------------------------------------------
    // PARRY
    // ---------------------------------------------------------------------
    IEnumerator ParryWindow()
    {
        canParry = false;
        isParrying = true;

        anim.SetTrigger("Parry"); // renombra tu animación si es Block u otra

        yield return new WaitForSeconds(parryDuration);
        isParrying = false;

        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }

    // ---------------------------------------------------------------------
    // GIZMOS
    // ---------------------------------------------------------------------
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
