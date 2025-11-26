using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarPlayer : MonoBehaviour
{
    public Image currentHealthBar;   // Barra roja que baja instantáneo
    public Image regainHealthBar;    // Barra del "regain" que baja lento (opcional)

    private float lerpSpeed = 5f;

    public void UpdateHealthBar(float current, float max)
    {
        float targetFill = current / max;

        // baja inmediato
        currentHealthBar.fillAmount = targetFill;

        // baja suave (regain)
        if (regainHealthBar != null)
            regainHealthBar.fillAmount = Mathf.Lerp(regainHealthBar.fillAmount, targetFill, Time.deltaTime * lerpSpeed);
    }
}
