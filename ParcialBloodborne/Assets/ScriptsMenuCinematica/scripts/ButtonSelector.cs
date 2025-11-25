using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Referencia visual")]
    public GameObject highlight; 

    [Header("Configuración estilo Bloodborne")]
    [Range(0f, 1f)]
    public float alphaHighlight = 0.8f;

    [Header("Sonidos del menú")]
    public AudioSource audioSource;
    public AudioClip hoverSound; 
    public AudioClip clickSound; 
    [Range(0f, 1f)]
    public float volume = 0.6f;

    private Image highlightImage;

    private void Start()
    {
        highlightImage = highlight.GetComponent<Image>();

        // Apagar el highlight al inicio
        highlight.SetActive(false);

        // Ajustar opacidad
        SetHighlightAlpha(alphaHighlight);

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f; 
            audioSource.volume = volume;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlight.SetActive(true);

        // Sonido al seleccionar
        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound, volume);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Sonido al hacer click
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound, volume);
    }

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