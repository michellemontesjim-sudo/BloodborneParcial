using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/Item")]
public class Item : ScriptableObject
{

    public string nombre;
    public Sprite icono;
    public bool esConsumible;
    public int valor;   // ej: cuánta vida cura, cuánto daño da, etc.

    [TextArea]
    public string descripcion;
}
