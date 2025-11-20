using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
public class Inventario : MonoBehaviour
{
    public static Inventario instancia;

    private void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

    public List<Item> items = new List<Item>();  // inventario real
    public int espacio = 20; // limite de items

    public bool Agregar(Item item)
    {
        if (items.Count >= espacio)
        {
            Debug.Log("Inventario lleno");
            return false;
        }

        items.Add(item);
        Debug.Log("Añadido: " + item.nombre);
        return true;
    }

    public void Usar(Item item)
    {
        if (item.esConsumible)
        {
            PlayerHealth player = FindObjectOfType<PlayerHealth>();
            player.salud += item.valor;
            Debug.Log("Curado +" + item.valor);

            items.Remove(item);
        }
    }
}
