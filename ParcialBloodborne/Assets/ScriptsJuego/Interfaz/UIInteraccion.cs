using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIInteraccion : MonoBehaviour
{
    public static UIInteraccion Instance;

    public TMP_Text interactText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowText(string msg)
    {
        interactText.text = msg;
        interactText.gameObject.SetActive(true);
    }

    public void HideText()
    {
        interactText.gameObject.SetActive(false);
    }
}
