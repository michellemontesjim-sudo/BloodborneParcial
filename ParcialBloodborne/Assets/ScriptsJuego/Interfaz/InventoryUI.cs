using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject panelInventario;
    public Transform gridItems;
    public GameObject slotPrefab;

    [Header("Panel de Descripción")]
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDescripcion;
    public Button botonUsar;

    private bool inventarioAbierto;

    void Start()
    {
        // Sincronizar con el estado inicial del panel
        inventarioAbierto = panelInventario.activeSelf;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventarioAbierto)
                CerrarInventario();
            else
                AbrirInventario();
        }
    }

    public void AbrirInventario()
    {
        inventarioAbierto = true;
        panelInventario.SetActive(true);
        Time.timeScale = 0f;
        RefrescarInventario();

        // LIBERA EL CURSOR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarInventario()
    {
        inventarioAbierto = false;
        panelInventario.SetActive(false);
        Time.timeScale = 1f;

        // BLOQUEA EL CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefrescarInventario()
    {
        foreach (Transform child in gridItems)
            Destroy(child.gameObject);

        foreach (Item item in Inventario.instancia.items)
        {
            GameObject slot = Instantiate(slotPrefab, gridItems);
            ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
            slotUI.Configurar(item, this);
        }
    }

    public void MostrarDescripcion(Item item)
    {
        textoNombre.text = item.nombre;

        if (item.esConsumible)
        {
            textoDescripcion.text = "Consumible\nCuración: " + item.valor;
            botonUsar.gameObject.SetActive(true);
        }
        else
        {
            textoDescripcion.text = "Objeto";
            botonUsar.gameObject.SetActive(false);
        }

        botonUsar.onClick.RemoveAllListeners();
        botonUsar.onClick.AddListener(() => Usar(item));
    }

    public void Usar(Item item)
    {
        Inventario.instancia.Usar(item);
        RefrescarInventario();
        LimpiarDescripcion();
    }

    public void LimpiarDescripcion()
    {
        textoNombre.text = "";
        textoDescripcion.text = "";
        
    }
}
