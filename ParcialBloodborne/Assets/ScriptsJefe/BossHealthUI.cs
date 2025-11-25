using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public BossController boss;   // referencia al jefe
    public Slider slider;         // referencia al Slider

    void Start()
    {
        // Si no arrastras nada en el inspector, intenta encontrar al boss
        if (boss == null)
            boss = FindObjectOfType<BossController>();

        // Configuramos el slider para usar valores normalizados 0–1
        if (slider != null)
            slider.value = 1f;
    }

    void Update()
    {
        if (boss == null || slider == null) return;

        float normalizedHealth = boss.currentHealth / boss.maxHealth;
        slider.value = Mathf.Clamp01(normalizedHealth);
    }
}

