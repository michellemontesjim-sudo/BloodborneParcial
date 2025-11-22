using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencia")]
    public GameObject highlight; // Imagen PNG del rayo

    [Header("Configuración estilo Bloodborne")]
    [Range(0f, 1f)]
    public float alphaHighlight = 0.8f;

    private Image highlightImage;

    private void Start()
    {
        highlightImage = highlight.GetComponent<Image>();

        // Apagar al inicio
        highlight.SetActive(false);

        // Ajustar opacidad inicial
        SetHighlightAlpha(alphaHighlight);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.SetActive(false);
    }

    // Control de opacidad
    private void SetHighlightAlpha(float alpha)
    {
        if (highlightImage != null)
        {
            Color color = highlightImage.color;
            color.a = alpha;
            highlightImage.color = color;
        }
    }
}

