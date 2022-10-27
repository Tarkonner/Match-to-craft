using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI showText;

    public void ChangeText(string text)
    {
        showText.text = text;
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }
}
