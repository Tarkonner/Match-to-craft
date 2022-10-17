using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;
    [SerializeField] AudioClip[] songs;
    private int currentSong = -1;

    [Header("Audio groups")]
    [SerializeField] AudioMixer mixer;
    [Header("Sliders")]
    public GameObject masterSliderHolder;
    public GameObject musicSliderHolder;
    public GameObject effektSliderHolder;
    private Slider masterSlider;
    private Slider musicSlider;
    private Slider effectSlider;
    private TextMeshProUGUI masterText;
    private TextMeshProUGUI effektText;
    private TextMeshProUGUI musicText;
    private float savedMasterVolume = 1;
    private float savedEffectVolume = 1;
    private float savedMusicVolume = 1;
    public float SavedMasterVolume { get { return savedMasterVolume; } }
    public float SavedEffectVolume { get { return savedEffectVolume; } }
    public float SavedMusicVolume { get { return savedMusicVolume; } }

    public bool Muted { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //Play song
        currentSong = Random.Range(0, songs.Length);
        audioSource.clip = songs[currentSong];
        audioSource.Play();
        StartCoroutine(PlayNextSong(audioSource.clip.length));

        //Sliders
        SetupSliders();
    }

    private IEnumerator PlayNextSong(float songTime)
    {
        yield return new WaitForSeconds(songTime);
        int newSong = Random.Range(0, songs.Length);

        while(newSong == currentSong)
            newSong = Random.Range(0, songs.Length);

        audioSource.clip = songs[newSong];
        audioSource.Play();
        currentSong = newSong;

        StartCoroutine(PlayNextSong(audioSource.clip.length));
    }

    public void SetupSliders()
    {
        masterSlider = masterSliderHolder.transform.GetChild(0).GetComponent<Slider>();
        masterSlider.value = savedMasterVolume;
        masterSlider.onValueChanged.AddListener(delegate { ChangeMasterVolume(masterSlider.value); });
        masterText = masterSliderHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        musicSlider = musicSliderHolder.transform.GetChild(0).GetComponent<Slider>();
        musicSlider.value = savedMusicVolume;
        musicSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(musicSlider.value); });
        musicText = musicSliderHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        effectSlider = effektSliderHolder.transform.GetChild(0).GetComponent<Slider>();
        effectSlider.value = savedEffectVolume;
        effectSlider.onValueChanged.AddListener(delegate { ChangeEffektVolume(effectSlider.value); });
        effektText = effektSliderHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public float VolumeCalculation(float value)
    {
        return Mathf.Log(value) * 20;
    }

    public void ChangeMasterVolume(float value)
    {
        mixer.SetFloat("MasterVolume", VolumeCalculation(value));
        savedMasterVolume = value;
        masterText.text = Mathf.Round(masterSlider.value * 100).ToString() + "%"; 
    }
    public void ChangeMusicVolume(float value)
    {
        mixer.SetFloat("MusicVolume", VolumeCalculation(value));
        savedMusicVolume = value;
        musicText.text = Mathf.Round(musicSlider.value * 100).ToString() + "%";
    }
    public void ChangeEffektVolume(float value)
    {
        mixer.SetFloat("EffektVolume", VolumeCalculation(value));
        savedEffectVolume = value;
        effektText.text = Mathf.Round(effectSlider.value * 100).ToString() + "%";
    }

    public void MuteAudio()
    {
        if(!Muted)
            ChangeMasterVolume(0.001f);  
        else
            ChangeMasterVolume(masterSlider.value);

        Muted = !Muted;
    }
}
