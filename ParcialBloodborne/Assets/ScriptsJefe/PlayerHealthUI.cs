using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public BossFightPlayerHealth playerHealth;
    public Slider slider;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<BossFightPlayerHealth>();

        if (slider != null)
            slider.value = 1f;
    }

    void Update()
    {
        if (playerHealth == null || slider == null) return;

        float normalizedHealth = playerHealth.currentHealth / playerHealth.maxHealth;
        slider.value = Mathf.Clamp01(normalizedHealth);
    }
}

