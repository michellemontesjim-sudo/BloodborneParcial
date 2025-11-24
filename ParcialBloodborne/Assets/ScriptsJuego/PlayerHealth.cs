using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerHealth : MonoBehaviour
{
    public enum CombatState
    {
        Normal,
        Hurt,
        Dead
    }

        public float salud = 150f;

        public CombatState state = CombatState.Normal;

        public bool isDead = false;
        public bool isParrying = false;

        public Animator anim;

        public void TomarDaño(float daño)
        {
            // 1️⃣ Si está muerto → no hacer nada
            if (state == CombatState.Dead)
                return;

            // 2️⃣ Si está haciendo parry → no recibe daño
            if (isParrying)
            {
                Debug.Log("Parry bloqueó el daño!");
                return;
            }

            // 3️⃣ Si está en animación de dolor → bloqueo el daño extra
            if (state == CombatState.Hurt)
                return;

            // 4️⃣ Aplicar daño
            salud -= daño;

            if (salud > 0)
            {
                StartCoroutine(HurtState());
            }
            else
            {
                state = CombatState.Dead;
                anim.SetTrigger("Muerto");
            }
        }

        IEnumerator HurtState()
        {
            state = CombatState.Hurt;

            anim.SetTrigger("Herido");

            yield return new WaitForSeconds(0.3f); // duracion de anim de golpe

            state = CombatState.Normal;
        }
    
}
