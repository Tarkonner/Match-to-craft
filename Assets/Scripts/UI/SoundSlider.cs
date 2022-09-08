using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] volumeType volumeToChange = volumeType.Master;
    [SerializeField] Slider volumeSlider;

    [SerializeField] TextMeshProUGUI procentText;

    void Start()
    {
        switch (volumeToChange)
        {
            case volumeType.Master:
                volumeSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.ChangeMasterVolume(volumeSlider.value); });
                procentText.text = Mathf.Round(volumeSlider.value * 100).ToString() + "%";
                break;
            case volumeType.Effekt:
                volumeSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.ChangeEffektVolume(volumeSlider.value); });
                procentText.text = Mathf.Round(volumeSlider.value * 100).ToString() + "%";
                break;
            case volumeType.Music:
                volumeSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.ChangeMusicVolume(volumeSlider.value); });
                procentText.text = Mathf.Round(volumeSlider.value * 100).ToString() + "%";
                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        switch (volumeToChange)
        {
            case volumeType.Master:
                volumeSlider.value = SoundManager.Instance.SavedMasterVolume;
                break;
            case volumeType.Effekt:
                volumeSlider.value = SoundManager.Instance.SavedEffectVolume;
                break;
            case volumeType.Music:
                volumeSlider.value = SoundManager.Instance.SavedMusicVolume;
                break;
            default:
                break;
        }
    }
}
