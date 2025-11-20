using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    [Header("Movimiento")]
    public float velocidadCaminar = 2f;
    public float velocidadCorrer = 5f;
    public float velocidadRotacion = 10f;

    [Header("Cámara")]
    [SerializeField] private Camera followCamera;

    [Header("Gravedad y Salto")]
    private Vector3 veloJugador;
    public Transform checkPiso;
    public float distanciaPiso = 0.4f;
    public LayerMask piso;
    public float gravedad = -9.81f;
    public float salto = 1f;

    private bool enPiso;
    private bool corriendo;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        
    }

    private void Update()
    {
        Movimientop();
    }

    public void Movimientop()
    {
        // --- Verificar si está en el suelo ---
        enPiso = Physics.CheckSphere(checkPiso.position, distanciaPiso, piso);

        if (enPiso && veloJugador.y < 0)
            veloJugador.y = -2f;

        // --- Entradas ---
        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");
        corriendo = Input.GetKey(KeyCode.LeftShift);

        // --- Movimiento relativo a la cámara ---
        Vector3 moviInput = Quaternion.Euler(0, followCamera.transform.eulerAngles.y, 0) * new Vector3(X, 0, Z);
        Vector3 moveDirection = moviInput.normalized;

        // --- Calcular velocidad según si corre o camina ---
        float velocidadActual = corriendo ? velocidadCorrer : velocidadCaminar;
        controller.Move(moveDirection * velocidadActual * Time.deltaTime);

        // --- Rotación del personaje ---
        if (moveDirection != Vector3.zero)
        {
            Quaternion rotacion = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, velocidadRotacion * Time.deltaTime);
        }

        // --- Salto ---
        if (Input.GetButtonDown("Jump") && enPiso)
        {
            veloJugador.y = Mathf.Sqrt(salto * -2.0f * gravedad);
            if (animator) animator.SetTrigger("Saltar");
        }

        // --- Aplicar gravedad ---
        veloJugador.y += gravedad * Time.deltaTime;
        controller.Move(veloJugador * Time.deltaTime);

        // --- Animaciones ---
        if (animator)
        {
            float magnitud = new Vector3(X, 0, Z).magnitude;
            animator.SetFloat("Velocidad", magnitud * (corriendo ? 2f : 1f));
            animator.SetBool("EnPiso", enPiso);
        }
    }

    // Visualizar el CheckPiso en la escena
    private void OnDrawGizmosSelected()
    {
        if (checkPiso != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(checkPiso.position, distanciaPiso);
        }
    }
}
