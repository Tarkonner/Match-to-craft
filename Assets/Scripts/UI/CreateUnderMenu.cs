using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
public class CreateUnderMenu : SerializedMonoBehaviour
{
    [SerializeField] GameObject buttonPrefab;

    [Button("Make under menu")]
    public void UnderMenu()
    {
        GameObject canvas = gameObject.transform.parent.gameObject;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(TurnOff);

        GameObject header = Instantiate(new GameObject(), canvas.transform);
        var backButten = Instantiate(button, header.transform);
        
    }

    private void TurnOff() => gameObject.SetActive(false);
    private void TurnOn() => gameObject.SetActive(true);
}
