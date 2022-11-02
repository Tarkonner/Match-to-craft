using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI showText;
    private Image sprite;

    private void Awake()
    {
        sprite = GetComponent<Image>();
    }

    public void ChangeImageColor(Color targetColor)
    {
        sprite.color = targetColor;
    }


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
