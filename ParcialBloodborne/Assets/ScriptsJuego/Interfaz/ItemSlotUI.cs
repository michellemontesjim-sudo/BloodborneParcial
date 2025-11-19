using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image icono;
    public Button boton;

    [HideInInspector] public Item item;
    private InventoryUI ui;

    public void Configurar(Item newItem, InventoryUI inventarioUI)
    {
        item = newItem;
        ui = inventarioUI;

        boton.onClick.RemoveAllListeners(); // evita duplicados

        if (item != null)
        {
            icono.sprite = item.icono;
            icono.enabled = true;
            boton.onClick.AddListener(AlHacerClick);
        }
        else
        {
            icono.enabled = false;
        }
    }

    public void AlHacerClick()
    {
        ui.MostrarDescripcion(item);
    }
}
