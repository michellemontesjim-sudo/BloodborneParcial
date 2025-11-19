using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item itemData;
    bool isPlayerNear = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            UIInteraccion.Instance.ShowText($"Presiona E para recoger {itemData.nombre}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            UIInteraccion.Instance.HideText();
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Inventario.instancia.Agregar(itemData);
            UIInteraccion.Instance.HideText();
            Destroy(gameObject);
        }
    }
}
