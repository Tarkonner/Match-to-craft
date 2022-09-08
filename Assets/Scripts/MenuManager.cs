using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField] private GameObject masterVolume;
    [SerializeField] private GameObject musicVolume;
    [SerializeField] private GameObject effectVolume;

    private bool activeMenu = false;
    public bool ActiveMenu { get { return activeMenu; }}

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SoundManager.Instance.masterSliderHolder = masterVolume;
        SoundManager.Instance.musicSliderHolder = musicVolume;
        SoundManager.Instance.effektSliderHolder = effectVolume;
        SoundManager.Instance.SetupSliders();
    }

    public void Mute()
    {
        SoundManager.Instance.MuteAudio();
    }
}
