using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOpcionesManager : MonoBehaviour
{
    [Header("Canvas que contiene el panel de opciones")]
    public GameObject canvasOpciones;

    public void ActivarOpciones()
    {
        if (canvasOpciones != null)
            canvasOpciones.SetActive(true);
    }

    public void DesactivarOpciones()
    {
        if (canvasOpciones != null)
            canvasOpciones.SetActive(false);
    }
}
